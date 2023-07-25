using UnityEngine;

[CreateAssetMenu(fileName = "MeshSplitterData", menuName = "Mesh-Peeling/MeshSplitterData", order = 0)]
public class MeshSplitterData : ScriptableObject
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