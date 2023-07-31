using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

[BurstCompile]
public struct JobUvToClipSetter : IJobFor
{
    [WriteOnly] public NativeArray<float2> uvs2ToClip;
    [ReadOnly] public float2 value;

    public JobUvToClipSetter(NativeArray<float2> uvs2ToClip, float2 value)
    {
        this.uvs2ToClip = uvs2ToClip;
        this.value = value;
    }

    public void Execute(int index)
    {
        uvs2ToClip[index] = value;
    }
}