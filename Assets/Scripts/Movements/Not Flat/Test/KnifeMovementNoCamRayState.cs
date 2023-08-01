using System.Collections;
using System.Linq;
using DG.Tweening;
using FSM;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
public class KnifeMovementNoCamRayStateData
{
    public bool pressed = true;
    public Vector3 rayDir = Vector3.forward;
    public Vector3 rayOrigin = new Vector3(0, 0, -2f);

}

public class KnifeMovementNoCamRayState : KnifeStateBase
{
    Vector3 vel;
    protected Vector3 hitPointToCheckMovementDirection;
    protected Vector3 upwards;

    public KnifeMovementNoCamRayState(MonoBehaviour mono, bool needsExitTime) : base(mono, needsExitTime)
    {
    }

    public override void Init()
    {
    }

    public override void OnEnter()
    {
    }

    public override void OnLogic()
    {
        if (!knife.knifeMovementNoCamRayStateData.pressed) return;

        RaycastHit hitInFrame = new RaycastHit();
        Utility.RaycastWithRay(new Ray(knife.knifeMovementNoCamRayStateData.rayOrigin, knife.knifeMovementNoCamRayStateData.rayDir), out hitInFrame);

        if (hitInFrame.collider == null) return;

        Vector3 rotatedHitPoint = Quaternion.AngleAxis(-1, Vector3.up) * hitPointToCheckMovementDirection;
        hitPointToCheckMovementDirection = hitInFrame.point;

        Vector3 dirA = (rotatedHitPoint - hitInFrame.point).normalized;
        Vector3 cross = Vector3.Cross(dirA, -hitInFrame.normal);
        upwards = -cross;

        Quaternion rotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, knife.knifeTurnSpeed * Time.deltaTime);

        Vector3 oldPos = transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, knife.smoothTime);
        knife.velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
    }

    public override void OnExit()
    {
    }
}