using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PeelingMesh : MonoBehaviour
{
    public ShellMeshContainer shellMeshContainer;
    public NativeMultiHashMap<int, int> multiHashMapVertIndexToSameVerticesIndices;

    [HideInInspector] public NativeArray<float3> vertices;
    [HideInInspector] public NativeArray<float2> uvs2ToClip;
    [HideInInspector] public NativeArray<int> triangles;
    [HideInInspector] public Mesh mesh;

    protected virtual void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;

        int vertexOrTriangleIndicesCount = mesh.vertices.Length;

        vertices = new NativeArray<float3>(vertexOrTriangleIndicesCount, Allocator.Persistent);
        uvs2ToClip = new NativeArray<float2>(vertexOrTriangleIndicesCount, Allocator.Persistent);
        triangles = new NativeArray<int>(vertexOrTriangleIndicesCount, Allocator.Persistent);

        JobDataSetter jobDataSetter = new JobDataSetter()
        {
            vertices = shellMeshContainer.vertices,
            uvs2ToClip = shellMeshContainer.uvs2ToClip,
            triangles = shellMeshContainer.triangles,
            targetVertices = vertices,
            targetUvs2ToClip = uvs2ToClip,
            targetTriangles = triangles,
        };

        jobDataSetter.ScheduleParallel(vertexOrTriangleIndicesCount, 1024, default).Complete();
    }

    protected virtual void OnDisable()
    {
        vertices.Dispose();
        uvs2ToClip.Dispose();
        triangles.Dispose();

        multiHashMapVertIndexToSameVerticesIndices.Dispose();
    }
}