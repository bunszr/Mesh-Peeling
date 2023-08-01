using UnityEngine;
using FSM;
using System.Linq;

public abstract class KnifeStateBase : StateBase
{
    public MonoBehaviour mono;
    public Knife knife;
    public Transform transform => mono.transform;

    public KnifeStateBase(MonoBehaviour mono, bool needsExitTime) : base(needsExitTime)
    {
        this.mono = mono;
        knife = mono.GetComponent<Knife>();
    }
}