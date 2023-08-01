using System.Collections;
using System.Linq;
using DG.Tweening;
using FSM;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnifeMovementStateB : KnifeStateBase
{
    Vector3 vel;
    protected Vector3 hitPointToCheckMovementDirection;
    protected Vector3 upwards;

    public KnifeMovementStateB(MonoBehaviour mono, bool needsExitTime) : base(mono, needsExitTime)
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
        RaycastHit hitInFrame = new RaycastHit();
        Utility.RaycastWithCam(Camera.main, out hitInFrame);

        if (hitInFrame.collider == null) return;

        Vector3 rotatedHitPoint = Quaternion.AngleAxis(-1, Vector3.up) * hitPointToCheckMovementDirection;
        hitPointToCheckMovementDirection = hitInFrame.point;

        Vector3 dirA = (rotatedHitPoint - hitInFrame.point).normalized;
        Vector3 cross = Vector3.Cross(dirA, -hitInFrame.normal);
        upwards = -cross;

        Quaternion knifeVisualToRotation = Quaternion.LookRotation(-hitInFrame.normal, upwards);
        knife.knifeVisual.rotation = Quaternion.Slerp(knife.knifeVisual.rotation, knifeVisualToRotation, knife.knifeTurnSpeed * Time.deltaTime);
        // Debug.DrawRay(hitInFrame.point, upwards);

        Vector3 oldPos = transform.localPosition;
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, hitInFrame.point, ref vel, knife.smoothTime);
        knife.velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
        transform.rotation = Quaternion.FromToRotation(-Vector3.forward, hitInFrame.normal);
    }

    public override void OnExit()
    {
    }
}