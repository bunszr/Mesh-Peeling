using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public abstract class CutterBase : MonoBehaviour, ICutterUpdater
{
    protected IGetTriIndicesA _getTriIndicesA;

    LevelDataHolder levelDataHolder;
    public ShellMeshContainer shellMeshContainer => levelDataHolder.shellMeshContainer;
    public PeelingMesh peelingMesh => levelDataHolder.peelingMesh;
    public ShellMeshCollision shellMeshCollisionPrefab;
    [HideInInspector] public ShellControllerBase shellControllerBase;

    public System.Action onStartPeling;
    public System.Action onEndPeling;

    protected NativeQueue<int> peeledTriangleIndicesAtOnce; // From the beginning of peeeling until the release of shell
    protected Queue<int> lastPeeledTriIndicesNormalQueue = new Queue<int>();
    public int maxLastPeeledTriangleIndicesLength = 140;

    public float vertexOffset = 0;
    protected bool hasPeelingInFrame = false;

    public float PeelingTriangleIndexCount { get; protected set; }
    public bool HasCut => hasPeelingInFrame;

    protected virtual void Start()
    {
        levelDataHolder = GetComponentInParent<LevelDataHolder>();
        shellControllerBase = GetComponentInParent<ShellControllerBase>();

        peeledTriangleIndicesAtOnce = new NativeQueue<int>(Allocator.Persistent);
        _getTriIndicesA = GetComponent<IGetTriIndicesA>();
    }

    public void UpdateShellMesh()
    {
        shellControllerBase.CurrShellMesh.mesh.SetVertices(shellControllerBase.CurrShellMesh.vertices);
        shellControllerBase.CurrShellMesh.mesh.SetUVs(1, shellControllerBase.CurrShellMesh.uvs2ToClip);
        shellControllerBase.CurrShellMesh.mesh.SetNormals(shellControllerBase.CurrShellMesh.normals);
    }

    public void PeelMesh(NativeArray<int> travelingTriangleIndicesAArray, out int peeledTriIndicesCountInFrame)
    {
        NativeQueue<int> peelingTriIndicesInFrameNativeQueue = new NativeQueue<int>(Allocator.TempJob); // it is not working like the normal Queue<T>. See NativeQueue documentation
        JobMeshPeeler jobMeshPeeler = new JobMeshPeeler()
        {
            triIndicesA = travelingTriangleIndicesAArray,
            vertices = peelingMesh.vertices,
            uvs2ToClip = peelingMesh.uvs2ToClip,
            triangles = peelingMesh.triangles,
            shellVertices = shellControllerBase.CurrShellMesh.vertices,
            shellUvs2ToClip = shellControllerBase.CurrShellMesh.uvs2ToClip,
            peelingWorldToLocalMatrix = peelingMesh.transform.worldToLocalMatrix,
            peelingLocalToWorldMatrix = peelingMesh.transform.localToWorldMatrix,
            shellWorldToLocalMatrix = shellControllerBase.CurrShellMesh.transform.worldToLocalMatrix,
            cutterCenterPosition = transform.position,
            localP = peelingMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position),
            cutterSqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2),
            peelingTriIndicesQueue = peelingTriIndicesInFrameNativeQueue.AsParallelWriter(),
            peeledTriangleIndicesAtOnce = peeledTriangleIndicesAtOnce.AsParallelWriter(),
            vertexOffset = vertexOffset,
        };

        JobHandle jobHandleMeshPeeler = jobMeshPeeler.ScheduleParallel(travelingTriangleIndicesAArray.Length, 102, default);
        jobHandleMeshPeeler.Complete();

        peeledTriIndicesCountInFrame = peelingTriIndicesInFrameNativeQueue.Count;
        for (int i = 0; i < peeledTriIndicesCountInFrame; i++) lastPeeledTriIndicesNormalQueue.Enqueue(peelingTriIndicesInFrameNativeQueue.Dequeue());

        peelingTriIndicesInFrameNativeQueue.Dispose();
    }

    public void LimitLastPeeledTriIndicesLength(Queue<int> lastPeeledTriIndicesNormalQueue, int maxLastPeeledTriangleIndices)
    {
        int extraTriangleCount = lastPeeledTriIndicesNormalQueue.Count / 3 - maxLastPeeledTriangleIndices;
        if (extraTriangleCount > 0)
        {
            for (int i = 0; i < extraTriangleCount; i++)
            {
                lastPeeledTriIndicesNormalQueue.Dequeue();
                lastPeeledTriIndicesNormalQueue.Dequeue();
                lastPeeledTriIndicesNormalQueue.Dequeue();
            }
        }
    }

    protected void JobSnapVertices(Queue<int> lastPeeledTriIndicesNormalQueue)
    {
        NativeHashMap<int, bool> vertexKeyFromPeelingTriIndices = new NativeHashMap<int, bool>(lastPeeledTriIndicesNormalQueue.Count, Allocator.TempJob);
        NativeArray<int> array = new NativeArray<int>(lastPeeledTriIndicesNormalQueue.Count, Allocator.TempJob);
        for (int i = 0; i < array.Length; i++)
        {
            int value = lastPeeledTriIndicesNormalQueue.Dequeue();
            lastPeeledTriIndicesNormalQueue.Enqueue(value);
            array[i] = value;
            vertexKeyFromPeelingTriIndices.Add(shellControllerBase.CurrShellMesh.triangles[value], true);
        }

        JobVertexSnapper jobVertexSnapper = new JobVertexSnapper()
        {
            shellTriangles = shellControllerBase.CurrShellMesh.triangles,
            shellVertices = shellControllerBase.CurrShellMesh.vertices,
            shellUvs2ToClip = shellControllerBase.CurrShellMesh.uvs2ToClip,
            multiHashMapVertIndexToSameVerticesIndices = peelingMesh.multiHashMapVertIndexToSameVerticesIndices,
            peelingTriIndices = array,
            vertexKeyFromPeelingTriIndices = vertexKeyFromPeelingTriIndices,
        };

        JobHandle jobHandle = jobVertexSnapper.ScheduleParallel(array.Length, 9, default);
        jobHandle.Complete();

        array.Dispose();
        vertexKeyFromPeelingTriIndices.Dispose();
    }

    protected void CalculatePercentOfPeeling(int peeledTriIndicesCountInFrame)
    {
        PeelingTriangleIndexCount += peeledTriIndicesCountInFrame;
        peelingMesh.PercentOfPeeling = PeelingTriangleIndexCount / (float)peelingMesh.VertexOrTriangleIndicesCount;
    }

    protected void JobCalculatePeeledTriIndicesNormal(NativeQueue<int> peeledTriangleIndicesAtOnce)
    {
        NativeArray<int> peeledTriIndicesAtOnceArray = peeledTriangleIndicesAtOnce.ToArray(Allocator.TempJob);

        JobCalculateNormals jobCalculateNormals = new JobCalculateNormals()
        {
            shellVertices = shellControllerBase.CurrShellMesh.vertices,
            shellTriangles = shellControllerBase.CurrShellMesh.triangles,
            peeledTriIndicesAtOnceArray = peeledTriIndicesAtOnceArray,
            shellNormals = shellControllerBase.CurrShellMesh.normals,
        };

        JobHandle jobHandle = jobCalculateNormals.ScheduleParallel(peeledTriIndicesAtOnceArray.Length, 9, default);
        jobHandle.Complete();

        peeledTriIndicesAtOnceArray.Dispose();
    }

    public bool JobHasPeeling(NativeArray<int> travelingTriangleIndicesAArray)
    {
        NativeArray<bool> hasInsadeResult = new NativeArray<bool>(1, Allocator.TempJob);

        JobHasPeeling jobHasPeeling = new JobHasPeeling()
        {
            triIndicesA = travelingTriangleIndicesAArray,
            vertices = peelingMesh.vertices,
            uvs2ToClip = peelingMesh.uvs2ToClip,
            triangles = peelingMesh.triangles,
            localP = peelingMesh.transform.worldToLocalMatrix.MultiplyPoint3x4(transform.position),
            cutterSqrRadius = Mathf.Pow(transform.localScale.x * .5f, 2),
            hasInsadeResult = hasInsadeResult,
        };

        JobHandle jobHandleMeshPeeler = jobHasPeeling.ScheduleParallel(travelingTriangleIndicesAArray.Length, 102, default);
        jobHandleMeshPeeler.Complete();

        bool result = hasInsadeResult[0];
        hasInsadeResult.Dispose();
        return result;
    }


    public void GenerateShellMeshFromShellMesh(NativeQueue<int> peeledTriangleIndicesAtOnce)
    {
        if (peeledTriangleIndicesAtOnce.Count <= 3) return;

        int count = peeledTriangleIndicesAtOnce.Count * 3;
        NativeArray<float3> newMeshVertices = new NativeArray<float3>(count, Allocator.TempJob);
        NativeArray<float3> newMeshNormals = new NativeArray<float3>(count, Allocator.TempJob);
        NativeArray<float2> newMeshUVs = new NativeArray<float2>(count, Allocator.TempJob);
        NativeArray<int> newMeshTriangles = new NativeArray<int>(count, Allocator.TempJob);
        NativeArray<int> peeledTriIndicesAtOnceArray = peeledTriangleIndicesAtOnce.ToArray(Allocator.TempJob);

        JobGenerateMesh jobGenerateMesh = new JobGenerateMesh()
        {
            shellVertices = shellControllerBase.CurrShellMesh.vertices,
            shellNormals = shellControllerBase.CurrShellMesh.normals,
            peelingMeshUvs = peelingMesh.uvs,
            peelingMeshTriangles = peelingMesh.triangles,
            newMeshVertices = newMeshVertices,
            newMeshNormals = newMeshNormals,
            newMeshUvs = newMeshUVs,
            newMeshTriangles = newMeshTriangles,
            peeledTriIndicesAtOnceArray = peeledTriIndicesAtOnceArray,
        };

        jobGenerateMesh.ScheduleParallel(count / 3, 9, default).Complete();

        Mesh mesh = new Mesh();
        mesh.SetVertices(newMeshVertices);
        mesh.SetUVs(0, newMeshUVs);
        mesh.SetNormals(newMeshNormals);
        mesh.SetIndices(newMeshTriangles, MeshTopology.Triangles, 0, true);

        ShellMeshCollision shellMeshCollision = Instantiate(shellMeshCollisionPrefab, shellControllerBase.CurrShellMesh.transform.position, shellControllerBase.CurrShellMesh.transform.rotation);
        shellMeshCollision.meshFilter.mesh = mesh;
        shellMeshCollision.meshCollider.sharedMesh = mesh;
        shellMeshCollision.meshRenderer.SetPropertyBlock(peelingMesh.materialPropertyBlock);

        newMeshVertices.Dispose();
        newMeshNormals.Dispose();
        newMeshUVs.Dispose();
        newMeshTriangles.Dispose();
        peeledTriIndicesAtOnceArray.Dispose();
    }

    public virtual void CutterUpdate()
    {
    }
}