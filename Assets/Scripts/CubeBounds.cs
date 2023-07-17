using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using System.Linq;
using Random = UnityEngine.Random;

// line box intersection https://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection.html

public class CubeBounds : MonoBehaviour
{
    public Renderer render;

    // Version of Transform.TransformPoint which is unaffected by scale?  https://answers.unity.com/questions/1238142/version-of-transformtransformpoint-which-is-unaffe.html
    public bool HasInsadeOfBox(Vector3 worldPos)
    {
        // Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);
        Matrix4x4 worldToLocalMatrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse;
        Vector3 local = worldToLocalMatrix.MultiplyPoint3x4(worldPos);
        Vector3 scaleAbs = Abs(transform.localScale * .5f);
        return Mathf.Abs(local.x) <= scaleAbs.x && Mathf.Abs(local.y) <= scaleAbs.y && Mathf.Abs(local.z) <= scaleAbs.z;
    }

    public Vector3 Abs(Vector3 v) => new Vector3(Mathf.Abs(v.x), Mathf.Abs(v.y), Mathf.Abs(v.z));
}
