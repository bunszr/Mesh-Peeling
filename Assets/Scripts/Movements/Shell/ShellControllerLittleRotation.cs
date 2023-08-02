using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellControllerLittleRotation : ShellControllerBase
{
    public Transform shellCenterT;
    public float shellAngleSpeedAdder = 5;

    protected override void Start()
    {
        base.Start();
        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.transform.parent = null;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
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
        shellMeshContainer.CurrShellMesh.transform.parent = null;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    private void Update()
    {
        if (!hasPeeling) return;

        float angle = (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(_knife.velocity, ShellMeshRadius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
        shellMeshContainer.CurrShellMesh.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
    }
}