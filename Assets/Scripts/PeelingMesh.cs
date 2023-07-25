using Unity.Collections;

public class PeelingMesh : PeelingMeshBase
{
    public NativeMultiHashMap<int, int> multiHashMapVertIndexToSameVerticesIndices;

    private void OnDisable()
    {
        multiHashMapVertIndexToSameVerticesIndices.Dispose();
    }
}