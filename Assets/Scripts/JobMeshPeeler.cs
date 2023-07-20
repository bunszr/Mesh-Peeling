using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using UnityEngine;

[BurstCompile]
public struct JobMeshPeeler : IJobFor
{
    // For peeling mesh
    [ReadOnly, NativeDisableParallelForRestriction] public NativeArray<float3> vertices;
    public NativeArray<float2> uvs;
    [NativeDisableParallelForRestriction] public NativeArray<float2> uvs2ToClip;
    [ReadOnly] public NativeArray<int> triangles;

    // For shell peeling mesh
    [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<float3> shellVertices;
    [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<float2> shellUvs2ToClip;

    [ReadOnly] public float4x4 peelingWorldToLocalMatrix;
    [ReadOnly] public float4x4 peelingLocalToWorldMatrix;
    [ReadOnly] public float4x4 shellWorldToLocalMatrix;

    [ReadOnly] public float3 cutterCenterPosition;
    [ReadOnly] public float cutterSqrRadius;

    [NativeDisableParallelForRestriction, WriteOnly] public NativeArray<bool> hasInsadeResult;

    public void Execute(int index)
    {
        int triIndexA = index * 3 + 0;
        int triIndexB = index * 3 + 1;
        int triIndexC = index * 3 + 2;

        if (uvs2ToClip[triangles[triIndexA]].y < 0) return;

        float3 localP = math.mul(peelingWorldToLocalMatrix, math.float4(cutterCenterPosition, 1)).xyz;
        bool hasInsade = math.distancesq(localP, vertices[triangles[triIndexA]]) < cutterSqrRadius &&
                         math.distancesq(localP, vertices[triangles[triIndexB]]) < cutterSqrRadius &&
                         math.distancesq(localP, vertices[triangles[triIndexC]]) < cutterSqrRadius;

        if (!hasInsade) return;

        for (int j = 0; j < 3; j++)
        {
            float3 world = math.mul(peelingLocalToWorldMatrix, math.float4(vertices[triangles[triIndexA + j]], 1)).xyz;
            float3 localPeelingShell = math.mul(shellWorldToLocalMatrix, math.float4(world, 1)).xyz;
            shellVertices[triangles[triIndexA + j]] = localPeelingShell;
        }

        shellUvs2ToClip[triangles[triIndexA]] = new float2(0, 1);
        shellUvs2ToClip[triangles[triIndexB]] = new float2(0, 1);
        shellUvs2ToClip[triangles[triIndexC]] = new float2(0, 1);

        uvs2ToClip[triangles[triIndexA]] = new float2(0, -1);
        uvs2ToClip[triangles[triIndexB]] = new float2(0, -1);
        uvs2ToClip[triangles[triIndexC]] = new float2(0, -1);

        hasInsadeResult[0] = true;
    }
}