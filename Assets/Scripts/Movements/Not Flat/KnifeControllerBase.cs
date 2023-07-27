using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class KnifeControllerBase : MonoBehaviour
{
    protected bool hasPeeling;
    protected Vector3 vel;
    protected TransformRecovery transformRecovery;

    public float smoothTime = .1f;

    public Cutter2 cutter2;
    public Transform shellCenterT;
    public Rotater rotater;

    protected float shellMeshRotationAngle = 0;
    public float shellCenterSpeed = .04f;
    public float shellAngleSpeedAdder = 5;

    public float knifeTurnSpeed = 2;

    protected float velocity;

    protected Vector3 hitPointToCheckMovementDirection;
    protected Vector3 upwards = Vector3.left;

    public float ShellMeshRadius => cutter2.currShellMesh.transform.localPosition.magnitude;

    protected virtual void Start()
    {
        cutter2.onStartPeling += OnStartPeeling;
        cutter2.onEndPeling += OnEndPeeling;
        transformRecovery = new TransformRecovery(transform);
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
        float radius = cutter2.currShellMesh.transform.localPosition.magnitude;
        return Mathf.Abs(rotater.angleSpeed * (rotater.radius / radius));
    }
}