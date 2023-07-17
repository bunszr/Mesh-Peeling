using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using System.Linq;
using System.IO;

public class QuickAccess : OdinEditorWindow
{
    public Object[] objects;

    [MenuItem("Tools/QuickAccess")]
    public static void ShowWindow()
    {
        GetWindow(typeof(QuickAccess));
    }



    [Button, HorizontalGroup("A")]
    public void SelectGameManager()
    {
        // Selection.activeGameObject = FindObjectOfType<GameManager>().gameObject;
    }

    [HorizontalGroup("1")]
    public string typeName = "Obstacle";
    [HorizontalGroup("1"), Button(ButtonSizes.Small)]
    public void SelectWithComponent()
    {
        Selection.objects = FindObjectsOfType<Component>().Where(x => x.GetType().Name == typeName).Select(x => (Object)x.gameObject).ToArray(); // If we write x instead of x.gameObject, it only selects that component. This kind of Transform selects together with meshFilter etc.
    }

    [HorizontalGroup("2"), Button(ButtonSizes.Small, ButtonStyle.FoldoutButton)]
    public void SelectWithName(string name = "Obstacle Holder")
    {
        Selection.objects = FindObjectsOfType<GameObject>().Where(x => x.name == name).Select(x => (Object)x.gameObject).ToArray();
    }

    [Button, HorizontalGroup("A")]
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
    }

    [Button, HorizontalGroup("A")]
    public void DeleteAllPersistantData()
    {
        DeletePrefs();

        string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
        foreach (string filePath in filePaths) File.Delete(filePath);
    }


    [Button(ButtonSizes.Small), HorizontalGroup("B")] public void x1() => Time.timeScale = 1;
    [Button(ButtonSizes.Small), HorizontalGroup("B")] public void x2() => Time.timeScale = 3;
    [Button(ButtonSizes.Small), HorizontalGroup("B")] public void x6() => Time.timeScale = 6;
    [Button(ButtonSizes.Small), HorizontalGroup("B")] public void x12() => Time.timeScale = 12;


    // [Button(ButtonSizes.Small), HorizontalGroup("A")]
    // public void SaveShatter()
    // {
    //     foreach (var item in FindObjectsOfType<RayFire.RayfireShatter>(true))
    //     {
    //         EditorUtility.SetDirty(item);
    //     }
    // }
}