using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpatulaController : MonoBehaviour
{
    bool hasPeeling;

    public Cutter2 cutter2;
    public Transform shellCenterT;

    public float speed = .4f;
    [Range(-60, 60)] public float turnSpeed = 0;

    float angle = 0;
    public float shellPositionSpeed = .04f;
    public float shellAngleSpeed = 30;

    private void Start()
    {
        cutter2.onStartPeling += OnStartPeeling;
        cutter2.onEndPeling += OnEndPeeling;
    }

    private void Update()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * turnSpeed, Vector3.up);

        if (hasPeeling)
        {
            angle += Time.deltaTime * (shellAngleSpeed + GetAngleSpeedFromSpeed(speed));
            cutter2.currShellMesh.transform.localPosition += Vector3.up * shellPositionSpeed * Time.deltaTime;
            cutter2.currShellMesh.transform.rotation = Quaternion.AngleAxis(angle, transform.right);
        }
    }

    void OnStartPeeling()
    {
        cutter2.currShellMesh.transform.localPosition = shellCenterT.localPosition;
        cutter2.currShellMesh.transform.rotation = transform.rotation;
        angle = 0;
        hasPeeling = true;
    }

    void OnEndPeeling()
    {
        hasPeeling = false;
    }

    public float GetAngleSpeedFromSpeed(float speed)
    {
        float radius = cutter2.currShellMesh.transform.localPosition.magnitude;
        float circumferenceOfCircle = 2 * Mathf.PI * radius;
        return speed * 360f / circumferenceOfCircle;
    }
}