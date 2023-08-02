using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

[BurstCompile]
public struct JobVertexSnapper : IJobFor
{
    [ReadOnly] public NativeArray<int> shellTriangles;
    [ReadOnly] public NativeArray<float2> shellUvs2ToClip;
    [ReadOnly] public NativeMultiHashMap<int, int> multiHashMapVertIndexToSameVerticesIndices;
    [ReadOnly] public NativeArray<int> peelingTriIndices;
    [ReadOnly] public NativeHashMap<int, bool> vertexKeyFromPeelingTriIndices;
    [NativeDisableParallelForRestriction] public NativeArray<float3> shellVertices;

    public void Execute(int index)
    {
        int vertIndex = shellTriangles[peelingTriIndices[index]];
        NativeMultiHashMapIterator<int> it;
        if (multiHashMapVertIndexToSameVerticesIndices.TryGetFirstValue(vertIndex, out int valueSameVertexIndex, out it))
        {
            Snap(vertIndex, valueSameVertexIndex);
            while (multiHashMapVertIndexToSameVerticesIndices.TryGetNextValue(out valueSameVertexIndex, ref it))
            {
                Snap(vertIndex, valueSameVertexIndex);
            }
        }
    }

    public void Snap(int vertIndex, int valueSameVertexIndex)
    {
        if (shellUvs2ToClip[valueSameVertexIndex].y > 0 && vertexKeyFromPeelingTriIndices.ContainsKey(valueSameVertexIndex))
        {
            shellVertices[valueSameVertexIndex] = shellVertices[vertIndex];
        }
    }
}