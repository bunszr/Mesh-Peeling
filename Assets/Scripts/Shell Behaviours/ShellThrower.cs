using UnityEngine;

[CreateAssetMenu(fileName = "ShellThrower", menuName = "ShellBehaviour/ShellThrower", order = 0)]
public class ShellThrower : ShellBehaviourBase
{
    public Vector2 minMaxForce = new Vector2(2, 4);
    public override void Execute(ShellMeshCollision shellMeshCollision, CutterBase cutterBase)
    {
        float force = Random.Range(minMaxForce.x, minMaxForce.y);
        shellMeshCollision.rb.AddForce((cutterBase.transform.forward + Vector3.up).normalized * force, ForceMode.VelocityChange);
    }
}