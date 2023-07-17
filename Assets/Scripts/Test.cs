using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public Vector3[] vertices;
    public Vector2[] uvs;
    public Vector2[] uvs2;
    public int[] triangles;

    public Mesh mesh;

    private void Awake()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        vertices = meshFilter.mesh.vertices;
        uvs = meshFilter.mesh.uv;
        uvs2 = meshFilter.mesh.uv;
        triangles = meshFilter.mesh.triangles;
        mesh = meshFilter.mesh;

        // float speed = 0;
        // for (int i = 0; i < 1000; i += 3)
        // {
        //     speed += .0001f;
        //     vertices[triangles[i]] += Vector3.up * speed;
        //     vertices[triangles[i + 1]] += Vector3.up * speed;
        //     vertices[triangles[i + 2]] += Vector3.up * speed;
        // }
        // mesh.vertices = vertices;


        float speed = 0;
        for (int i = 0; i < 1000; i += 3)
        {
            speed += .0001f;
            vertices[triangles[i + 0]] += vertices[triangles[i + 0]].normalized * speed;
            vertices[triangles[i + 1]] += vertices[triangles[i + 1]].normalized * speed;
            vertices[triangles[i + 2]] += vertices[triangles[i + 2]].normalized * speed;
        }
        mesh.vertices = vertices;
    }
}