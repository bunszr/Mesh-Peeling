using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using Random = UnityEngine.Random;

public class Rotater : MonoBehaviour
{
    public float radius = 1;
    public float angleSpeed = 30;

    private void Update()
    {
        transform.rotation *= Quaternion.AngleAxis(angleSpeed * Time.deltaTime, Vector3.up);
    }
}