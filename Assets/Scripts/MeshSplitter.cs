using System.Collections.Generic;
using UnityEngine;

public class MeshSplitter : MonoBehaviour
{
    [SerializeField] int subdivisionX = 3;
    [SerializeField] int subdivisionY = 3;
    [SerializeField] int subdivisionZ = 3;

    SplitterData[] dataArray;

    PeelingMesh peelingMesh;

    [SerializeField] bool showBounds = false;

    private void Awake()
    {
        peelingMesh = GetComponent<PeelingMesh>();
        peelingMesh.trianglesExtraDatas = new TriangleExtraData[peelingMesh.triangles.Length];

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = mesh.bounds;
        Vector3 size = bounds.size;
        Vector3 min = bounds.min;

        float stepX = size.x / subdivisionX;
        float stepY = size.y / subdivisionY;
        float stepZ = size.z / subdivisionZ;

        dataArray = new SplitterData[subdivisionX * subdivisionY * subdivisionZ];
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
                    dataArray[index] = new SplitterData(subBounds);
                    index++;
                }
            }
        }

        for (int i = 0; i < peelingMesh.triangles.Length; i++)
        {
            for (int j = 0; j < dataArray.Length; j++)
            {
                if (dataArray[j].bounds.Contains(peelingMesh.vertices[peelingMesh.triangles[i]]))
                {
                    dataArray[j].vertexIndices.Add(peelingMesh.triangles[i]);
                    break;
                }
            }
        }

        List<int> sameVertexIndices = new List<int>();
        for (int i = 0; i < peelingMesh.triangles.Length; i++)
        {
            sameVertexIndices.Clear();
            for (int j = 0; j < dataArray.Length; j++)
            {
                if (dataArray[j].bounds.Contains(peelingMesh.vertices[peelingMesh.triangles[i]]))
                {
                    Vector3 v = peelingMesh.vertices[peelingMesh.triangles[i]];
                    for (int k = 0; k < dataArray[j].vertexIndices.Count; k++)
                    {
                        float sqrDst = Vector3.SqrMagnitude(v - peelingMesh.vertices[dataArray[j].vertexIndices[k]]);
                        if (sqrDst < .001f)
                        {
                            sameVertexIndices.Add(dataArray[j].vertexIndices[k]);
                        }
                    }
                    break;
                }
            }
            peelingMesh.trianglesExtraDatas[i] = new TriangleExtraData(sameVertexIndices.ToArray());
        }
    }

    private void OnDrawGizmos()
    {
        if (showBounds && dataArray != null)
        {
            for (int i = 0; i < dataArray.Length; i++)
            {
                Gizmos.DrawWireCube(dataArray[i].bounds.center, dataArray[i].bounds.size);
            }
        }
    }

    public struct SplitterData
    {
        public readonly Bounds bounds;
        public List<int> vertexIndices;

        public SplitterData(Bounds bounds)
        {
            this.bounds = bounds;
            this.vertexIndices = new List<int>();
        }
    }
}
