using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MinimalFurnitureInstance : MonoBehaviour
{
    [SerializeField] private MinimalFurnitureData sharedData;

    private void OnValidate() => Initialize();
    private void Start() => Initialize();

    private void Initialize()
    {
        if (sharedData == null)
        {
            return;
        }
        GetComponent<MeshFilter>().sharedMesh = sharedData.mesh;
        GetComponent<MeshRenderer>().sharedMaterials = sharedData.materials;
    }
}