using System.Collections;
using System.Linq;
using DG.Tweening;
using FSM;
using UnityEngine;
using UnityEngine.EventSystems;

public class KnifeMovementStateA : KnifeStateBase
{
    Vector3 vel;
    protected Vector3 hitPointToCheckMovementDirection;
    protected Vector3 upwards;

    public KnifeMovementStateA(MonoBehaviour mono, bool needsExitTime) : base(mono, needsExitTime)
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
        Utility.RaycastWithCam(Camera.main, knife.peelingMeshLayerMask, out hitInFrame);

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
        knife.Velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
    }

    public override void OnExit()
    {
    }
}