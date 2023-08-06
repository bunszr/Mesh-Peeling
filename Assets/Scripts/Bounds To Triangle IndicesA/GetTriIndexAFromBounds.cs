using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;

public class GetTriIndexAFromBounds : MonoBehaviour, IGetTriIndicesA
{
    List<BoundsAndTriangleIndicesData.Data> tempBoundsData = new List<BoundsAndTriangleIndicesData.Data>();
    BoundsAndTriangleIndicesData boundsAndTriangleIndicesData;
    Transform peelingMeshT;

    [SerializeField] bool showInterctedBounds = true;
    [SerializeField] Color gizmoColor = Color.red;

    private void Start()
    {
        boundsAndTriangleIndicesData = GetComponentInParent<LevelDataHolder>().peelingMesh.boundsAndTriangleIndicesData;
        peelingMeshT = GetComponentInParent<LevelDataHolder>().peelingMesh.transform;
    }

    public NativeArray<int> GetIndices(CutterBase cutterBase)
    {
        float radius = cutterBase.transform.localScale.x * .5f;
        tempBoundsData.Clear();
        for (int i = 0; i < boundsAndTriangleIndicesData.data.Length; i++)
        {
            Vector3 local = peelingMeshT.worldToLocalMatrix.MultiplyPoint3x4(cutterBase.transform.position);
            Vector3 closest = boundsAndTriangleIndicesData.data[i].bounds.ClosestPoint(local);
            float dst = Vector3.Distance(local, closest);
            if (dst < radius) tempBoundsData.Add(boundsAndTriangleIndicesData.data[i]);
        }

        int triIndicesALength = 0;
        for (int i = 0; i < tempBoundsData.Count; i++) triIndicesALength += tempBoundsData[i].triangleIndicesA.Length;

        NativeArray<int> indices = new NativeArray<int>(triIndicesALength, Allocator.TempJob);
        int index = 0;
        for (int i = 0; i < tempBoundsData.Count; i++)
        {
            if (tempBoundsData[i].triangleIndicesA != null)
            {
                for (int j = 0; j < tempBoundsData[i].triangleIndicesA.Length; j++)
                {
                    indices[index] = tempBoundsData[i].triangleIndicesA[j];
                    index++;
                }
            }
        }
        return indices;
    }

    private void OnDrawGizmos()
    {
        if (showInterctedBounds && tempBoundsData != null && tempBoundsData.Count > 0 && peelingMeshT != null)
        {
            Gizmos.matrix = peelingMeshT.localToWorldMatrix;
            Gizmos.color = gizmoColor;
            for (int i = 0; i < tempBoundsData.Count; i++)
            {
                Gizmos.DrawWireCube(tempBoundsData[i].bounds.center, tempBoundsData[i].bounds.size);
            }
        }
    }
}
