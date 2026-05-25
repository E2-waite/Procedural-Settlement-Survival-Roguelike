using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class BuildingObject : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size;
}
