using System.Linq;
using UnityEngine;

public class ShellMeshPropertyBlockSetter : MonoBehaviour
{
    [Tooltip("To show insade of peeling mesh. Set to true. Then change peeling mesh material properties.")]
    [SerializeField] bool hasUpdate = false;

    Renderer[] renderers;

    private void Start()
    {
        PeelingMesh peelingMesh = GetComponentInParent<LevelDataHolder>().peelingMesh;
        renderers = GetComponentsInChildren<ShellMeshBase>(true).Select(x => x.GetComponent<Renderer>()).ToArray();
        foreach (var ren in renderers)
        {
            ren.SetPropertyBlock(peelingMesh.materialPropertyBlock);
        }
    }

    private void Update()
    {
        if (hasUpdate)
        {
            foreach (var ren in renderers)
            {
                PeelingMesh peelingMesh = GetComponentInParent<LevelDataHolder>().peelingMesh;
                ren.SetPropertyBlock(peelingMesh.materialPropertyBlock);
            }
        }
    }
}