using UnityEngine;

public class PeelingMeshPropertyBlock : MonoBehaviour
{
    public Color frontColor = Color.white;
    public Texture2D frontTex;
    public Color backColor = Color.white;
    public Texture2D backTex;

    private void Awake()
    {
        OnValidate();
    }

    public void OnValidate()
    {
        if (gameObject.activeSelf)
        {
            if (frontTex == null) frontTex = Resources.Load<Texture2D>("white 10x10");
            if (backTex == null) backTex = Resources.Load<Texture2D>("white 10x10");

            PeelingMesh peelingMesh = GetComponent<PeelingMesh>();

            peelingMesh.materialPropertyBlock = new MaterialPropertyBlock();
            peelingMesh.materialPropertyBlock.SetColor(SPs._FrontColor, frontColor);
            peelingMesh.materialPropertyBlock.SetTexture(SPs._FrontTex, frontTex);
            peelingMesh.materialPropertyBlock.SetColor(SPs._BackColor, backColor);
            peelingMesh.materialPropertyBlock.SetTexture(SPs._BackTex, backTex);
            peelingMesh.meshRenderer.SetPropertyBlock(peelingMesh.materialPropertyBlock);
        }
    }

    void SetProperyBlock(Renderer renderer)
    {
    }
}