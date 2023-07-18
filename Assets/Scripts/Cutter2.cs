using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

public class Cutter2 : MonoBehaviour
{
    public PeelingMesh peelingMesh;
    public PeelingShellMesh shellMeshPrefab;
    public PeelingShellMesh currPeelingShellMesh;
    public Transform shellCenterT;
    public CubeBounds cubeBounds;

    public float delay = .2f;
    float nextTime;

    State state;
    enum State
    {
        None, Start, End
    }

    public event System.Action onStartPeling;
    public event System.Action onEndPeling;

    Vector3[] enterShellCenterTLocalPoints;

    float defaultTotalEdgeSqrDst;

    // NativeList<int> insadeTriangleIndices;
    public List<int> insadeTriangleIndices;
    public int LengthInsadeTriangleIndices => insadeTriangleIndices.Count;

    private void Start()
    {
        enterShellCenterTLocalPoints = new Vector3[peelingMesh.vertices.Length];

        defaultTotalEdgeSqrDst = Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[0]] - peelingMesh.vertices[peelingMesh.triangles[1]]) + Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[1]] - peelingMesh.vertices[peelingMesh.triangles[2]]) + Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[2]] - peelingMesh.vertices[peelingMesh.triangles[0]]);

        // insadeTriangleIndices = new NativeList<int>(Allocator.Persistent);
        insadeTriangleIndices = new List<int>();
    }

    private void Update()
    {
        bool hasPeelingInFrame = false;

        for (int i = 0; i < peelingMesh.triangles.Length; i += 3)
        {
            if (peelingMesh.triangles[i + 0] == 0 && peelingMesh.triangles[i + 1] == 0 && peelingMesh.triangles[i + 2] == 0)
            {
                if (currPeelingShellMesh != null)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3 world = shellCenterT.localToWorldMatrix.MultiplyPoint3x4(enterShellCenterTLocalPoints[currPeelingShellMesh.triangles[i + j]]);
                        Vector3 local = currPeelingShellMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(world);
                        currPeelingShellMesh.vertices[currPeelingShellMesh.triangles[i + j]] = local;
                    }
                }
                continue;
            }

            Vector3 localP = peelingMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
            float sqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2);
            bool hasInsade = Vector3.SqrMagnitude(localP - peelingMesh.vertices[peelingMesh.triangles[i + 0]]) < sqrRadius &&
                             Vector3.SqrMagnitude(localP - peelingMesh.vertices[peelingMesh.triangles[i + 1]]) < sqrRadius &&
                             Vector3.SqrMagnitude(localP - peelingMesh.vertices[peelingMesh.triangles[i + 2]]) < sqrRadius;
            if (hasInsade)
            {
                hasPeelingInFrame = true;

                if (LengthInsadeTriangleIndices >= 81)
                {
                    insadeTriangleIndices.RemoveAt(0);
                    insadeTriangleIndices.RemoveAt(0);
                    insadeTriangleIndices.RemoveAt(0);
                }
                if (LengthInsadeTriangleIndices > 3 && currPeelingShellMesh != null)
                {
                    Debug.Log("aaaaa");
                    NewMethod(i);
                }
                insadeTriangleIndices.Add(i + 0); insadeTriangleIndices.Add(i + 1); insadeTriangleIndices.Add(i + 2);

                if (state == State.None || state == State.End)
                {
                    // Debug.Break();
                    state = State.Start;
                    currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, peelingMesh.transform.position, peelingMesh.transform.rotation, peelingMesh.transform);
                    currPeelingShellMesh.tri();
                    onStartPeling?.Invoke();
                    enterShellCenterTLocalPoints = new Vector3[peelingMesh.vertices.Length];
                    insadeTriangleIndices.Clear();
                    Debug.Log("Start");
                }

                for (int j = 0; j < 3; j++)
                {
                    Vector3 world = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(peelingMesh.vertices[peelingMesh.triangles[i + j]]);
                    Vector3 localShellCenterT = shellCenterT.worldToLocalMatrix.MultiplyPoint3x4(world);
                    enterShellCenterTLocalPoints[peelingMesh.triangles[i + j]] = localShellCenterT;
                    // Debug.DrawLine(Vector3.up * 5, world, Color.red);


                    // Vector3 world = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.triangles[i + j]]);
                    // Vector3 localShellCenterT = shellCenterT.worldToLocalMatrix.MultiplyPoint3x4(world);
                    // enterShellCenterTLocalPoints[peelingMesh.triangles[i + j]] = localShellCenterT;
                    // // Debug.DrawLine(Vector3.up * 5, world, Color.red);
                }

                nextTime = Time.time + delay;

                currPeelingShellMesh.triangles[i + 0] = peelingMesh.triangles[i + 0];
                currPeelingShellMesh.triangles[i + 1] = peelingMesh.triangles[i + 1];
                currPeelingShellMesh.triangles[i + 2] = peelingMesh.triangles[i + 2];

                peelingMesh.triangles[i + 0] = 0;
                peelingMesh.triangles[i + 1] = 0;
                peelingMesh.triangles[i + 2] = 0;
            }
        }

        if (Time.time > nextTime)
        {
            if (state == State.Start && !hasPeelingInFrame)
            {
                state = State.End;
                currPeelingShellMesh.Throw(transform.forward + transform.up);
                currPeelingShellMesh = null;
                onEndPeling?.Invoke();
                Debug.Log("end");
            }
        }

        if (currPeelingShellMesh != null)
        {
            currPeelingShellMesh.mesh.vertices = currPeelingShellMesh.vertices;
            currPeelingShellMesh.mesh.triangles = currPeelingShellMesh.triangles;
        }


        peelingMesh.mesh.triangles = peelingMesh.triangles;
    }

    private void NewMethod(int i)
    {
        for (int currThreeIndicesIndex = i; currThreeIndicesIndex < i + 3; currThreeIndicesIndex++)
        {
            int vertIndex = peelingMesh.originalTriangles[currThreeIndicesIndex];
            int[] sameVertexIndices = peelingMesh.trianglesExtraDatas[currThreeIndicesIndex].sameVertexIndices;

            // Vector3 p = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(peelingMesh.vertices[peelingMesh.originalTriangles[currThreeIndicesIndex]]);
            // Debug.DrawLine(p, p + Vector3.up * .01f, Color.red);

            for (int j = 0; j < sameVertexIndices.Length; j++)
            {
                for (int k = 0; k < LengthInsadeTriangleIndices; k++)
                {
                    int vertIndexB = peelingMesh.originalTriangles[insadeTriangleIndices[k]];
                    // Debug.Log(array[j] + " = " + peelingMesh.originalTriangles[insadeTriangleIndices[k]]);
                    if (sameVertexIndices[j] == vertIndexB && vertIndex != sameVertexIndices[j])
                    {
                        peelingMesh.vertices[vertIndex] = currPeelingShellMesh.vertices[vertIndexB];
                        // Vector3 pA = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.originalTriangles[currThreeIndicesIndex]]);
                        // Debug.DrawLine(pA + new Vector3(.05f, .05f), pA, Color.red);
                        // Debug.DrawLine(pB + new Vector3(-.05f, .05f), pB, Color.yellow);

                    }
                }
                // VISUALİZE SAME VERTEX INDIES IS CORRECT
                // float angle = j / (float)sameVertexIndices.Length * 360f;
                // Vector3 t = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(0, 1, 1) * .01f;
                // Vector3 pB = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[sameVertexIndices[j]]);
                // Debug.DrawLine(pB, pB + t, Color.red);
            }
        }
    }

    // private void NewMethod(int i)
    // {
    //     for (int currThreeIndicesIndex = i; currThreeIndicesIndex < i + 3; currThreeIndicesIndex++)
    //     {
    //         int[] array = peelingMesh.trianglesExtraDatas[currThreeIndicesIndex].sameVertexIndices;
    //         for (int j = 0; j < array.Length; j++)
    //         {
    //             for (int k = 0; k < LengthInsadeTriangleIndices; k++)
    //             {
    //                 // Debug.Log(array[j] + " = " + peelingMesh.originalTriangles[insadeTriangleIndices[k]]);
    //                 if (array[j] == peelingMesh.originalTriangles[insadeTriangleIndices[k]])
    //                 {
    //                     Debug.Log("enter");
    //                     int vertIndex = peelingMesh.originalTriangles[currThreeIndicesIndex];
    //                     peelingMesh.vertices[vertIndex] = currPeelingShellMesh.vertices[peelingMesh.originalTriangles[insadeTriangleIndices[k]]];

    //                     Vector3 pA = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.originalTriangles[currThreeIndicesIndex]]);
    //                     Vector3 pB = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.originalTriangles[insadeTriangleIndices[k]]]);
    //                     Debug.DrawLine(pA + new Vector3(.05f, .05f), pA, Color.red);
    //                     Debug.DrawLine(pB + new Vector3(-.05f, .05f), pB, Color.yellow);

    //                 }
    //             }
    //         }
    //     }
    // }
}













// using System.Collections.Generic;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class Cutter2 : MonoBehaviour
// {
//     public PeelingMesh peelingMesh;
//     public PeelingShellMesh shellMeshPrefab;
//     public PeelingShellMesh currPeelingShellMesh;
//     public Transform shellCenterT;
//     public CubeBounds cubeBounds;

//     public float delay = .2f;
//     float nextTime;

//     State state;
//     enum State
//     {
//         None, Start, End
//     }

//     public event System.Action onStartPeling;
//     public event System.Action onEndPeling;

//     Vector3[] enterShellCenterTLocalPoints;

//     float defaultTotalEdgeSqrDst;

//     private void Start()
//     {
//         enterShellCenterTLocalPoints = new Vector3[peelingMesh.vertices.Length];

//         defaultTotalEdgeSqrDst = Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[0]] - peelingMesh.vertices[peelingMesh.triangles[1]]) + Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[1]] - peelingMesh.vertices[peelingMesh.triangles[2]]) + Vector3.SqrMagnitude(peelingMesh.vertices[peelingMesh.triangles[2]] - peelingMesh.vertices[peelingMesh.triangles[0]]);
//     }

//     private void Update()
//     {
//         bool hasPeelingInFrame = false;

//         for (int i = 0; i < peelingMesh.triangles.Length; i += 3)
//         {
//             if (peelingMesh.triangles[i + 0] == 0 && peelingMesh.triangles[i + 1] == 0 && peelingMesh.triangles[i + 2] == 0)
//             {
//                 if (currPeelingShellMesh != null)
//                 {
//                     for (int j = 0; j < 3; j++)
//                     {
//                         Vector3 world = shellCenterT.localToWorldMatrix.MultiplyPoint3x4(enterShellCenterTLocalPoints[currPeelingShellMesh.triangles[i + j]]);
//                         Vector3 local = currPeelingShellMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(world);
//                         currPeelingShellMesh.vertices[currPeelingShellMesh.triangles[i + j]] = local;
//                     }
//                 }
//                 continue;
//             }

//             Vector3 localP = peelingMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
//             float sqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2);
//             Vector3 mid = (peelingMesh.vertices[peelingMesh.triangles[i]] + peelingMesh.vertices[peelingMesh.triangles[i + 1]] + peelingMesh.vertices[peelingMesh.triangles[i + 2]]) / 3f;
//             float sqrDst = Vector3.SqrMagnitude(localP - mid);
//             if (sqrDst < sqrRadius)
//             {
//                 hasPeelingInFrame = true;

//                 if (state == State.None || state == State.End)
//                 {
//                     // Debug.Break();
//                     state = State.Start;
//                     currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, peelingMesh.transform.position, peelingMesh.transform.rotation, peelingMesh.transform);
//                     currPeelingShellMesh.tri();
//                     onStartPeling?.Invoke();
//                     enterShellCenterTLocalPoints = new Vector3[peelingMesh.vertices.Length];
//                     Debug.Log("Start");
//                 }

//                 // Vector3 pA = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 0]]);
//                 // Vector3 pB = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 1]]);
//                 // Vector3 pC = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 2]]);

//                 // Debug.DrawLine(pA, pB, Color.red);
//                 // Debug.DrawLine(pB, pC, Color.red);
//                 // Debug.DrawLine(pC, pA, Color.red);


//                 for (int j = 0; j < 3; j++)
//                 {
//                     Vector3 world = currPeelingShellMesh.transform.localToWorldMatrix.MultiplyPoint3x4(peelingMesh.vertices[peelingMesh.triangles[i + j]]);
//                     Vector3 localShellCenterT = shellCenterT.worldToLocalMatrix.MultiplyPoint3x4(world);
//                     enterShellCenterTLocalPoints[peelingMesh.triangles[i + j]] = localShellCenterT;
//                     // Debug.DrawLine(Vector3.up * 5, world, Color.red);


//                     // Vector3 world = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[peelingMesh.triangles[i + j]]);
//                     // Vector3 localShellCenterT = shellCenterT.worldToLocalMatrix.MultiplyPoint3x4(world);
//                     // enterShellCenterTLocalPoints[peelingMesh.triangles[i + j]] = localShellCenterT;
//                     // // Debug.DrawLine(Vector3.up * 5, world, Color.red);
//                 }

//                 nextTime = Time.time + delay;

//                 currPeelingShellMesh.triangles[i + 0] = peelingMesh.triangles[i + 0];
//                 currPeelingShellMesh.triangles[i + 1] = peelingMesh.triangles[i + 1];
//                 currPeelingShellMesh.triangles[i + 2] = peelingMesh.triangles[i + 2];

//                 peelingMesh.triangles[i + 0] = 0;
//                 peelingMesh.triangles[i + 1] = 0;
//                 peelingMesh.triangles[i + 2] = 0;
//             }
//         }

//         if (Time.time > nextTime)
//         {
//             if (state == State.Start && !hasPeelingInFrame)
//             {
//                 state = State.End;
//                 currPeelingShellMesh.Throw(transform.forward + transform.up);
//                 currPeelingShellMesh = null;
//                 onEndPeling?.Invoke();
//                 Debug.Log("end");
//             }
//         }

//         if (currPeelingShellMesh != null)
//         {
//             // for (int i = 0; i < peelingMesh.triangles.Length - 3; i += 1)
//             // {
//             //     float totalEdgeSqrDst = Vector3.SqrMagnitude(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 0]] - currPeelingShellMesh.vertices[peelingMesh.triangles[i + 1]]) + Vector3.SqrMagnitude(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 1]] - currPeelingShellMesh.vertices[peelingMesh.triangles[i + 2]]) + Vector3.SqrMagnitude(currPeelingShellMesh.vertices[peelingMesh.triangles[i + 2]] - currPeelingShellMesh.vertices[peelingMesh.triangles[i + 0]]);

//             //     if (totalEdgeSqrDst > defaultTotalEdgeSqrDst * 10f)
//             //     {
//             //         currPeelingShellMesh.triangles[i + 0] = 0;
//             //         currPeelingShellMesh.triangles[i + 1] = 0;
//             //         currPeelingShellMesh.triangles[i + 2] = 0;
//             //     }
//             // }

//             currPeelingShellMesh.mesh.vertices = currPeelingShellMesh.vertices;
//             currPeelingShellMesh.mesh.triangles = currPeelingShellMesh.triangles;
//         }


//         peelingMesh.mesh.triangles = peelingMesh.triangles;
//     }

//     [Button]
//     public void Ins()
//     {
//         currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, peelingMesh.transform.position, peelingMesh.transform.rotation, peelingMesh.transform);
//     }
// }