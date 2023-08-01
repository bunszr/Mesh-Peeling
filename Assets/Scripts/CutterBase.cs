using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public abstract class CutterBase : MonoBehaviour
{
    public ShellMeshContainer shellMeshContainer;

    public System.Action onStartPeling;
    public System.Action onEndPeling;
}