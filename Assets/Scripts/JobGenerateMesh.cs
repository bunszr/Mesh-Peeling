using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct JobGenerateMesh : IJobFor
{
    [ReadOnly] public NativeArray<float3> shellVertices;
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
        float3 normal = math.normalizesafe(math.cross(shellVertices[peelingMeshTriangles[triIndex + 1]] - shellVertices[peelingMeshTriangles[triIndex + 0]], shellVertices[peelingMeshTriangles[triIndex + 2]] - shellVertices[peelingMeshTriangles[triIndex + 0]]));

        for (int j = 0; j < 3; j++)
        {
            int newIndex = index * 3 + j;
            newMeshVertices[newIndex] = shellVertices[peelingMeshTriangles[triIndex + j]];
            newMeshUvs[newIndex] = peelingMeshUvs[peelingMeshTriangles[triIndex + j]];
            newMeshNormals[newIndex] = normal;
            newMeshTriangles[newIndex] = newIndex;
        }
    }
}







// using Unity.Collections;
// using Unity.Mathematics;
// using Unity.Burst;
// using Unity.Jobs;

// [BurstCompile]
// public struct JobGenerateMesh : IJobFor
// {
//     [ReadOnly] public NativeArray<float3> shellVertices;
//     [ReadOnly] public NativeArray<float2> shellUvs;
//     [ReadOnly] public NativeArray<int> shellTriangles;

//     [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> newMeshVertices;
//     [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float2> newMeshUVs;
//     [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<int> newMeshTriangles;

//     [NativeDisableParallelForRestriction] public NativeQueue<int> peeledTriIndicesAtOnce;

//     public void Execute(int invalidIndex)
//     {
//         int triIndex = peeledTriIndicesAtOnce.Dequeue();

//         for (int j = 0; j < 3; j++)
//         {
//             int a = invalidIndex * 3 + j;
//             newMeshVertices[a] = shellVertices[shellTriangles[triIndex + j]];
//             newMeshUVs[a] = shellUvs[shellTriangles[triIndex + j]];
//             newMeshTriangles[a] = a;

//             // newMeshVertices[shellTriangles[triIndex + j]] = shellVertices[shellTriangles[triIndex + j]];
//             // newMeshUVs[shellTriangles[triIndex + j]] = shellUvs[shellTriangles[triIndex + j]];
//             // newMeshTriangles[triIndex + j] = shellTriangles[triIndex + j];
//         }
//     }
// }