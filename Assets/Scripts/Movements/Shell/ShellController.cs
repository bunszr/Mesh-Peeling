using System.Collections.Generic;
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
        CurrShellMesh = shellMeshContainer.Rent();
        SetTransformStuff();
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
        shellMeshRotationAngle = 0;
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();

        SetTransformStuff();
        CurrShellMesh.SetUvToClipValueNegative();
    }

    public void SetTransformStuff()
    {
        CurrShellMesh.transform.parent = transform;
        CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    private void Update()
    {
        if (!hasPeeling) return;

        shellMeshRotationAngle += (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(_knife.Velocity, ShellMeshRadius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
        CurrShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
        CurrShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.up);
    }
}