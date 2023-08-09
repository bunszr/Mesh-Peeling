using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class CutterUpdater : MonoBehaviour
{
    PeelingMesh peelingMesh;
    ICutterUpdater[] _cutterUpdaterArray;

    private void Start()
    {
        peelingMesh = GetComponentInParent<LevelDataHolder>().peelingMesh;
        _cutterUpdaterArray = transform.parent.GetComponentsInChildren<ICutterUpdater>();
    }

    private void Update()
    {
        bool hasCut = false;
        for (int i = 0; i < _cutterUpdaterArray.Length; i++)
        {
            _cutterUpdaterArray[i].CutterUpdate();
            if (_cutterUpdaterArray[i].HasCut) hasCut = true;
        }

        if (hasCut)
        {
            peelingMesh.UpdateMeshUv2ToClip();
        }
    }
}

public interface ICutterUpdater
{
    bool HasCut { get; }
    void CutterUpdate();
}