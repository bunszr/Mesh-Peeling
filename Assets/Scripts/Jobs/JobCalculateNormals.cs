using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct JobCalculateNormals : IJobFor
{
    [ReadOnly] public NativeArray<float3> shellVertices;
    [ReadOnly] public NativeArray<int> shellTriangles;
    [ReadOnly] public NativeArray<int> peeledTriIndicesAtOnceArray;
    [WriteOnly, NativeDisableParallelForRestriction] public NativeArray<float3> shellNormals;

    public void Execute(int index)
    {
        int triIndexA = peeledTriIndicesAtOnceArray[index];

        float3 normal = math.normalizesafe(math.cross(shellVertices[shellTriangles[triIndexA + 1]] - shellVertices[shellTriangles[triIndexA]], shellVertices[shellTriangles[triIndexA + 2]] - shellVertices[shellTriangles[triIndexA]]));
        // float3 normal = math.up();
        shellNormals[shellTriangles[triIndexA + 0]] = normal;
        shellNormals[shellTriangles[triIndexA + 1]] = normal;
        shellNormals[shellTriangles[triIndexA + 2]] = normal;
    }
}