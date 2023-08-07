using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

public class SameVertexIndicesPreCalculator : MonoBehaviour
{
    [SerializeField] int subdivisionX = 3;
    [SerializeField] int subdivisionY = 3;
    [SerializeField] int subdivisionZ = 3;

    [SerializeField] bool visualizeBounds = false;

    public SameVertexIndexData sameVertexIndexData;

    private void Start()
    {
        PeelingMesh peelingMesh = GetComponent<PeelingMesh>();

        if (sameVertexIndexData.vertexAndSameVertexDatas == null || sameVertexIndexData.vertexAndSameVertexDatas.Length == 0)
        {
            Debug.LogError(("sameVertexIndexData is null. Created and assign it"));
            return;
        }

        if (sameVertexIndexData.vertexAndSameVertexDatas.Length != peelingMesh.vertices.Length)
        {
            Debug.LogError("The mesh has changed. Press PreCalculate button to calculate SameVertexIndexData", sameVertexIndexData);
            return;
        }

        peelingMesh.multiHashMapVertIndexToSameVerticesIndices = new Unity.Collections.NativeMultiHashMap<int, int>(1000, Allocator.Persistent);

        for (int i = 0; i < sameVertexIndexData.vertexAndSameVertexDatas.Length; i++)
        {
            if (sameVertexIndexData.vertexAndSameVertexDatas[i].vertexIndices == null) continue;

            for (int j = 0; j < sameVertexIndexData.vertexAndSameVertexDatas[i].vertexIndices.Length; j++)
            {
                peelingMesh.multiHashMapVertIndexToSameVerticesIndices.Add(i, sameVertexIndexData.vertexAndSameVertexDatas[i].vertexIndices[j]);
            }
        }
    }

#if UNITY_EDITOR
    [Button]
    public void PreCalculate()
    {
        Vector3[] vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        SplitterData[] splitterDataArray = new SplitterData[subdivisionX * subdivisionY * subdivisionZ];
        SplitterData[] dictionarySplitterData = new SplitterData[vertices.Length];
        sameVertexIndexData.vertexAndSameVertexDatas = new SameVertexIndexData.VertexAndSameVertexData[vertices.Length];

        CreateSubBounds(splitterDataArray);
        AssignVertexAccrodingSubBounds(vertices, splitterDataArray, dictionarySplitterData);
        CalculateSameVertexIndices(vertices, dictionarySplitterData);

        UnityEditor.EditorUtility.SetDirty(sameVertexIndexData);
    }
#endif

    private void CreateSubBounds(SplitterData[] splitterDataArray)
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Bounds bounds = mesh.bounds;
        bounds.Expand(.1f);
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

    private void AssignVertexAccrodingSubBounds(Vector3[] vertices, SplitterData[] splitterDataArray, SplitterData[] dictionarySplitterData)
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            SplitterData splitterData = new SplitterData();
            for (int j = 0; j < splitterDataArray.Length; j++)
            {
                if (splitterDataArray[j].bounds.Contains(vertices[i]))
                {
                    splitterData = splitterDataArray[j];
                    splitterDataArray[j].vertexIndices.Add(i);
                    break;
                }
            }
            dictionarySplitterData[i] = splitterData;
        }
    }

    private void CalculateSameVertexIndices(Vector3[] vertices, SplitterData[] dicSplitterData)
    {
        List<int> sameVertexIndices = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            sameVertexIndices.Clear();
            SplitterData splitterData = dicSplitterData[i];
            if (splitterData.vertexIndices == null) continue;

            for (int k = 0; k < splitterData.vertexIndices.Count; k++)
            {
                if (splitterData.vertexIndices[k] == i) continue;

                float sqrDst = Vector3.SqrMagnitude(vertices[i] - vertices[splitterData.vertexIndices[k]]);
                if (sqrDst < .000001f) sameVertexIndices.Add(splitterData.vertexIndices[k]);
            }
            sameVertexIndexData.vertexAndSameVertexDatas[i] = new SameVertexIndexData.VertexAndSameVertexData(sameVertexIndices.Count != 0 ? sameVertexIndices.ToArray() : null);
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
        public List<int> vertexIndices;

        public SplitterData(Bounds bounds)
        {
            this.bounds = bounds;
            this.vertexIndices = new List<int>();
        }
    }
}