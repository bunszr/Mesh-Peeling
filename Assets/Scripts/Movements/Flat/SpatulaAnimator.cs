using System.Collections.Generic;
using UnityEngine;

public class SpatulaAnimator : MonoBehaviour
{
    SpatulaController spatulaController;
    [SerializeField] float speed = 2;
    [SerializeField] float amount = 5;

    private void Start()
    {
        spatulaController = GetComponent<SpatulaController>();
    }

    private void Update()
    {
        spatulaController.turnSpeed = Mathf.PingPong(Time.time * speed, amount) - amount * .5f;
    }
}