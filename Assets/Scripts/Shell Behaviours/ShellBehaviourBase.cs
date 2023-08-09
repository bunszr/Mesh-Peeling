using UnityEngine;

public abstract class ShellBehaviourBase : ScriptableObject
{
    public abstract void Execute(ShellMeshCollision shellMeshCollision, CutterBase cutterBase);
}