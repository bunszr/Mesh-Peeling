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
    }

    protected override void OnStartPeeling()
    {
        base.OnStartPeeling();
        cutter2.currShellMesh.transform.localPosition = shellCenterT.localPosition;
        cutter2.currShellMesh.transform.localRotation = shellCenterT.localRotation;
        shellMeshRotationAngle = 0;
    }

    protected override void OnEndPeeling()
    {
        base.OnEndPeeling();
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
            cutter2.currShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
            cutter2.currShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.up);
        }
    }
}










// using System.Collections.Generic;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class KnifeController : MonoBehaviour
// {
//     bool hasPeeling;
//     Vector3 vel;
//     TransformRecovery transformRecovery;

//     public float smoothTime = .1f;

//     public Cutter2 cutter2;
//     public Transform shellCenterT;
//     public Transform knife;
//     public Rotater rotater;

//     float shellMeshRotationAngle = 0;
//     public float shellCenterSpeed = .04f;
//     public float shellAngleSpeedAdder = 250;

//     public float knifeTurnSpeed = 2;

//     public float Radius => cutter2.currShellMesh.transform.localPosition.magnitude;

//     private void Start()
//     {
//         cutter2.onStartPeling += OnStartPeeling;
//         cutter2.onEndPeling += OnEndPeeling;
//         transformRecovery = new TransformRecovery(transform);
//     }

//     void OnStartPeeling()
//     {
//         cutter2.currShellMesh.transform.localPosition = shellCenterT.localPosition;
//         cutter2.currShellMesh.transform.localRotation = shellCenterT.localRotation;
//         shellMeshRotationAngle = 0;
//         hasPeeling = true;
//     }

//     void OnEndPeeling()
//     {
//         hasPeeling = false;
//     }

//     public Vector3 rayDir = Vector3.forward;
//     public bool pressed = false;
//     public bool useMouse = true;

//     float velocity;

//     Vector3 hitPointToCheckMovementDirection;
//     Vector3 upwards = Vector3.left;

//     private void Update()
//     {
//         RaycastHit hitInFrame = new RaycastHit();
//         if (Input.GetMouseButton(0) || pressed)
//         {
//             if (useMouse) Utility.RaycastWithCam(Camera.main, out hitInFrame);
//             else Utility.RaycastWithRay(new Ray(Vector3.back * 2, rayDir), out hitInFrame);

//             if (hitInFrame.collider != null)
//             {
//                 Vector3 rotatedHitPoint = Quaternion.AngleAxis(-1, Vector3.up) * hitPointToCheckMovementDirection;
//                 hitPointToCheckMovementDirection = hitInFrame.point;

//                 Vector3 dirA = (rotatedHitPoint - hitInFrame.point).normalized;
//                 Vector3 cross = Vector3.Cross(dirA, -hitInFrame.normal);
//                 upwards = -cross;

//                 Quaternion rotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
//                 knife.rotation = Quaternion.Slerp(knife.rotation, rotation, knifeTurnSpeed * Time.deltaTime);
//                 // Debug.DrawRay(hitInFrame.point, upwards);

//                 Vector3 oldPos = transform.localPosition;
//                 transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, smoothTime);
//                 velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
//                 transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hitInFrame.normal);
//             }
//         }

//         if (hitInFrame.collider == null)
//         {
//             transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transformRecovery.localPosition, ref vel, smoothTime);
//         }

//         if (hasPeeling)
//         {
//             shellMeshRotationAngle += (shellAngleSpeedAdder + Utility.GetAngleSpeedFromSpeed(velocity, Radius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
//             cutter2.currShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
//             cutter2.currShellMesh.transform.rotation = Quaternion.AngleAxis(shellMeshRotationAngle, transform.up);
//         }
//     }

//     public float GetAngleSpeedFromAngleSpeedOfRotater()
//     {
//         float radius = cutter2.currShellMesh.transform.localPosition.magnitude;
//         return Mathf.Abs(rotater.angleSpeed * (rotater.radius / radius));
//     }
// }