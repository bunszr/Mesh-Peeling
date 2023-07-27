using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class KnifeControllerWithSurfaceRotation : KnifeControllerBase
{
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
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, knifeTurnSpeed * Time.deltaTime);

                Vector3 oldPos = transform.localPosition;
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, smoothTime);
                velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
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

// public class KnifeControllerWithSurfaceRotation : MonoBehaviour
// {
//     bool hasPeeling;

//     public float speed = .4f;
//     public Cutter2 cutter2;
//     public Transform shellCenterT;
//     public Rotater rotater;

//     public float upwardsTurnSpeed = 2;

//     float shellMeshRotationAngle = 0;
//     public float shellCenterAngleSpeed = 240;
//     public float shellCenterSpeed = 0.04f;

//     TransformRecovery transformRecovery;

//     public float smoothTime = .2f;
//     Vector3 vel;

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

//     Vector3 upwards = Vector3.left;
//     Vector3 hitPointToCheckMovementDirection;

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
//                 transform.rotation = Quaternion.Slerp(transform.rotation, rotation, upwardsTurnSpeed * Time.deltaTime);

//                 Vector3 oldPos = transform.localPosition;
//                 transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, smoothTime);
//                 velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;


//                 // if (Vector3.Distance(hitPoint, hitInFrame.point) > .06f)
//                 // {
//                 //     Vector3 dirA = (hitInFrame.point - hitPoint).normalized;
//                 //     upwards = Vector3.Cross(dirA, -hitInFrame.normal);
//                 //     hitPoint = hitInFrame.point;
//                 // }
//                 // Debug.DrawRay(hitInFrame.point, upwards);
//                 // transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, smoothTime);
//                 // Quaternion rotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
//                 // transform.rotation = Quaternion.Slerp(transform.rotation, rotation, upwardsTurnSpeed * Time.deltaTime);
//             }
//         }

//         if (hitInFrame.collider == null)
//         {
//             transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transformRecovery.localPosition, ref vel, smoothTime);
//         }

//         if (hasPeeling)
//         {
//             // shellMeshRotationAngle += Time.deltaTime * shellCenterAngleSpeed;
//             shellMeshRotationAngle += (shellCenterAngleSpeed + Utility.GetAngleSpeedFromSpeed(velocity, Radius) + GetAngleSpeedFromAngleSpeedOfRotater()) * Time.deltaTime;
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