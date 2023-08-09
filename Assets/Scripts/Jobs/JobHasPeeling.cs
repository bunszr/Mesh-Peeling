using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct JobHasPeeling : IJobFor
{
    [ReadOnly] public NativeArray<int> triIndicesA;
    [ReadOnly] public NativeArray<float3> vertices;
    [ReadOnly] public NativeArray<float2> uvs2ToClip;
    [ReadOnly] public NativeArray<int> triangles;

    [ReadOnly] public float3 localP;
    [ReadOnly] public float cutterSqrRadius;

    [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<bool> hasInsadeResult;

    public void Execute(int index)
    {
        if (uvs2ToClip[triangles[triIndicesA[index] + 0]].y < 0) return;

        bool hasInsade = math.distancesq(localP, vertices[triangles[triIndicesA[index] + 0]]) < cutterSqrRadius &&
                         math.distancesq(localP, vertices[triangles[triIndicesA[index] + 1]]) < cutterSqrRadius &&
                         math.distancesq(localP, vertices[triangles[triIndicesA[index] + 2]]) < cutterSqrRadius;

        if (hasInsade) hasInsadeResult[0] = true;
    }
}