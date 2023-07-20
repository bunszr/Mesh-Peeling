[System.Serializable]
public struct TriangleExtraData
{
    public int[] sameVertexIndices;

    public TriangleExtraData(int[] indices)
    {
        this.sameVertexIndices = indices;
    }
}