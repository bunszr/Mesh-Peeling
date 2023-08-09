using UnityEngine;

[CreateAssetMenu(fileName = "ShellBouncesy", menuName = "ShellBehaviour/ShellBouncesy", order = 0)]
public class ShellBouncesy : ShellBehaviourBase
{
    public PhysicMaterial physicMaterial;
    public override void Execute(ShellMeshCollision shellMeshCollision, CutterBase cutterBase)
    {
        shellMeshCollision.meshCollider.material = physicMaterial;
    }
}