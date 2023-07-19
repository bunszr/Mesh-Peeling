using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class CutterMovement : MonoBehaviour
{
    public float speed = 4;
    public float shellCenterSpeed = 4;
    public Cutter2 cutter2;

    Vector3 initialShellCenterLocalPos;
    public Transform shellCenterT;

    bool hasPeeling;

    private void Start()
    {
        cutter2.onStartPeling += OnStartPeeling;
        cutter2.onEndPeling += OnEndPeeling;
        initialShellCenterLocalPos = shellCenterT.localPosition;
    }

    void OnStartPeeling()
    {
        // cutter2.shellCenterT.DOLocalRotate(Vector3.right * 360f, 3).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart).From(Vector3.zero);
        cutter2.currPeelingShellMesh.transform.localPosition = initialShellCenterLocalPos;
        cutter2.currPeelingShellMesh.transform.rotation = transform.rotation;
        angle = 0;
        hasPeeling = true;
    }

    void OnEndPeeling()
    {
        hasPeeling = false;
    }

    [Range(-300, 300)] public float angleSpeed = 4;

    float angle = 0;
    public float shellCenterAngleSpeed = 30;

    private void Update()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * angleSpeed, Vector3.up);

        if (hasPeeling)
        {
            angle += Time.deltaTime * shellCenterAngleSpeed;
            cutter2.currPeelingShellMesh.transform.localPosition += Vector3.up * shellCenterSpeed * Time.deltaTime;
            cutter2.currPeelingShellMesh.transform.rotation = Quaternion.AngleAxis(angle, transform.right);
        }
    }
}








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