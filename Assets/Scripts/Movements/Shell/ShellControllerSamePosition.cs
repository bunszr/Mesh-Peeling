using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellControllerSamePosition : ShellControllerBase
{
    PeelingMesh peelingMesh;

    protected override void Start()
    {
        base.Start();
        peelingMesh = GetComponentInParent<LevelDataHolder>().peelingMesh;
        CurrShellMesh = shellMeshContainer.Rent();
        CurrShellMesh.transform.parent = peelingMesh.transform;
        CurrShellMesh.transform.localPosition = Vector3.zero;
        CurrShellMesh.transform.localRotation = Quaternion.identity;
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();

        CurrShellMesh.transform.parent = peelingMesh.transform;
        CurrShellMesh.transform.localPosition = Vector3.up;
        CurrShellMesh.transform.localRotation = Quaternion.identity;
        CurrShellMesh.SetUvToClipValueNegative();
    }

    private void Update()
    {
        if (!hasPeeling) return;
        // CurrShellMesh.transform.SetLocalPosY(Mathf.PingPong(Time.time * speed, 1));
    }
}