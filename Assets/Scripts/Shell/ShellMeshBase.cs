using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Jobs;

public abstract class ShellMeshBase : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    [HideInInspector] public Mesh mesh;

    [HideInInspector] public NativeArray<float3> vertices;
    [HideInInspector] public NativeArray<float3> normals;
    [HideInInspector] public NativeArray<float2> uvs;
    [HideInInspector] public NativeArray<float2> uvs2ToClip;
    [HideInInspector] public NativeArray<int> triangles;

    public void SetUvToClipValueNegative()
    {
        JobUvToClipSetter jobUvToClipSetter = new JobUvToClipSetter(this.uvs2ToClip, new float2(0, -1));
        jobUvToClipSetter.ScheduleParallel(vertices.Length, 2048, default).Complete();
        mesh.SetUVs(1, uvs2ToClip);
    }

    protected virtual void OnDestroy()
    {
        vertices.Dispose();
        normals.Dispose();
        uvs.Dispose();
        uvs2ToClip.Dispose();
        triangles.Dispose();
    }
}