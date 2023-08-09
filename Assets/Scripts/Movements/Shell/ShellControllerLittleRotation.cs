using System.Collections.Generic;
using UnityEngine;

public class ShellControllerLittleRotation : ShellControllerBase
{
    public Transform shellCenterT;
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
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();

        SetTransformStuff();
        CurrShellMesh.SetUvToClipValueNegative();
    }

    public void SetTransformStuff()
    {
        CurrShellMesh.transform.parent = null;
        CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    private void Update()
    {
        if (!hasPeeling) return;

        float angle = (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(_knife.Velocity, ShellMeshRadius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
        CurrShellMesh.transform.rotation *= Quaternion.AngleAxis(angle, Vector3.up);
    }
}