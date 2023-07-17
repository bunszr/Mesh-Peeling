#if UNITY_EDITOR

using UnityEditor;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshSaverEditor : MonoBehaviour
{
    [Tooltip("meshToSave listesi boşken çalışır")]
    public bool thisMeshIsSave = true;

    public List<MeshFilter> meshToSave = new List<MeshFilter> ();

    private void Start() {
         if (thisMeshIsSave) {
            meshToSave.Add(GetComponent<MeshFilter>());
        }
    }

    //[MenuItem ("Tools/MeshFilter/Save Mesh...")] //MonoBehaviour'dan dolayı işe yaramıyor sanırım
    [ContextMenu("Save Mesh")]
    void MeshSaver ()
    {
        for (int i = 0; i < meshToSave.Count; i++)
        {
            MenuCommand command = new MenuCommand (meshToSave[i]);
            SaveMeshInPlace (command);
        }
    }

    private void Update ()
    {
        if (Input.GetKeyDown (KeyCode.H))
        {
            MeshSaver ();
        }
    }

    //[MenuItem ("Tools/MeshFilter/Save Mesh...")]
    public void SaveMeshInPlace (MenuCommand menuCommand)
    {
        MeshFilter mf = menuCommand.context as MeshFilter;
        Mesh m = mf.sharedMesh;
        SaveMesh (m , m.name , false);
    }

    public void SaveMesh (Mesh mesh , string name , bool makeNewInstance)
    {
        string path =  "Assets/Mesh/" + name + ".asset";
        if (string.IsNullOrEmpty (path)) return;
        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate (mesh) as Mesh : mesh;

        AssetDatabase.CreateAsset (meshToSave , path);
        AssetDatabase.SaveAssets ();
    }

}
#endif