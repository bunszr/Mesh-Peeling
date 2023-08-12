using UnityEngine;

[System.Serializable]
public class KnifeIdleStateData
{
    public float smoothTimeMovement = 0.16f;
    public float rotationSpeed = 2.4f;
}

public class KnifeIdleState : KnifeStateBase
{
    TransformRecovery transformRecovery;
    TransformRecovery knifeVisualTransformRecovery;
    Vector3 vel;
    Vector3 targetLocalPos;

    public KnifeIdleState(MonoBehaviour mono, bool needsExitTime) : base(mono, needsExitTime)
    {
    }

    public override void Init()
    {
        transformRecovery = new TransformRecovery(transform);
        knifeVisualTransformRecovery = new TransformRecovery(knife.knifeVisual);
    }

    public override void OnEnter()
    {
        targetLocalPos = new Vector3(transform.localPosition.x, transform.localPosition.y, transformRecovery.localPosition.z);
    }

    public override void OnLogic()
    {
        if (Vector3.SqrMagnitude(transform.localPosition - targetLocalPos) < .001f) targetLocalPos = transformRecovery.localPosition;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetLocalPos, ref vel, knife.knifeIdleStateData.smoothTimeMovement);
        transform.rotation = Quaternion.Slerp(transform.rotation, transformRecovery.rotation, Time.deltaTime * knife.knifeIdleStateData.rotationSpeed);
        knife.knifeVisual.localRotation = Quaternion.Slerp(knife.knifeVisual.localRotation, knifeVisualTransformRecovery.localRotation, Time.deltaTime * knife.knifeIdleStateData.rotationSpeed);
    }


    public override void OnExit()
    {
    }
}