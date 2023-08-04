using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellControllerBase : MonoBehaviour
{
    LevelDataHolder levelDataHolder;
    public CutterBase cutterBase => levelDataHolder.cutterBase;
    public IKnife _knife => levelDataHolder._knife;
    public Rotater rotater => levelDataHolder.rotater;
    public ShellMeshContainer shellMeshContainer => levelDataHolder.shellMeshContainer;

    public ShellMeshBase CurrShellMesh;

    protected bool hasPeeling;

    public float ShellMeshRadius => CurrShellMesh.transform.localPosition.magnitude;

    protected virtual void Start()
    {
        levelDataHolder = GetComponentInParent<LevelDataHolder>();
        cutterBase.onStartPeling += OnStartPeeling;
        cutterBase.onEndPeling += OnEndPeeling;
    }

    protected virtual void OnStartPeeling()
    {
        hasPeeling = true;
    }

    protected virtual void OnEndPeeling()
    {
        hasPeeling = false;
    }

    public float GetAngleSpeedFromAngleSpeedOfRotater()
    {
        float radius = CurrShellMesh.transform.localPosition.magnitude;
        return Mathf.Abs(rotater.angleSpeed * (rotater.radius / radius));
    }
}