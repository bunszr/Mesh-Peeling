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
        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.transform.parent = peelingMesh.transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = Vector3.zero;
        shellMeshContainer.CurrShellMesh.transform.localRotation = Quaternion.identity;
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();

        shellMeshContainer.CurrShellMesh.rb.isKinematic = false;

        shellMeshContainer.Return(shellMeshContainer.CurrShellMesh);

        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.rb.isKinematic = true;
        shellMeshContainer.CurrShellMesh.transform.parent = peelingMesh.transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = Vector3.up;
        shellMeshContainer.CurrShellMesh.transform.localRotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!hasPeeling) return;
        // shellMeshContainer.CurrShellMesh.transform.SetLocalPosY(Mathf.PingPong(Time.time * speed, 1));
    }
}