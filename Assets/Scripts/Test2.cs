using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Test2 : MonoBehaviour
{
    public PeelingMesh peelingMesh;

    public Vector3[] vertices;
    public int[] triangles;

    public Mesh mesh;

    public Data[] trianglesExtraDatas;

    private void Start()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        triangles = meshFilter.mesh.triangles;
        mesh = meshFilter.mesh;

        trianglesExtraDatas = new Data[peelingMesh.triangles.Length];

        List<int> triangleIndicesInSamePosition = new List<int>();
        for (int i = 0; i < peelingMesh.triangles.Length; i++)
        {
            Vector3 v = peelingMesh.vertices[peelingMesh.triangles[i]];
            triangleIndicesInSamePosition.Clear();
            for (int j = 0; j < peelingMesh.triangles.Length; j++)
            {
                float sqrDst = Vector3.SqrMagnitude(v - peelingMesh.vertices[peelingMesh.triangles[j]]);
                if (sqrDst < .001f)
                {
                    triangleIndicesInSamePosition.Add(peelingMesh.triangles[j]);
                }
            }
            trianglesExtraDatas[i] = new Data(triangleIndicesInSamePosition.ToArray());
        }

        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (triangles[i + 0] == 0 && triangles[i + 1] == 0 && triangles[i + 2] == 0) continue;

            for (int j = 0; j < 3; j++)
            {
                int triangleIndex = i + j;
                HasZero(triangleIndex, out bool hasZero);

                if (!hasZero)
                {
                    Vector3 v = vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[1]];
                    for (int k = 0; k < trianglesExtraDatas[triangleIndex].sameVertexIndices.Length; k++)
                    {
                        vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[k]] = v;
                        // Debug.Log(vertices[datas[triangles[i + j]].indices[k]] + "   " + triangles[i + j] + "    " + datas[triangles[i + j]].indices[k]);
                    }
                }
            }
        }

        mesh.vertices = vertices;
    }

    public void HasZero(int triangleIndex, out bool hasZero)
    {
        hasZero = false;
        for (int k = 0; k < trianglesExtraDatas[triangleIndex].sameVertexIndices.Length; k++)
        {
            // Debug.LogError(triangleIndex + " = " + trianglesExtraDatas[triangleIndex].sameVertexIndices[k]);
            int index = trianglesExtraDatas[triangleIndex].sameVertexIndices[k];
            if (triangles[index] == 0)
            {
                // Vector3 pA = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[k]]);
                // Debug.LogError("ass");
                // Debug.DrawLine(Vector3.zero, pA, Color.red);
                hasZero = true;
                break;
            }
        }
    }

    // public void HasZero(int triangleIndex, out bool hasZero)
    // {
    //     hasZero = false;
    //     int aaa = 0;
    //     for (int k = 0; k < trianglesExtraDatas[triangleIndex].sameVertexIndices.Length; k++)
    //     {
    //         // Debug.LogError(triangleIndex + " = " + trianglesExtraDatas[triangleIndex].sameVertexIndices[k]);
    //         int index = trianglesExtraDatas[triangleIndex].sameVertexIndices[k];
    //         if (triangles[index] == 0)
    //         {
    //             aaa++;
    //             Vector3 pA = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[k]]);
    //             // Debug.LogError("ass");
    //             // Debug.DrawLine(Vector3.zero, pA, Color.red);
    //             if (aaa == 2)
    //             {
    //                 hasZero = true;
    //                 break;

    //             }
    //         }
    //     }
    // }
}

[System.Serializable]
public struct Data
{
    public int[] sameVertexIndices;

    public Data(int[] indices)
    {
        this.sameVertexIndices = indices;
    }
}











// using System.Collections.Generic;
// using Unity.Collections;
// using Unity.Jobs;
// using UnityEngine;

// public class Test2 : MonoBehaviour
// {
//     public PeelingMesh peelingMesh;

//     public Vector3[] vertices;
//     public int[] triangles;

//     public Mesh mesh;

//     public Data[] trianglesExtraDatas;

//     private void Start()
//     {
//         MeshFilter meshFilter = GetComponent<MeshFilter>();
//         vertices = meshFilter.mesh.vertices;
//         triangles = meshFilter.mesh.triangles;
//         mesh = meshFilter.mesh;

//         trianglesExtraDatas = new Data[peelingMesh.triangles.Length];

//         List<int> triangleIndicesInSamePosition = new List<int>();
//         for (int i = 0; i < peelingMesh.triangles.Length; i++)
//         {
//             Vector3 v = peelingMesh.vertices[peelingMesh.triangles[i]];
//             triangleIndicesInSamePosition.Clear();
//             for (int j = 0; j < peelingMesh.triangles.Length; j++)
//             {
//                 float sqrDst = Vector3.SqrMagnitude(v - peelingMesh.vertices[peelingMesh.triangles[j]]);
//                 if (sqrDst < .001f)
//                 {
//                     triangleIndicesInSamePosition.Add(peelingMesh.triangles[j]);
//                 }
//             }
//             trianglesExtraDatas[i] = new Data(triangleIndicesInSamePosition.ToArray());
//         }

//         // for (int i = 0; i < triangles.Length; i++)
//         // {
//         //     int activeVertIndex = 0;
//         //     for (int j = 0; j < datas[i].indices.Length; j++)
//         //     {
//         //         if (datas[i].indices[j] == 0)
//         //         {
//         //             activeVertIndex = -1;
//         //             break;
//         //         }
//         //     }

//         //     if (activeVertIndex != -1)
//         //     {
//         //         Vector3 v = vertices[datas[i].indices[0]];
//         //         for (int j = 0; j < datas[i].indices.Length; j++)
//         //         {
//         //             vertices[datas[i].indices[j]] = v;
//         //         }
//         //     }
//         // }

//         for (int i = 0; i < triangles.Length; i += 3)
//         {
//             if (triangles[i + 0] == 0 && triangles[i + 1] == 0 && triangles[i + 2] == 0) continue;

//             for (int j = 0; j < 3; j++)
//             {
//                 int triangleIndex = i + j;
//                 HasZero(triangleIndex, out bool hasZero);

//                 // Vector3 pA = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[i + j].sameVertexIndices[0]]);
//                 // Vector3 pB = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[i + j].sameVertexIndices[1]]);
//                 // Vector3 pC = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[i + j].sameVertexIndices[2]]);

//                 // Debug.DrawLine(pA, pB, Color.red);
//                 // Debug.DrawLine(pB, pC, Color.red);
//                 // Debug.DrawLine(pC, pA, Color.red);

//                 if (!hasZero)
//                 {
//                     Vector3 v = vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[1]];
//                     for (int k = 0; k < trianglesExtraDatas[triangleIndex].sameVertexIndices.Length; k++)
//                     {
//                         vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[k]] = v;
//                         // Debug.Log(vertices[datas[triangles[i + j]].indices[k]] + "   " + triangles[i + j] + "    " + datas[triangles[i + j]].indices[k]);
//                     }
//                 }
//             }
//         }

//         mesh.vertices = vertices;
//     }

//     public void HasZero(int triangleIndex, out bool hasZero)
//     {
//         hasZero = false;
//         for (int k = 0; k < trianglesExtraDatas[triangleIndex].sameVertexIndices.Length; k++)
//         {
//             // Debug.LogError(triangleIndex + " = " + trianglesExtraDatas[triangleIndex].sameVertexIndices[k]);
//             if (trianglesExtraDatas[triangleIndex].sameVertexIndices[k] == 0)
//             {
//                 Vector3 pA = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[trianglesExtraDatas[triangleIndex].sameVertexIndices[k]]);
//                 Debug.LogError("ass");
//                 Debug.DrawLine(Vector3.zero, pA, Color.red);
//                 hasZero = true;
//                 break;
//             }
//         }
//     }
// }

// [System.Serializable]
// public struct Data
// {
//     public int[] sameVertexIndices;

//     public Data(int[] indices)
//     {
//         this.sameVertexIndices = indices;
//     }
// }