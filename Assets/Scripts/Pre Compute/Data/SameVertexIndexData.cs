using UnityEngine;

[CreateAssetMenu(fileName = "SameVertexIndexData", menuName = "Mesh-Peeling/SameVertexIndexData", order = 0)]
public class SameVertexIndexData : ScriptableObject
{
    public VertexAndSameVertexData[] vertexAndSameVertexDatas;


    [System.Serializable]
    public struct VertexAndSameVertexData
    {
        public int[] vertexIndices;

        public VertexAndSameVertexData(int[] vertexIndices)
        {
            this.vertexIndices = vertexIndices;
        }
    }
}