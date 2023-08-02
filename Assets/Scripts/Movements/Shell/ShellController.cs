using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellController : ShellControllerBase
{
    public Transform shellCenterT;
    protected float shellMeshRotationAngle = 0;
    public float shellCenterSpeed = .04f;
    public float shellAngleSpeedAdder = 5;

    protected override void Start()
    {
        base.Start();
        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.transform.parent = transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
        shellMeshRotationAngle = 0;
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();

        shellMeshContainer.CurrShellMesh.rb.isKinematic = false;

        shellMeshContainer.Return(shellMeshContainer.CurrShellMesh);

        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.rb.isKinematic = true;
        shellMeshContainer.CurrShellMesh.transform.parent = transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    private void Update()
    {
        if (!hasPeeling) return;

        shellMeshRotationAngle += (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(_knife.velocity, ShellMeshRadius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
        shellMeshContainer.CurrShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
        shellMeshContainer.CurrShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.up);
    }
}