using System.Collections;
using System.Collections.Generic;
using FSM;
using UnityEngine;

public class Test_FSM : MonoBehaviour
{
    private StateMachine fsm;
    public float playerScanningRange = 4f;
    public float ownScanningRange = 6f;

    float DistanceToPlayer()
    {
        // This implementation is an example and may differ for your scene setup
        Vector3 player = transform.position;
        return Vector2.Distance(transform.position, player);
    }

    void MoveTowardsPlayer(float speed)
    {
        // This implementation is an example and may differ for your scene setup
        Vector3 player = transform.position;
        transform.position = Vector2.MoveTowards(transform.position, player, speed * Time.deltaTime);
    }

    void Start()
    {
        fsm = new StateMachine(this);

        // Empty state without any logic
        // fsm.AddState("ExtractIntel", new State());
        fsm.AddState("c1", new CustomSendData(false));
        fsm.AddState("c2", new CustomSendData2(false));
        // fsm.AddState("StopState", new StopState());

        // fsm.AddState("FollowPlayer", new State(
        //     onLogic: (state) => MoveTowardsPlayer(1)
        // ));

        // fsm.AddState("FleeFromPlayer", new State(
        //     onLogic: (state) => MoveTowardsPlayer(-1)
        // ));


        // This configures the entry point of the state machine

        // fsm.RequestStateChange("StopState", true);
        // fsm.AddTransition(new TransitionBase<string>("MoveState", "StopState", true));
        // Initialises the state machine and must be called before OnLogic() is called
        fsm.AddTriggerTransition("OnHit", new Transition("c1", "c2"));
        // fsm.AddTransition(new Transition("c1", "c2", x => Input.GetMouseButtonDown(0)));
        // fsm.AddTriggerTransition();
        fsm.Init();
        fsm.SetStartState("c1");

    }

    private void Update()
    {
        fsm.OnLogic();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("asd");
        fsm.Trigger("OnHit");
    }

    private void FixedUpdate()
    {
    }

    public class CustomSendData : StateBase
    {
        MonoBehaviour mono;



        public CustomSendData(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
        {
        }

        // Important: The constructor must call StateBase's constructor (here: base(...))
        // because it declares whether the state needsExitTime
        // public CustomSendData(MonoBehaviour mono) : base(needsExitTime: false)
        // {
        //     // We need to have access to the MonoBehaviour so that we can rotate it.
        //     // => Keep a reference
        //     this.mono = mono;
        // }

        public override void OnEnter()
        {
            Debug.Log("1");

            // Write your code for OnEnter here.
            // If you don't have any, you can just leave this entire method override out.
        }

        public override void OnExit()
        {
            Debug.Log("1 exit");

            base.OnExit();
        }

        public override void OnLogic()
        {
            Debug.Log("1");
            // this.mono.transform.eulerAngles += new Vector3(0, 0, 100 * Time.deltaTime);
        }
    }

    public class CustomSendData2 : StateBase
    {
        MonoBehaviour mono;

        public CustomSendData2(bool needsExitTime, bool isGhostState = false) : base(needsExitTime, isGhostState)
        {
        }

        // Important: The constructor must call StateBase's constructor (here: base(...))
        // because it declares whether the state needsExitTime
        public override void OnEnter()
        {
            Debug.Log("2");
            // Write your code for OnEnter here.
            // If you don't have any, you can just leave this entire method override out.
        }

        public override void OnLogic()
        {
            Debug.Log("2");
        }
    }
}
