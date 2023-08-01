using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Cutter2 : CutterBase
{
    public PeelingMesh peelingMesh;
    public Transform shellCenterT;


    public float delay = .2f;
    float nextTime;

    State state;
    enum State
    {
        None, Start, End
    }

    Queue<int> peelingTriIndicesNormalQueue = new Queue<int>();
    public int maxPeelingTriangle = 70;

    public float PeelingTriangleIndexCount { get; set; }

    private void Update()
    {
        bool hasPeelingInFrame = false;

        NativeQueue<int> peelingTriIndicesNativeQueue = new NativeQueue<int>(Allocator.TempJob); // it is not working like the normal Queue<T>. See NativeQueue documentation
        NativeArray<bool> hasInsadeResult = new NativeArray<bool>(1, Allocator.TempJob);

        JobMeshPeeler jobMeshPeeler = new JobMeshPeeler()
        {
            vertices = peelingMesh.vertices,
            uvs2ToClip = peelingMesh.uvs2ToClip,
            triangles = peelingMesh.triangles,
            shellVertices = shellMeshContainer.CurrShellMesh.vertices,
            shellUvs2ToClip = shellMeshContainer.CurrShellMesh.uvs2ToClip,
            peelingWorldToLocalMatrix = peelingMesh.transform.worldToLocalMatrix,
            peelingLocalToWorldMatrix = peelingMesh.transform.localToWorldMatrix,
            shellWorldToLocalMatrix = shellMeshContainer.CurrShellMesh.transform.worldToLocalMatrix,
            cutterCenterPosition = transform.position,
            cutterSqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2),
            peelingTriIndicesQueue = peelingTriIndicesNativeQueue.AsParallelWriter(),
            hasInsadeResult = hasInsadeResult,
        };

        JobHandle jobHandleMeshPeeler = jobMeshPeeler.ScheduleParallel(peelingMesh.triangles.Length / 3, 102, default);
        jobHandleMeshPeeler.Complete();

        int peelingTriIndicesNativeQueueCount = peelingTriIndicesNativeQueue.Count;
        for (int i = 0; i < peelingTriIndicesNativeQueueCount; i++) peelingTriIndicesNormalQueue.Enqueue(peelingTriIndicesNativeQueue.Dequeue());
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

            CalculatePercentOfPeeling(peelingTriIndicesNativeQueueCount);

            LimitPeelingTriIndicesLength();
            RunJobVertexSnapper(jobHandleMeshPeeler);

            if (shellMeshContainer.CurrShellMesh != null)
            {
                shellMeshContainer.CurrShellMesh.mesh.SetVertices(shellMeshContainer.CurrShellMesh.vertices);
                shellMeshContainer.CurrShellMesh.mesh.SetUVs(1, shellMeshContainer.CurrShellMesh.uvs2ToClip);
            }
            peelingMesh.mesh.SetUVs(1, peelingMesh.uvs2ToClip);

            nextTime = Time.time + delay;
        }

        if (Time.time > nextTime)
        {
            if (state == State.Start && !hasPeelingInFrame)
            {
                state = State.End;
                onEndPeling?.Invoke();
                Debug.Log("end");
            }
        }

        hasInsadeResult.Dispose();
    }

    private void OnDisable()
    {
    }

    void CalculatePercentOfPeeling(int peelingTriIndicesCount)
    {
        PeelingTriangleIndexCount += peelingTriIndicesCount;
        peelingMesh.PercentOfPeeling = PeelingTriangleIndexCount / (float)peelingMesh.VertexOrTriangleIndicesCount;
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
            shellTriangles = shellMeshContainer.CurrShellMesh.triangles,
            shellVertices = shellMeshContainer.CurrShellMesh.vertices,
            shellUvs2ToClip = shellMeshContainer.CurrShellMesh.uvs2ToClip,
            multiHashMapVertIndexToSameVerticesIndices = peelingMesh.multiHashMapVertIndexToSameVerticesIndices,
            peelingTriIndices = array,
        };

        JobHandle jobHandle = jobVertexSnapper.ScheduleParallel(array.Length, 9, jobHandleMeshPeeler);
        // jobVertexSnapper.Schedule(peelingTriIndices.Length, default).Complete();
        jobHandle.Complete();

        array.Dispose();
    }
}