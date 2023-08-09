using System.Collections.Generic;
using UnityEngine;

public class SpatulaController : MonoBehaviour, IKnife
{
    public float speed = .4f;
    [Range(-60, 60)] public float turnSpeed = 0;

    public float Velocity { get; set; }

    private void Update()
    {
        Vector3 oldPos = transform.localPosition;
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        Velocity = Vector3.Distance(transform.localPosition, oldPos) / Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * turnSpeed, Vector3.up);
    }
}