using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "ShellHinge", menuName = "ShellBehaviour/ShellHinge", order = 0)]
public class ShellHinge : ShellBehaviourBase
{
    public Vector2 minMaxForce = new Vector2(2, 4);
    public override void Execute(ShellMeshCollision shellMeshCollision, CutterBase cutterBase)
    {
        float force = Random.Range(minMaxForce.x, minMaxForce.y);
        shellMeshCollision.rb.AddForce((cutterBase.transform.forward + Vector3.up).normalized * force, ForceMode.VelocityChange);
        shellMeshCollision.StartCoroutine(Method(shellMeshCollision, cutterBase));
    }

    IEnumerator Method(ShellMeshCollision shellMeshCollision, CutterBase cutterBase)
    {
        yield return new WaitForSeconds(.3f);
        while (shellMeshCollision.rb.velocity.y > 0) yield return null;
        shellMeshCollision.gameObject.AddComponent<HingeJoint>();
    }
}