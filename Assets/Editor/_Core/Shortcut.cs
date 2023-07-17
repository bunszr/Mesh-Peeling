using UnityEngine;
using UnityEditor;

public class Shortcut : EditorWindow
{
    [MenuItem("Shortcuts/Select Asset &z")]
    private static void ShowWindow()
    {
        string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Selection.activeGameObject);
        var prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        if (prefab != null) EditorGUIUtility.PingObject(prefab);
    }

}