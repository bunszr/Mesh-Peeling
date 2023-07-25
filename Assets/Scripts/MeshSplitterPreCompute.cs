using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Collections;
using UnityEngine;

public class MeshSplitterPreCompute : MonoBehaviour
{
    [SerializeField] int subdivisionX = 3;
    [SerializeField] int subdivisionY = 3;
    [SerializeField] int subdivisionZ = 3;

    SplitterData[] splitterDataArray;
    public MeshSplitterData meshSplitterData;

    [SerializeField] bool showBounds = false;
    Dictionary<int, SplitterData> dic;

    Vector3[] vertices;

    private void Start()
    {
        PeelingMesh peelingMesh = GetComponent<PeelingMesh>();
        peelingMesh.multiHashMapVertIndexToSameVerticesIndices = new Unity.Collections.NativeMultiHashMap<int, int>(1000, Allocator.Persistent);

        if (meshSplitterData.vertexAndSameVertexDatas == null || meshSplitterData.vertexAndSameVertexDatas.Length == 0)
        {
            Debug.LogError(("vertexAndSameVertexDatas is null"));
            return;
        }

        for (int i = 0; i < meshSplitterData.vertexAndSameVertexDatas.Length; i++)
        {
            if (meshSplitterData.vertexAndSameVertexDatas[i].vertexIndices == null) continue;

            for (int j = 0; j < meshSplitterData.vertexAndSameVertexDatas[i].vertexIndices.Length; j++)
            {
                peelingMesh.multiHashMapVertIndexToSameVerticesIndices.Add(i, meshSplitterData.vertexAndSameVertexDatas[i].vertexIndices[j]);
            }
        }
    }

    [Button]
    public void PreCalculate()
    {
#if UNITY_EDITOR
        vertices = GetComponent<MeshFilter>().sharedMesh.vertices;
        meshSplitterData.vertexAndSameVertexDatas = new MeshSplitterData.VertexAndSameVertexData[vertices.Length];
        CreateSubBounds();
        dic = new Dictionary<int, SplitterData>(vertices.Length);
        AssignVertexAccrodingSubBounds();
        CalculateSameVertexIndices();

        UnityEditor.EditorUtility.SetDirty(meshSplitterData);
#endif
    }

    private void CreateSubBounds()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;
        Bounds bounds = mesh.bounds;
        Vector3 size = bounds.size;
        Vector3 min = bounds.min;

        float stepX = size.x / subdivisionX;
        float stepY = size.y / subdivisionY;
        float stepZ = size.z / subdivisionZ;

        splitterDataArray = new SplitterData[subdivisionX * subdivisionY * subdivisionZ];
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

    private void CalculateSameVertexIndices()
    {
        List<int> sameVertexIndices = new List<int>();
        for (int i = 0; i < vertices.Length; i++)
        {
            sameVertexIndices.Clear();
            SplitterData splitterData = dic[i];
            if (splitterData.vertexIndices == null) continue;

            for (int k = 0; k < splitterData.vertexIndices.Count; k++)
            {
                float sqrDst = Vector3.SqrMagnitude(vertices[i] - vertices[splitterData.vertexIndices[k]]);
                if (sqrDst < .0001f)
                {
                    sameVertexIndices.Add(splitterData.vertexIndices[k]);
                }
            }
            sameVertexIndices.RemoveAt(0);
            meshSplitterData.vertexAndSameVertexDatas[i] = new MeshSplitterData.VertexAndSameVertexData(sameVertexIndices.Count != 0 ? sameVertexIndices.ToArray() : null);
        }
    }

    private void AssignVertexAccrodingSubBounds()
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
            // if (splitterData.vertexIndices == null) Debug.LogError("aaa");
            dic.Add(i, splitterData);
        }
    }

    private void OnDrawGizmos()
    {
        if (showBounds && splitterDataArray != null)
        {
            for (int i = 0; i < splitterDataArray.Length; i++)
            {
                Gizmos.DrawWireCube(splitterDataArray[i].bounds.center, splitterDataArray[i].bounds.size);
            }
        }
    }

    [System.Serializable]
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