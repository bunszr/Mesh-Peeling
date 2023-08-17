using UnityEngine;

public class ShellMeshCollision : MonoBehaviour
{
    public Rigidbody rb;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;
    public MeshCollider meshCollider;

    private void Awake()
    {
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
    }
}
