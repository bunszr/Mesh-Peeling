using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Cutter2 : MonoBehaviour
{
    public PeelingMesh peelingMesh;
    public PeelingShellMesh shellMeshPrefab;
    public PeelingShellMesh currShellMesh;
    public CubeBounds cubeBounds;
    public Transform shellCenterT;

    public float delay = .2f;
    float nextTime;

    State state;
    enum State
    {
        None, Start, End
    }

    public event System.Action onStartPeling;
    public event System.Action onEndPeling;

    NativeList<int> peelingTriIndices;
    public int maxPeelingTriangle = 70;

    private void Start()
    {
        currShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, shellCenterT.transform.position, shellCenterT.transform.rotation, transform.parent);
        currShellMesh.tri();
        peelingTriIndices = new NativeList<int>(Allocator.Persistent);
    }

    private void Update()
    {
        bool hasPeelingInFrame = false;

        NativeArray<bool> hasInsadeResult = new NativeArray<bool>(1, Allocator.TempJob);

        JobMeshPeeler jobMeshPeeler = new JobMeshPeeler()
        {
            vertices = peelingMesh.vertices,
            uvs = peelingMesh.uvs,
            uvs2ToClip = peelingMesh.uvs2ToClip,
            triangles = peelingMesh.triangles,
            shellVertices = currShellMesh.vertices,
            shellUvs2ToClip = currShellMesh.uvs2ToClip,
            peelingWorldToLocalMatrix = peelingMesh.transform.worldToLocalMatrix,
            peelingLocalToWorldMatrix = peelingMesh.transform.localToWorldMatrix,
            shellWorldToLocalMatrix = currShellMesh.transform.worldToLocalMatrix,
            cutterCenterPosition = transform.position,
            cutterSqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2),
            peelingTriIndices = peelingTriIndices,
            hasInsadeResult = hasInsadeResult,
        };

        JobHandle jobHandleMeshPeeler = jobMeshPeeler.ScheduleParallel(peelingMesh.triangles.Length / 3, 102, default);
        jobHandleMeshPeeler.Complete();


        hasPeelingInFrame = hasInsadeResult[0];
        if (hasPeelingInFrame)
        {
            if (state == State.None || state == State.End)
            {
                state = State.Start;
                onStartPeling?.Invoke();
                Debug.Log("Start");
            }

            LimitPeelingTriIndicesLength();
            RunJobVertexSnapper(jobHandleMeshPeeler);

            if (currShellMesh != null)
            {
                currShellMesh.mesh.SetVertices(currShellMesh.vertices);
                currShellMesh.mesh.SetUVs(1, currShellMesh.uvs2ToClip);
            }
            peelingMesh.mesh.SetUVs(1, peelingMesh.uvs2ToClip);

            nextTime = Time.time + delay;
        }

        if (Time.time > nextTime)
        {
            if (state == State.Start && !hasPeelingInFrame)
            {
                state = State.End;
                currShellMesh.Throw(transform.forward + transform.up);
                currShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, shellCenterT.transform.position, shellCenterT.transform.rotation, transform.parent);
                currShellMesh.tri();
                onEndPeling?.Invoke();
                Debug.Log("end");
            }
        }

        hasInsadeResult.Dispose();
    }

    private void OnDisable()
    {
        peelingTriIndices.Dispose();
    }

    void LimitPeelingTriIndicesLength()
    {
        int extraTriangleCount = peelingTriIndices.Length / 3 - maxPeelingTriangle;
        if (extraTriangleCount > 0)
        {
            for (int i = 0; i < extraTriangleCount; i++)
            {
                peelingTriIndices.RemoveAt(0);
                peelingTriIndices.RemoveAt(0);
                peelingTriIndices.RemoveAt(0);
            }
        }
        // Debug.Log("peelingTriIndices Length = " + peelingTriIndices.Length);
    }

    void RunJobVertexSnapper(JobHandle jobHandleMeshPeeler)
    {
        JobVertexSnapper jobVertexSnapper = new JobVertexSnapper()
        {
            shellTriangles = currShellMesh.triangles,
            shellVertices = currShellMesh.vertices,
            shellUvs2ToClip = currShellMesh.uvs2ToClip,
            multiHashMapVertIndexToSameVerticesIndices = peelingMesh.multiHashMapVertIndexToSameVerticesIndices,
            peelingTriIndices = peelingTriIndices,
        };

        JobHandle jobHandle = jobVertexSnapper.ScheduleParallel(peelingTriIndices.Length, 9, jobHandleMeshPeeler);
        // jobVertexSnapper.Schedule(peelingTriIndices.Length, default).Complete();
        jobHandle.Complete();
    }
}