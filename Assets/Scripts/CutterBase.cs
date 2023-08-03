using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public abstract class CutterBase : MonoBehaviour
{
    LevelDataHolder levelDataHolder;
    public ShellMeshContainer shellMeshContainer => levelDataHolder.shellMeshContainer;
    public PeelingMesh peelingMesh => levelDataHolder.peelingMesh;
    public ShellMeshCollision shellMeshCollisionPrefab;

    public System.Action onStartPeling;
    public System.Action onEndPeling;

    protected virtual void Start()
    {
        levelDataHolder = GetComponentInParent<LevelDataHolder>();
    }
}