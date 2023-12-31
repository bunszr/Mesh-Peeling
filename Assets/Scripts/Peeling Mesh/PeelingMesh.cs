using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PeelingMesh : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    [HideInInspector] public MaterialPropertyBlock materialPropertyBlock;
    ShellMeshContainer shellMeshContainer;
    public BoundsAndTriangleIndicesData boundsAndTriangleIndicesData;
    public NativeMultiHashMap<int, int> multiHashMapVertIndexToSameVerticesIndices;

    [HideInInspector] public NativeArray<float3> vertices;
    [HideInInspector] public NativeArray<float2> uvs;
    [HideInInspector] public NativeArray<float2> uvs2ToClip;
    [HideInInspector] public NativeArray<int> triangles;
    [HideInInspector] public Mesh mesh;

    public float PercentOfPeeling { get; set; }
    public int VertexOrTriangleIndicesCount { get; private set; }

    protected virtual void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        shellMeshContainer = GetComponentInParent<LevelDataHolder>().shellMeshContainer;

        VertexOrTriangleIndicesCount = mesh.vertices.Length;

        vertices = new NativeArray<float3>(VertexOrTriangleIndicesCount, Allocator.Persistent);
        uvs = new NativeArray<float2>(VertexOrTriangleIndicesCount, Allocator.Persistent);
        uvs2ToClip = new NativeArray<float2>(VertexOrTriangleIndicesCount, Allocator.Persistent);
        triangles = new NativeArray<int>(VertexOrTriangleIndicesCount, Allocator.Persistent);

        JobDataSetter jobDataSetter = new JobDataSetter()
        {
            vertices = shellMeshContainer.vertices,
            uvs2ToClip = shellMeshContainer.uvs2ToClip,
            uvs = shellMeshContainer.uvs,
            triangles = shellMeshContainer.triangles,
            targetVertices = vertices,
            targetUvs = uvs,
            targetUvs2ToClip = uvs2ToClip,
            targetTriangles = triangles,
        };

        jobDataSetter.ScheduleParallel(VertexOrTriangleIndicesCount, 1024, default).Complete();
    }

    public void UpdateMeshUv2ToClip()
    {
        mesh.SetUVs(1, uvs2ToClip);
    }

    protected virtual void OnDisable()
    {
        vertices.Dispose();
        uvs.Dispose();
        uvs2ToClip.Dispose();
        triangles.Dispose();

        multiHashMapVertIndexToSameVerticesIndices.Dispose();
    }
}