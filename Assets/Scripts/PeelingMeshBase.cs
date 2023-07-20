using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Unity.Collections;
using Unity.Mathematics;

public abstract class PeelingMeshBase : MonoBehaviour
{
    public NativeArray<float3> vertices;
    public NativeArray<float2> uvs;
    public NativeArray<float2> uvs2ToClip;
    public NativeArray<int> triangles;

    public Mesh mesh;

    protected virtual void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        Vector3[] _vertices = meshFilter.mesh.vertices;
        Vector2[] _uvs = meshFilter.mesh.uv;
        Vector2[] _uvs2 = meshFilter.mesh.uv2;
        vertices = new NativeArray<float3>(_vertices.Length, Allocator.Persistent);
        uvs = new NativeArray<float2>(_vertices.Length, Allocator.Persistent);
        uvs2ToClip = new NativeArray<float2>(_vertices.Length, Allocator.Persistent);
        for (int i = 0; i < vertices.Length; i++)
        {
            this.vertices[i] = _vertices[i];
            this.uvs[i] = _uvs[i];
            this.uvs2ToClip[i] = _uvs2[i];
        }


        int[] _triangles = meshFilter.mesh.triangles;
        triangles = new NativeArray<int>(_triangles.Length, Allocator.Persistent);
        for (int i = 0; i < triangles.Length; i++) this.triangles[i] = _triangles[i];

        mesh = meshFilter.mesh;
    }

    private void OnDisable()
    {
        vertices.Dispose();
        uvs.Dispose();
        uvs2ToClip.Dispose();
        triangles.Dispose();
    }
}










// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Random = UnityEngine.Random;

// public class PeelingMesh : MonoBehaviour
// {
//     public Vector3[] vertices;
//     public Vector2[] uvs;
//     public Vector2[] uvs2;
//     public int[] triangles;

//     public Mesh mesh;

//     public bool isReverse;

//     private void Awake()
//     {
//         MeshFilter meshFilter = GetComponent<MeshFilter>();
//         vertices = meshFilter.mesh.vertices;
//         uvs = meshFilter.mesh.uv;
//         uvs2 = meshFilter.mesh.uv;
//         triangles = meshFilter.mesh.triangles;
//         mesh = meshFilter.mesh;

//         if (isReverse)
//         {
//             for (int i = 0; i < 1000; i += 3)
//             {
//                 uvs2[triangles[i]] = Vector2.down;
//                 uvs2[triangles[i + 1]] = Vector2.down;
//                 uvs2[triangles[i + 2]] = Vector2.down;
//             }
//         }
//         else
//         {
//             for (int i = 1000; i < uvs2.Length; i += 3)
//             {
//                 uvs2[triangles[i]] = Vector2.up;
//                 uvs2[triangles[i + 1]] = Vector2.up;
//                 uvs2[triangles[i + 2]] = Vector2.up;
//             }
//         }
//         mesh.uv2 = uvs2;
//     }

//     // private void Update()
//     // {
//     //     for (int i = 0; i < 21; i += 3)
//     //     {
//     //         vertices[triangles[i]] += Vector3.up * Time.deltaTime * i;
//     //         vertices[triangles[i + 1]] += Vector3.up * Time.deltaTime * i;
//     //         vertices[triangles[i + 2]] += Vector3.up * Time.deltaTime * i;
//     //     }
//     //     mesh.vertices = vertices;
//     // }
// }