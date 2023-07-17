using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Cutter : MonoBehaviour
{
    public PeelingMesh peelingMesh;
    public PeelingShellMesh shellMeshPrefab;
    public PeelingShellMesh currPeelingShellMesh;
    public Transform shellCenterT;

    public float delay = .2f;
    float nextTime;

    State state;
    enum State
    {
        None, Start, End
    }

    Vector3[] enterLocalPoints;

    private void Start()
    {
        enterLocalPoints = new Vector3[peelingMesh.vertices.Length];
    }

    private void Update()
    {
        // bool oldHasPeeling = hasPeeling;
        bool hasPeelingInFrame = false;
        Vector3 localP = peelingMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position);
        float sqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2);
        for (int i = 0; i < peelingMesh.triangles.Length; i += 3)
        {
            if (peelingMesh.triangles[i + 0] == 0 && peelingMesh.triangles[i + 1] == 0 && peelingMesh.triangles[i + 2] == 0)
            {
                if (currPeelingShellMesh != null)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Vector3 world = shellCenterT.localToWorldMatrix.MultiplyPoint3x4(enterLocalPoints[currPeelingShellMesh.triangles[i + j]]);
                        Vector3 local = currPeelingShellMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(world);
                        currPeelingShellMesh.vertices[currPeelingShellMesh.triangles[i + j]] = local;
                    }
                }
                continue;
            }

            Vector3 mid = (peelingMesh.vertices[peelingMesh.triangles[i]] + peelingMesh.vertices[peelingMesh.triangles[i + 1]] + peelingMesh.vertices[peelingMesh.triangles[i + 2]]) / 3f;

            float sqrDst = Vector3.SqrMagnitude(localP - mid);

            if (sqrDst < sqrRadius)
            {
                hasPeelingInFrame = true;

                if (state == State.None || state == State.End)
                {
                    state = State.Start;
                    currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, peelingMesh.transform.position, peelingMesh.transform.rotation, peelingMesh.transform);
                    Debug.Log("Start");
                }

                for (int j = 0; j < 3; j++)
                {
                    int index = peelingMesh.triangles[i + j];
                    Vector3 world = peelingMesh.transform.localToWorldMatrix.MultiplyPoint3x4(currPeelingShellMesh.vertices[index]);
                    Vector3 localShellCenterT = shellCenterT.worldToLocalMatrix.MultiplyPoint3x4(world);
                    enterLocalPoints[index] = localShellCenterT;

                    currPeelingShellMesh.uvs2ToClip[index].y = 1;
                }

                nextTime = Time.time + delay;


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
                currPeelingShellMesh = null;
                Debug.Log("end");
            }
        }

        if (currPeelingShellMesh != null)
        {
            currPeelingShellMesh.mesh.vertices = currPeelingShellMesh.vertices;
            currPeelingShellMesh.mesh.uv2 = currPeelingShellMesh.uvs2ToClip;
        }


        peelingMesh.mesh.triangles = peelingMesh.triangles;
    }

    [Button]
    public void Ins()
    {
        currPeelingShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, peelingMesh.transform.position, peelingMesh.transform.rotation, peelingMesh.transform);
    }
}