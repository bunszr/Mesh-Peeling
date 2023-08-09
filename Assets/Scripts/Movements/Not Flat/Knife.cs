using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour, IKnife
{
    public KnifeIdleStateData knifeIdleStateData;
    public Transform knifeVisual;
    public float smoothTime = .1f;
    public float knifeTurnSpeed = 2;
    public float Velocity { get; set; }
    public LayerMask peelingMeshLayerMask;
    public KnifeMovementNoCamRayStateData knifeMovementNoCamRayStateData;
}