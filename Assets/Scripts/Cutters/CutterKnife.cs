using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CutterKnife : CutterBase
{
    float nextTime;
    bool hasPeelStart = false;
    bool hasPressed = false;
    public float delay = .05f;

    protected override void Start()
    {
        base.Start();
    }

    public override void CutterUpdate()
    {
        if (Input.GetMouseButtonDown(0)) hasPressed = true;
        if (!hasPressed) return;

        NativeArray<int> travelingTriangleIndicesAArray = _getTriIndicesA.GetIndices(this);

        hasPeelingInFrame = JobHasPeeling(travelingTriangleIndicesAArray);

        if (hasPeelingInFrame)
        {
            if (!hasPeelStart)
            {
                hasPeelStart = true;
                onStartPeling?.Invoke();
                Debug.Log("Start");
            }
            nextTime = Time.time + delay;
        }

        if (hasPeelStart)
        {
            PeelMesh(travelingTriangleIndicesAArray, out int peeledTriIndicesCountInFrame);
            CalculatePercentOfPeeling(peeledTriIndicesCountInFrame);
            LimitLastPeeledTriIndicesLength(lastPeeledTriIndicesNormalQueue, maxLastPeeledTriangleIndicesLength);
            JobSnapVertices(lastPeeledTriIndicesNormalQueue);
            JobCalculatePeeledTriIndicesNormal(peeledTriangleIndicesAtOnce);

            UpdateShellMesh();
        }

        if (hasPeelStart)
        {
            if (Time.time > nextTime && !hasPeelingInFrame)
            {
                PeelEnded();
                hasPeelStart = false;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            hasPressed = false;
            if (hasPeelStart) PeelEnded();
        }

        travelingTriangleIndicesAArray.Dispose();
    }

    void PeelEnded()
    {
        GenerateShellMeshFromShellMesh(peeledTriangleIndicesAtOnce);
        peeledTriangleIndicesAtOnce.Clear();
        onEndPeling?.Invoke();
        Debug.Log("PeelEnded");
    }
}