using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class PeelingMesh : PeelingMeshBase
{
    public Data[] trianglesExtraDatas;

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        triangles = meshFilter.mesh.triangles;
        mesh = meshFilter.mesh;

        trianglesExtraDatas = new Data[triangles.Length];

        List<int> triangleIndicesInSamePosition = new List<int>();
        for (int i = 0; i < triangles.Length; i++)
        {
            Vector3 v = vertices[triangles[i]];
            triangleIndicesInSamePosition.Clear();
            for (int j = 0; j < triangles.Length; j++)
            {
                float sqrDst = Vector3.SqrMagnitude(v - vertices[triangles[j]]);
                if (sqrDst < .001f)
                {
                    triangleIndicesInSamePosition.Add(triangles[j]);
                }
            }
            trianglesExtraDatas[i] = new Data(triangleIndicesInSamePosition.ToArray());
        }




        // public bool show = false;

        // private void OnDrawGizmosSelected()
        // {
        //     if (show)
        //     {
        //         Gizmos.matrix = transform.worldToLocalMatrix;
        //         for (int i = 0; i < vertices.Length; i++)
        //         {
        //             float time = i / (float)vertices.Length;
        //             Gizmos.color = Color.Lerp(Color.black, Color.red, time);
        //             Gizmos.DrawSphere(vertices[i], .005f);
        //         }
        //     }
        // }
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