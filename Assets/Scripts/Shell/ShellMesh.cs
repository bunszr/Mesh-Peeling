using UnityEngine;

public class ShellMesh : ShellMeshBase
{
    private void Awake()
    {
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
    }
}
