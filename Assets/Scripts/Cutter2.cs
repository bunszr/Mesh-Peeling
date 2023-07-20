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
    public CubeBounds cubeBounds;
    public Transform shellCenterT;

    public float delay = .2f;
    float nextTime;

    State state;
    enum State
    {
        None, Start, End
    }

    public event System.Action onStartPeling;
    public event System.Action onEndPeling;

    float defaultTotalEdgeSqrDst;

    // NativeList<int> insadeTriangleIndices;
    public List<int> insadeTriangleIndices = new List<int>();
    public int LengthInsadeTriangleIndices => insadeTriangleIndices.Count;

    private void Update()
    {
        bool hasPeelingInFrame = false;

        for (int i = 0; i < peelingMesh.triangles.Length; i += 3)
        {
            if (peelingMesh.uvs2ToClip[peelingMesh.triangles[i + 0]].y < 0)
            {
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

                if (state == State.None || state == State.End)
                {
                    // Debug.Break();
                    state = State.Start;
                    currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, shellCenterT.transform.position, shellCenterT.transform.rotation, transform.parent);
                    currPeelingShellMesh.tri();
                    onStartPeling?.Invoke();
                    insadeTriangleIndices.Clear();
                    Debug.Log("Start");
                }

                for (int j = 0; j < 3; j++)
                {
                    Vector3 world = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(peelingMesh.vertices[peelingMesh.triangles[i + j]]);
                    Vector3 localPeelingShell = currPeelingShellMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(world);
                    currPeelingShellMesh.vertices[peelingMesh.triangles[i + j]] = localPeelingShell;
                }

                SnapToPreviousVertices(i);

                currPeelingShellMesh.uvs2ToClip[peelingMesh.triangles[i + 0]] = Vector2.up;
                currPeelingShellMesh.uvs2ToClip[peelingMesh.triangles[i + 1]] = Vector2.up;
                currPeelingShellMesh.uvs2ToClip[peelingMesh.triangles[i + 2]] = Vector2.up;

                peelingMesh.uvs2ToClip[peelingMesh.triangles[i + 0]] = Vector2.down;
                peelingMesh.uvs2ToClip[peelingMesh.triangles[i + 1]] = Vector2.down;
                peelingMesh.uvs2ToClip[peelingMesh.triangles[i + 2]] = Vector2.down;

                nextTime = Time.time + delay;
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
            currPeelingShellMesh.mesh.uv2 = currPeelingShellMesh.uvs2ToClip;
        }


        peelingMesh.mesh.uv2 = peelingMesh.uvs2ToClip;
    }

    public void SnapToPreviousVertices(int i)
    {
        if (LengthInsadeTriangleIndices >= 81)
        {
            insadeTriangleIndices.RemoveAt(0);
            insadeTriangleIndices.RemoveAt(0);
            insadeTriangleIndices.RemoveAt(0);
        }
        if (LengthInsadeTriangleIndices > 3)
        {
            Snap(i);
        }

        insadeTriangleIndices.Add(i + 0);
        insadeTriangleIndices.Add(i + 1);
        insadeTriangleIndices.Add(i + 2);
    }

    private void Snap(int i)
    {
        for (int currThreeIndicesIndex = i; currThreeIndicesIndex < i + 3; currThreeIndicesIndex++)
        {
            SnapSubMethod(currThreeIndicesIndex);
        }
    }

    public void SnapSubMethod(int lastThreeVertIndex)
    {
        int[] sameVertexIndices = peelingMesh.trianglesExtraDatas[lastThreeVertIndex].sameVertexIndices;
        for (int i = 0; i < sameVertexIndices.Length; i++)
        {
            for (int j = 0; j < LengthInsadeTriangleIndices; j++)
            {
                int vertIndexB = peelingMesh.triangles[insadeTriangleIndices[j]];
                if (sameVertexIndices[i] == vertIndexB)
                {
                    currPeelingShellMesh.vertices[lastThreeVertIndex] = currPeelingShellMesh.vertices[vertIndexB];
                }
            }
        }
    }
}