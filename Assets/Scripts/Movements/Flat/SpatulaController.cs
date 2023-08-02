using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpatulaController : MonoBehaviour
{
    bool hasPeeling;

    public Cutter cutter2;
    public ShellMeshContainer shellMeshContainer;
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

        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.transform.parent = transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    private void Update()
    {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * turnSpeed, Vector3.up);

        if (hasPeeling)
        {
            angle += Time.deltaTime * (shellAngleSpeed + GetAngleSpeedFromSpeed(speed));
            shellMeshContainer.CurrShellMesh.transform.localPosition += Vector3.up * shellPositionSpeed * Time.deltaTime;
            shellMeshContainer.CurrShellMesh.transform.rotation = Quaternion.AngleAxis(angle, transform.right);
        }
    }

    void OnStartPeeling()
    {
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.rotation = transform.rotation;
        angle = 0;
        hasPeeling = true;
    }

    void OnEndPeeling()
    {
        hasPeeling = false;

        shellMeshContainer.CurrShellMesh.rb.isKinematic = false;
        shellMeshContainer.CurrShellMesh.rb.AddForce((transform.forward + transform.up) * 4, ForceMode.VelocityChange);

        shellMeshContainer.Return(shellMeshContainer.CurrShellMesh);

        shellMeshContainer.CurrShellMesh = shellMeshContainer.Rent();
        shellMeshContainer.CurrShellMesh.rb.isKinematic = true;
        shellMeshContainer.CurrShellMesh.transform.parent = transform;
        shellMeshContainer.CurrShellMesh.transform.localPosition = shellCenterT.localPosition;
        shellMeshContainer.CurrShellMesh.transform.localRotation = shellCenterT.localRotation;
    }

    public float GetAngleSpeedFromSpeed(float speed)
    {
        float radius = shellMeshContainer.CurrShellMesh.transform.localPosition.magnitude;
        float circumferenceOfCircle = 2 * Mathf.PI * radius;
        return speed * 360f / circumferenceOfCircle;
    }
}