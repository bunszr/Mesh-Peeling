using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public class ShellMeshContainer : MonoBehaviour
{
    public ShellMeshBase shellMeshPrefab;
    [Sirenix.OdinInspector.ReadOnly, Sirenix.OdinInspector.ShowInInspector] Queue<ShellMeshBase> peelingShellMeshQueue;

    [HideInInspector] public NativeArray<float3> vertices;
    [HideInInspector] public NativeArray<float2> uvs2ToClip;
    [HideInInspector] public NativeArray<int> triangles;
    [HideInInspector] public ShellMeshBase CurrShellMesh;

    private void Awake()
    {
        peelingShellMeshQueue = new Queue<ShellMeshBase>(GetComponentsInChildren<ShellMeshBase>(true));

        MeshFilter meshFilter = shellMeshPrefab.GetComponent<MeshFilter>();
        Vector3[] _vertices = meshFilter.sharedMesh.vertices;
        Vector2[] _uvs2 = meshFilter.sharedMesh.uv2;
        int[] _triangles = meshFilter.sharedMesh.triangles;
        vertices = new NativeArray<float3>(_vertices.Length, Allocator.Persistent);
        uvs2ToClip = new NativeArray<float2>(_vertices.Length, Allocator.Persistent);
        triangles = new NativeArray<int>(_triangles.Length, Allocator.Persistent);

        for (int i = 0; i < vertices.Length; i++)
        {
            this.vertices[i] = _vertices[i];
            this.uvs2ToClip[i] = _uvs2[i];
            this.triangles[i] = _triangles[i];
        }

        int vertexOrTriangleIndicesCount = vertices.Length;

        foreach (var shell in peelingShellMeshQueue)
        {
            shell.mesh = shell.GetComponent<MeshFilter>().mesh; // This process takes 8ms.
            shell.vertices = new NativeArray<float3>(vertexOrTriangleIndicesCount, Allocator.Persistent);
            shell.uvs2ToClip = new NativeArray<float2>(vertexOrTriangleIndicesCount, Allocator.Persistent);
            shell.triangles = new NativeArray<int>(vertexOrTriangleIndicesCount, Allocator.Persistent);

            JobDataSetter jobDataSetter = new JobDataSetter()
            {
                vertices = vertices,
                uvs2ToClip = uvs2ToClip,
                triangles = triangles,
                targetVertices = shell.vertices,
                targetUvs2ToClip = shell.uvs2ToClip,
                targetTriangles = shell.triangles,
            };

            jobDataSetter.ScheduleParallel(vertexOrTriangleIndicesCount, 2048, default).Complete();
        }
    }

    public ShellMeshBase Rent()
    {
        ShellMeshBase peelingShellMesh = peelingShellMeshQueue.Dequeue();
        peelingShellMesh.gameObject.SetActive(true);
        peelingShellMesh.SetUvToClipValueNegative();
        return peelingShellMesh;
    }

    public void Return(ShellMeshBase shellMesh)
    {
        peelingShellMeshQueue.Enqueue(shellMesh);
        shellMesh.transform.parent = transform;
    }
}
