using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CutterMovementSphere : MonoBehaviour
{
    public float speed = 4;
    public float shellCenterSpeed = 4;
    public Cutter2 cutter2;

    Vector3 initialShellCenterLocalPos;

    bool hasPeeling;

    public Transform shellCenterT;

    Quaternion initialRot;


    [Range(-300, 300)] public float angleSpeed = 4;

    float angle = 0;
    public Vector3 offsetRot;

    public float shellCenterAngleSpeed = 30;
    public float smoothTime = .2f;
    Vector3 vel;

    TransformRecovery transformRecovery;

    private void Start()
    {
        cutter2.onStartPeling += OnStartPeeling;
        cutter2.onEndPeling += OnEndPeeling;
        initialRot = transform.rotation;
        transformRecovery = new TransformRecovery(transform);
    }

    void OnStartPeeling()
    {
        cutter2.currShellMesh.transform.localPosition = shellCenterT.localPosition;
        cutter2.currShellMesh.transform.localRotation = shellCenterT.localRotation;
        angle = 0;
        hasPeeling = true;
    }

    void OnEndPeeling()
    {
        hasPeeling = false;
    }

    public Vector3 rayDir = Vector3.forward;
    public bool pressed = false;
    public bool useMouse = true;

    private void Update()
    {
        RaycastHit hit = new RaycastHit();
        if (Input.GetMouseButton(0) || pressed)
        {
            if (useMouse) Utility.RaycastWithCam(Camera.main, out hit);
            else Utility.RaycastWithRay(new Ray(Vector3.back * 2, rayDir), out hit);

            if (hit.collider != null)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hit.point, ref vel, smoothTime);
                transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hit.normal) * Quaternion.Euler(offsetRot);
            }
        }

        if (hit.collider == null)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transformRecovery.localPosition, ref vel, smoothTime);
        }

        if (hasPeeling)
        {
            angle += Time.deltaTime * shellCenterAngleSpeed;
            cutter2.currShellMesh.transform.localPosition += Vector3.back * shellCenterSpeed * Time.deltaTime;
            cutter2.currShellMesh.transform.rotation = Quaternion.AngleAxis(angle, transform.up);
        }
    }
}















// using System.Collections.Generic;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class CutterMovementSphere : MonoBehaviour
// {
//     public float speed = 4;
//     public float shellCenterSpeed = 4;
//     public Cutter2 cutter2;

//     Vector3 initialShellCenterLocalPos;

//     bool hasPeeling;

//     public Transform shellCenterT;

//     Quaternion initialRot;


//     [Range(-300, 300)] public float angleSpeed = 4;

//     float angle = 0;
//     public Vector3 offsetRot;

//     public float shellCenterAngleSpeed = 30;
//     public float smoothTime = .2f;
//     Vector3 vel;

//     TransformRecovery transformRecovery;

//     private void Start()
//     {
//         cutter2.onStartPeling += OnStartPeeling;
//         cutter2.onEndPeling += OnEndPeeling;
//         initialRot = transform.rotation;
//         transformRecovery = new TransformRecovery(transform);
//     }

//     void OnStartPeeling()
//     {
//         // cutter2.shellCenterT.DOLocalRotate(Vector3.right * 360f, 3).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).From(Vector3.zero);
//         cutter2.currPeelingShellMesh.transform.localPosition = shellCenterT.localPosition;
//         cutter2.currPeelingShellMesh.transform.localRotation = shellCenterT.localRotation;
//         angle = 0;
//         hasPeeling = true;
//     }

//     void OnEndPeeling()
//     {
//         hasPeeling = false;
//     }

//     public bool pressed = false;

//     private void Update()
//     {
//         RaycastHit hit = new RaycastHit();
//         if (Input.GetMouseButton(0) || pressed)
//         {
//             // if (Utility.RaycastWithCam(Camera.main, out hit).collider != null)
//             if (Utility.RaycastWithRay(new Ray(Vector3.back * 2, Vector3.forward), Camera.main, out hit).collider != null)
//             {
//                 transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hit.point, ref vel, smoothTime);
//                 transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(offsetRot);
//                 // transform.rotation = initialRot * Quaternion.FromToRotation(Vector3.up, hit.normal);
//                 // Quaternion sub = initialRot * Quaternion.Inverse(Quaternion.FromToRotation(Vector3.up, hit.normal));
//                 // transform.rotation = initialRot * sub;
//                 // // transform.rotation = initialRot * Quaternion.FromToRotation(Vector3.up, hit.normal);
//             }
//         }

//         if (hit.collider == null)
//         {
//             transform.localPosition = Vector3.SmoothDamp(transform.localPosition, transformRecovery.localPosition, ref vel, smoothTime);
//         }



//         if (hasPeeling)
//         {
//             angle += Time.deltaTime * shellCenterAngleSpeed;
//             cutter2.currPeelingShellMesh.transform.localPosition += Vector3.up * shellCenterSpeed * Time.deltaTime;
//             // cutter2.currPeelingShellMesh.transform.rotation = Quaternion.AngleAxis(angle, transform.right);
//         }
//     }
// }








// using System.Collections.Generic;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class CutterMovement : MonoBehaviour
// {
//     public float speed = 4;
//     public float shellCenterSpeed = 4;
//     public Cutter2 cutter2;

//     Vector3 initialShellCenterLocalPos;

//     bool hasPeeling;

//     private void Start()
//     {
//         cutter2.onStartPeling += OnStartPeeling;
//         cutter2.onEndPeling += OnEndPeeling;
//         initialShellCenterLocalPos = cutter2.shellCenterT.localPosition;
//     }

//     void OnStartPeeling()
//     {
//         // cutter2.shellCenterT.DOLocalRotate(Vector3.right * 360f, 3).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).From(Vector3.zero);
//         cutter2.shellCenterT.localPosition = initialShellCenterLocalPos;
//         cutter2.shellCenterT.rotation = Quaternion.FromToRotation(Vector3.right, cutter2.transform.right);
//         hasPeeling = true;
//     }

//     void OnEndPeeling()
//     {
//         cutter2.shellCenterT.DOKill();
//         hasPeeling = false;
//     }

//     [Range(-300, 300)] public float angleSpeed = 4;

//     private void Update()
//     {
//         transform.localPosition += transform.forward * speed * Time.deltaTime;
//         transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * angleSpeed, Vector3.up);

//         if (hasPeeling)
//         {
//             cutter2.shellCenterT.localPosition += Vector3.up * shellCenterSpeed * Time.deltaTime;
//             cutter2.shellCenterT.rotation *= Quaternion.AngleAxis(Time.deltaTime * 40, cutter2.transform.right);
//         }
//     }
// }









// using System.Collections.Generic;
// using DG.Tweening;
// using Sirenix.OdinInspector;
// using UnityEngine;

// public class CutterMovement : MonoBehaviour
// {
//     public float speed = 4;
//     public float shellCenterSpeed = 4;
//     public Cutter2 cutter2;

//     Vector3 initialShellCenterLocalPos;

//     bool hasPeeling;

//     private void Start()
//     {
//         cutter2.onStartPeling += OnStartPeeling;
//         cutter2.onEndPeling += OnEndPeeling;
//         initialShellCenterLocalPos = cutter2.shellCenterT.localPosition;
//     }

//     void OnStartPeeling()
//     {
//         cutter2.shellCenterT.DOLocalRotate(Vector3.right * 360f, 3).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).From(Vector3.zero);
//         cutter2.shellCenterT.localPosition = initialShellCenterLocalPos;
//         hasPeeling = true;
//     }

//     void OnEndPeeling()
//     {
//         cutter2.shellCenterT.DOKill();
//         hasPeeling = false;
//     }

//     public float angleSpeed = 4;

//     private void Update()
//     {
//         transform.localPosition += transform.forward * speed * Time.deltaTime;
//         transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * angleSpeed, Vector3.up);

//         if (hasPeeling)
//         {
//             cutter2.shellCenterT.localPosition += Vector3.up * shellCenterSpeed * Time.deltaTime;
//         }
//     }
// }