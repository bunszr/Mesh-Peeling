using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using FSM;

public class KnifeFSMForTesting : MonoBehaviour
{
    StateMachine fsm;
    Knife knife;

    void Start()
    {
        knife = GetComponent<Knife>();
        fsm = new StateMachine(this);

        fsm.AddState("KnifeMovementNoCamRayState", new KnifeMovementNoCamRayState(this, false));

        fsm.Init();
        fsm.SetStartState("KnifeMovementNoCamRayState");
    }

    private void Update()
    {
        if (fsm != null) fsm.OnLogic();
    }
}