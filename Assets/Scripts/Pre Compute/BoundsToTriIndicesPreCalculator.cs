using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class BoundsToTriIndicesPreCalculator : MonoBehaviour
{
    [SerializeField] int subdivisionX = 2;
    [SerializeField] int subdivisionY = 2;
    [SerializeField] int subdivisionZ = 2;

    [SerializeField] bool visualizeBounds = false;

    private void Start()
    {
        PeelingMesh peelingMesh = GetComponent<PeelingMesh>();
        if (peelingMesh.boundsAndTriangleIndicesData == null || peelingMesh.boundsAndTriangleIndicesData.data == null || peelingMesh.boundsAndTriangleIndicesData.data.Length == 0)
        {
            Debug.LogError(("boundsAndTriangleIndicesData is null", peelingMesh));
        }

        if (peelingMesh.triangles.Length / 3 != peelingMesh.boundsAndTriangleIndicesData.data.Where(x => x.triangleIndicesA != null).Sum(x => x.triangleIndicesA.Length))
        {
            Debug.LogError(("boundsAndTriangleIndicesData has changed. Recalculate!", peelingMesh));
        }
    }

#if UNITY_EDITOR 
    [Button]
    public void PreCalculate()
    {
        BoundsAndTriangleIndicesData boundsAndTriangleIndicesData = GetComponent<PeelingMesh>().boundsAndTriangleIndicesData;
        if (boundsAndTriangleIndicesData == null)
        {
            Debug.LogError("not found boundsAndTriangleIndicesData", transform);
            return;
        }

        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        int[] triangles = GetComponent<MeshFilter>().sharedMesh.triangles;
        SplitterData[] splitterDataArray = new SplitterData[subdivisionX * subdivisionY * subdivisionZ];
        boundsAndTriangleIndicesData.data = new BoundsAndTriangleIndicesData.Data[splitterDataArray.Length];

        CreateSubBounds(splitterDataArray);
        AssignVertexAccrodingSubBounds(vertices, triangles, splitterDataArray, boundsAndTriangleIndicesData);
        UnityEditor.EditorUtility.SetDirty(boundsAndTriangleIndicesData);
    }
#endif

    private void CreateSubBounds(SplitterData[] splitterDataArray)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Bounds bounds = mesh.bounds;
        bounds.Expand(.1f); // If does not expanded. Some bounds does not contain vertex
        Vector3 size = bounds.size;
        Vector3 min = bounds.min;

        float stepX = size.x / subdivisionX;
        float stepY = size.y / subdivisionY;
        float stepZ = size.z / subdivisionZ;

        int index = 0;

        for (int x = 0; x < subdivisionX; x++)
        {
            for (int y = 0; y < subdivisionY; y++)
            {
                for (int z = 0; z < subdivisionZ; z++)
                {
                    Vector3 subMin = new Vector3(min.x + stepX * x, min.y + stepY * y, min.z + stepZ * z);
                    Vector3 subMax = new Vector3(min.x + stepX * (x + 1), min.y + stepY * (y + 1), min.z + stepZ * (z + 1));
                    Bounds subBounds = new Bounds((subMin + subMax) * 0.5f, subMax - subMin);
                    splitterDataArray[index] = new SplitterData(subBounds);
                    index++;
                }
            }
        }
    }

    private void AssignVertexAccrodingSubBounds(Vector3[] vertices, int[] triangles, SplitterData[] splitterDataArray, BoundsAndTriangleIndicesData boundsAndTriangleIndicesData)
    {
        for (int i = 0; i < triangles.Length; i += 3)
        {
            for (int j = 0; j < splitterDataArray.Length; j++)
            {
                if (splitterDataArray[j].bounds.Contains(vertices[triangles[i]]))
                {
                    splitterDataArray[j].triangleIndices.Add(i);
                    break;
                }
            }
        }

        for (int i = 0; i < splitterDataArray.Length; i++)
        {
            if (splitterDataArray[i].triangleIndices.Count != 0)
            {
                boundsAndTriangleIndicesData.data[i] = new BoundsAndTriangleIndicesData.Data(splitterDataArray[i].bounds, splitterDataArray[i].triangleIndices.ToArray());
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (visualizeBounds)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            SplitterData[] splitterDataArray = new SplitterData[subdivisionX * subdivisionY * subdivisionZ];
            CreateSubBounds(splitterDataArray);
            for (int i = 0; i < splitterDataArray.Length; i++)
            {
                Gizmos.DrawWireCube(splitterDataArray[i].bounds.center, splitterDataArray[i].bounds.size);
            }
        }
    }

    public struct SplitterData
    {
        public readonly Bounds bounds;
        public readonly List<int> triangleIndices;

        public SplitterData(Bounds bounds)
        {
            this.bounds = bounds;
            this.triangleIndices = new List<int>();
        }
    }
}