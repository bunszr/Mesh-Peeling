using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Knife : MonoBehaviour, IKnife
{
    public KnifeIdleStateData knifeIdleStateData;
    public Transform knifeVisual;
    public float smoothTime = .1f;
    public float knifeTurnSpeed = 2;
    public float Velocity { get; set; }
    public KnifeMovementNoCamRayStateData knifeMovementNoCamRayStateData;
}