using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class ShellControllerForSpatula : ShellControllerBase
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

        shellMeshRotationAngle += Time.deltaTime * (shellAngleSpeedAdder + GetAngleSpeedFromSpeed(_knife.Velocity));
        CurrShellMesh.transform.localPosition += Vector3.up * shellCenterSpeed * Time.deltaTime;
        CurrShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.right);
    }

    public float GetAngleSpeedFromSpeed(float speed)
    {
        float radius = CurrShellMesh.transform.localPosition.magnitude;
        float circumferenceOfCircle = 2 * Mathf.PI * radius;
        return speed * 360f / circumferenceOfCircle;
    }
}