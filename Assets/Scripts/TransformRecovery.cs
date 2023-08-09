using UnityEngine;

public struct TransformRecovery
{
    public readonly Transform transform;
    public readonly Transform parent;
    public readonly Vector3 position;
    public readonly Vector3 localPosition;
    public readonly Quaternion rotation;
    public readonly Quaternion localRotation;
    public readonly Vector3 scale;

    public TransformRecovery(Transform t)
    {
        this.transform = t;
        this.parent = t.parent;
        this.position = t.position;
        this.localPosition = t.localPosition;
        this.rotation = t.rotation;
        this.localRotation = t.localRotation;
        this.scale = t.localScale;
    }

    public void SetPosRotScaleViaTransform(Transform t)
    {
        t.position = position;
        t.rotation = rotation;
        t.localScale = scale;
    }

    public void ResetWitLocalSpace()
    {
        transform.parent = parent;
        transform.localPosition = localPosition;
        transform.localRotation = localRotation;
        transform.localScale = scale;
    }

    public void ResetWitWorldSpace()
    {
        transform.parent = parent;
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }
}