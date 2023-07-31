using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class KnifeControllerWithRayDirection : KnifeControllerBase
{
    public Transform knife;
    public Vector3 rayDir = Vector3.forward;
    public bool pressed = false;

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
        shellMeshRotationAngle = 0;
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();
    }

    private void Update()
    {
        RaycastHit hitInFrame = new RaycastHit();
        if (pressed)
        {
            Utility.RaycastWithRay(new Ray(Vector3.back * 2, rayDir), out hitInFrame);

            if (hitInFrame.collider != null)
            {
                Vector3 rotatedHitPoint = Quaternion.AngleAxis(-1, Vector3.up) * hitPointToCheckMovementDirection;
                hitPointToCheckMovementDirection = hitInFrame.point;

                Vector3 dirA = (rotatedHitPoint - hitInFrame.point).normalized;
                Vector3 cross = Vector3.Cross(dirA, -hitInFrame.normal);
                upwards = -cross;

                Quaternion rotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
                knife.rotation = Quaternion.Slerp(knife.rotation, rotation, knifeTurnSpeed * Time.deltaTime);

                Vector3 oldPos = transform.localPosition;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, smoothTime);
                velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
                transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hitInFrame.normal);
            }
        }

        if (hitInFrame.collider == null)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transformRecovery.localPosition, ref vel, smoothTime);
        }

        if (hasPeeling)
        {
            shellMeshRotationAngle += (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(velocity, ShellMeshRadius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
            shellMeshContainer.CurrShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
            shellMeshContainer.CurrShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.up);
        }
    }
}