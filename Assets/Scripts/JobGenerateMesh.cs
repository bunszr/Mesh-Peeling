using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct JobGenerateMesh : IJobFor
{
    [ReadOnly] public NativeArray<float3> shellVertices;
    [ReadOnly] public NativeArray<float3> shellNormals;
    [ReadOnly] public NativeArray<float2> peelingMeshUvs;
    [ReadOnly] public NativeArray<int> peelingMeshTriangles;

    [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> newMeshVertices;
    [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> newMeshNormals;
    [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float2> newMeshUvs;
    [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<int> newMeshTriangles;

    [ReadOnly] public NativeArray<int> peeledTriIndicesAtOnceArray;

    public void Execute(int index)
    {
        int triIndex = peeledTriIndicesAtOnceArray[index];

        for (int j = 0; j < 3; j++)
        {
            int newIndex = index * 3 + j;
            newMeshVertices[newIndex] = shellVertices[peelingMeshTriangles[triIndex + j]];
            newMeshUvs[newIndex] = peelingMeshUvs[peelingMeshTriangles[triIndex + j]];
            newMeshNormals[newIndex] = shellNormals[peelingMeshTriangles[triIndex + j]];
            newMeshTriangles[newIndex] = newIndex;
        }
    }
}