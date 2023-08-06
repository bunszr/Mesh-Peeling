using UnityEngine;

[CreateAssetMenu(fileName = "BoundsAndTriangleIndicesData", menuName = "Mesh-Peeling/BoundsAndTriangleIndicesData", order = 0)]
public class BoundsAndTriangleIndicesData : ScriptableObject
{
    public Data[] data;

    [System.Serializable]
    public struct Data
    {
        public Bounds bounds;
        public int[] triangleIndicesA;

        public Data(Bounds bounds, int[] vertexIndices)
        {
            this.bounds = bounds;
            this.triangleIndicesA = vertexIndices;
        }
    }
}