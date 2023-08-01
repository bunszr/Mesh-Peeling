using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellControllerBase : MonoBehaviour
{
    public CutterBase cutterBase;
    public Knife knife;
    public Rotater rotater;
    public ShellMeshContainer shellMeshContainer;
    protected bool hasPeeling;

    public float ShellMeshRadius => shellMeshContainer.CurrShellMesh.transform.localPosition.magnitude;

    protected virtual void Start()
    {
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
        float radius = shellMeshContainer.CurrShellMesh.transform.localPosition.magnitude;
        return Mathf.Abs(rotater.angleSpeed * (rotater.radius / radius));
    }
}