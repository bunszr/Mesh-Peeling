using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using FSM;

public class KnifeFSM : MonoBehaviour
{
    PeelingMesh peelingMesh;
    StateMachine fsm;
    Camera cam;
    Knife knife;

    [SerializeField] bool runStateOfMovementA = true;

    void Start()
    {
        knife = GetComponent<Knife>();
        peelingMesh = GetComponentInParent<LevelDataHolder>().peelingMesh;
        cam = Camera.main;
        fsm = new StateMachine(this);

        fsm.AddState("KnifeIdleState", new KnifeIdleState(this, false));
        fsm.AddState("KnifeMovementState", runStateOfMovementA ? new KnifeMovementStateA(this, false) : new KnifeMovementStateB(this, false));
        fsm.AddState("KnifeMovementWhenPeelEnough", new KnifeMovementWhenPeelEnough(this, false));

        fsm.AddTransition(new Transition("KnifeIdleState", "KnifeMovementState", x => IsIdleToMove()));
        fsm.AddTransition(new Transition("KnifeMovementState", "KnifeIdleState", x => IsMoveToIdle()));
        fsm.AddTransition(new Transition("KnifeMovementState", "KnifeMovementWhenPeelEnough", x => IsEnoughPeel()));

        fsm.Init();
        fsm.SetStartState("KnifeIdleState");
    }

    private void Update()
    {
        if (fsm != null) fsm.OnLogic();
    }

    public bool IsIdleToMove()
    {
        Utility.RaycastWithCam(cam, out RaycastHit hit);
        return Input.GetMouseButton(0) && hit.collider != null && hit.collider.GetComponent<PeelingMesh>();
    }

    public bool IsMoveToIdle()
    {
        Utility.RaycastWithCam(cam, out RaycastHit hit);
        return Input.GetMouseButtonUp(0) || hit.collider == null;
    }

    public bool IsEnoughPeel()
    {
        return peelingMesh.PercentOfPeeling > .9f;
    }

#if UNITY_EDITOR
    [ReadOnly, ShowInInspector] List<string> states = new List<string>();
    [ReadOnly, ShowInInspector] string activeStateName = "sss";
    private void LateUpdate()
    {
        if (fsm != null && activeStateName != fsm.ActiveStateName)
        {
            activeStateName = fsm.ActiveStateName;
            states.Add(activeStateName);
            if (states.Count > 5) states.RemoveAt(0);
        }
    }
#endif
}