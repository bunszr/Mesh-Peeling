using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CutterWithTime : CutterBase
{
    float nextTime;
    bool hasPeelStart = false;
    bool hasPeelingInFrame = false;
    public float delay = .05f;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        NativeArray<int> travelingTriangleIndicesAArray = _getTriIndicesA.GetIndices(this);

        hasPeelingInFrame = JobHasPeeling(travelingTriangleIndicesAArray);

        if (hasPeelingInFrame && !hasPeelStart)
        {
            hasPeelStart = true;
            onStartPeling?.Invoke();
            Debug.Log("Start");
            nextTime = Time.time + delay;
        }

        if (hasPeelStart)
        {
            PeelMesh(travelingTriangleIndicesAArray, out int peelingTriIndicesNativeQueueCount);
            CalculatePercentOfPeeling(peelingTriIndicesNativeQueueCount);
            LimitLastPeeledTriIndicesLength(lastPeeledTriIndicesNormalQueue, maxLastPeeledTriangleIndicesLength);
            JobSnapVertices(lastPeeledTriIndicesNormalQueue);
            JobCalculatePeeledTriIndicesNormal(peeledTriangleIndicesAtOnce);

            UpdateMesh();
        }

        if (hasPeelStart)
        {
            if (Time.time > nextTime && !hasPeelingInFrame)
            {
                GenerateShellMeshFromShellMesh(peeledTriangleIndicesAtOnce);
                peeledTriangleIndicesAtOnce.Clear();
                onEndPeling?.Invoke();
                Debug.Log("PeelEnded");
                hasPeelStart = false;
            }
        }

        travelingTriangleIndicesAArray.Dispose();
    }
}