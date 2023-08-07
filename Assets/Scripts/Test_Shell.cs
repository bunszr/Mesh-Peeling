using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Test_Shell : MonoBehaviour
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

            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            materialPropertyBlock.SetColor(SPs._FrontColor, frontColor);
            materialPropertyBlock.SetTexture(SPs._FrontTex, frontTex);
            materialPropertyBlock.SetColor(SPs._BackColor, backColor);
            materialPropertyBlock.SetTexture(SPs._BackTex, backTex);
            GetComponent<Renderer>().SetPropertyBlock(materialPropertyBlock);
        }
    }

    [Button]
    public void RecalculateNormals()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh.RecalculateNormals();
    }
}