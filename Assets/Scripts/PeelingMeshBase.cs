using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public abstract class PeelingMeshBase : MonoBehaviour
{
    public Vector3[] vertices;
    public Vector2[] uvs;
    public Vector2[] uvs2ToClip;
    public int[] triangles;

    public Mesh mesh;

    protected virtual void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        uvs = meshFilter.mesh.uv;
        uvs2ToClip = meshFilter.mesh.uv2;
        triangles = meshFilter.mesh.triangles;
        mesh = meshFilter.mesh;
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