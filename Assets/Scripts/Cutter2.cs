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

    Queue<int> peelingTriIndicesNormalQueue = new Queue<int>();
    public int maxPeelingTriangle = 70;

    private void Start()
    {
        currShellMesh = Instantiate<PeelingShellMesh>(shellMeshPrefab, shellCenterT.transform.position, shellCenterT.transform.rotation, transform.parent);
        currShellMesh.tri();
    }

    private void Update()
    {
        bool hasPeelingInFrame = false;

        NativeQueue<int> peelingTriIndicesNativeQueue = new NativeQueue<int>(Allocator.TempJob); // it is not working like the normal Queue<T>. See NativeQueue documentation
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
            peelingTriIndicesQueue = peelingTriIndicesNativeQueue.AsParallelWriter(),
            hasInsadeResult = hasInsadeResult,
        };

        JobHandle jobHandleMeshPeeler = jobMeshPeeler.ScheduleParallel(peelingMesh.triangles.Length / 3, 102, default);
        jobHandleMeshPeeler.Complete();

        int count = peelingTriIndicesNativeQueue.Count;
        for (int i = 0; i < count; i++) peelingTriIndicesNormalQueue.Enqueue(peelingTriIndicesNativeQueue.Dequeue());
        peelingTriIndicesNativeQueue.Dispose();


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
    }

    void LimitPeelingTriIndicesLength()
    {
        int extraTriangleCount = peelingTriIndicesNormalQueue.Count / 3 - maxPeelingTriangle;
        if (extraTriangleCount > 0)
        {
            for (int i = 0; i < extraTriangleCount; i++)
            {
                peelingTriIndicesNormalQueue.Dequeue();
                peelingTriIndicesNormalQueue.Dequeue();
                peelingTriIndicesNormalQueue.Dequeue();
            }
        }
        // Debug.Log("peelingTriIndices Length = " + peelingTriIndices.Length);
    }

    void RunJobVertexSnapper(JobHandle jobHandleMeshPeeler)
    {
        // NativeArray<int> array = peelingTriIndicesNormalQueue.ToArray(Allocator.TempJob);
        NativeArray<int> array = new NativeArray<int>(peelingTriIndicesNormalQueue.Count, Allocator.TempJob);
        for (int i = 0; i < array.Length; i++)
        {
            int value = peelingTriIndicesNormalQueue.Dequeue();
            peelingTriIndicesNormalQueue.Enqueue(value);
            array[i] = value;
        }

        JobVertexSnapper jobVertexSnapper = new JobVertexSnapper()
        {
            shellTriangles = currShellMesh.triangles,
            shellVertices = currShellMesh.vertices,
            shellUvs2ToClip = currShellMesh.uvs2ToClip,
            multiHashMapVertIndexToSameVerticesIndices = peelingMesh.multiHashMapVertIndexToSameVerticesIndices,
            peelingTriIndices = array,
        };

        JobHandle jobHandle = jobVertexSnapper.ScheduleParallel(array.Length, 9, jobHandleMeshPeeler);
        // jobVertexSnapper.Schedule(peelingTriIndices.Length, default).Complete();
        jobHandle.Complete();

        array.Dispose();
    }
}