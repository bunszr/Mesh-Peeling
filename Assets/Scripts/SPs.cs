using UnityEngine;

public static class SPs
{
    public static readonly int _FrontColor = Shader.PropertyToID("_FrontColor");
    public static readonly int _FrontTex = Shader.PropertyToID("_FrontTex");
    public static readonly int _BackColor = Shader.PropertyToID("_BackColor");
    public static readonly int _BackTex = Shader.PropertyToID("_BackTex");
    public static readonly int _Cutoff = Shader.PropertyToID("_Cutoff");
}