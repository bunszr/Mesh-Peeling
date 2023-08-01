using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public KnifeIdleStateData knifeIdleStateData;
    public PeelingMesh peelingMesh;
    public Transform knifeVisual;
    public float smoothTime = .1f;
    public float knifeTurnSpeed = 2;
    public float velocity;

    public KnifeMovementNoCamRayStateData knifeMovementNoCamRayStateData;
}