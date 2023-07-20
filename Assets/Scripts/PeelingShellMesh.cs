using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class PeelingShellMesh : PeelingMeshBase
{
    public Rigidbody rb;

    protected override void Awake()
    {
        base.Awake();
        // for (int i = 0; i < triangles.Length; i += 3)
        // {
        //     triangles[i + 1] = triangles[i];
        //     triangles[i + 2] = triangles[i];
        // }
        // mesh.triangles = triangles;
    }

    public void Throw(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 4, ForceMode.VelocityChange);
    }

    [Button]
    public void tri()
    {
        for (int i = 0; i < uvs2ToClip.Length; i++)
        {
            uvs2ToClip[i] = Vector2.down;
            uvs2ToClip[i] = Vector2.down;
            uvs2ToClip[i] = Vector2.down;
        }
        mesh.SetUVs(1, uvs2ToClip);
    }
}
