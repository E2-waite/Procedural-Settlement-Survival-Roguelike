using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class Building : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size;
}
