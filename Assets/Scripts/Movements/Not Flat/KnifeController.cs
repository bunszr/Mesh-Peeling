using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class KnifeController : KnifeControllerBase
{
    public Transform knife;

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
        RaycastHit hitInFrame = new RaycastHit();
        if (Input.GetMouseButton(0))
        {
            Utility.RaycastWithCam(Camera.main, out hitInFrame);

            if (hitInFrame.collider != null)
            {
                Vector3 rotatedHitPoint = Quaternion.AngleAxis(-1, Vector3.up) * hitPointToCheckMovementDirection;
                hitPointToCheckMovementDirection = hitInFrame.point;

                Vector3 dirA = (rotatedHitPoint - hitInFrame.point).normalized;
                Vector3 cross = Vector3.Cross(dirA, -hitInFrame.normal);
                upwards = -cross;

                Quaternion rotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
                knife.rotation = Quaternion.Slerp(knife.rotation, rotation, knifeTurnSpeed * Time.deltaTime);
                // Debug.DrawRay(hitInFrame.point, upwards);

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