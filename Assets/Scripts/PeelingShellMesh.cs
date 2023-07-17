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
        // for (int i = 0; i < triangles.Length; i += 3)
        // {
        //     triangles[i + 1] = triangles[i];
        //     triangles[i + 2] = triangles[i];
        // }
        // mesh.triangles = triangles;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            triangles[i + 0] = 0;
            triangles[i + 1] = 0;
            triangles[i + 2] = 0;
        }
        mesh.triangles = triangles;
    }

    [Button]
    public void Compare()
    {
        PeelingMesh peelingMesh = FindObjectOfType<PeelingMesh>();
        for (int i = 0; i < triangles.Length; i += 3)
        {
            if (triangles[i + 0] == peelingMesh.triangles[i + 0] && triangles[i + 1] == peelingMesh.triangles[i + 1] && triangles[i + 2] == peelingMesh.triangles[i + 2])
            {

            }
            else
            {
                Debug.LogError("asdasd");
            }


        }
        mesh.triangles = triangles;
    }
}
