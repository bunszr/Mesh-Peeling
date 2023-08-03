using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct JobDataSetter : IJobFor
{
    [ReadOnly] public NativeArray<float3> vertices;
    [ReadOnly] public NativeArray<float2> uvs;
    [ReadOnly] public NativeArray<float2> uvs2ToClip;
    [ReadOnly] public NativeArray<int> triangles;

    [WriteOnly] public NativeArray<float3> targetVertices;
    [WriteOnly] public NativeArray<float2> targetUvs;
    [WriteOnly] public NativeArray<float2> targetUvs2ToClip;
    [WriteOnly] public NativeArray<int> targetTriangles;

    public void Execute(int index)
    {
        targetVertices[index] = vertices[index];
        targetUvs[index] = uvs[index];
        targetUvs2ToClip[index] = uvs2ToClip[index];
        targetTriangles[index] = triangles[index];
    }
}