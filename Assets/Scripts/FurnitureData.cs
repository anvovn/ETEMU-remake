using UnityEngine;

[CreateAssetMenu(fileName = "FurnitureData", menuName = "Flyweight")]
public class MinimalFurnitureData : ScriptableObject
{
    public Mesh mesh;
    public Material[] materials;
}