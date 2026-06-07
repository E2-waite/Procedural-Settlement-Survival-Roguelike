using UnityEngine;
using static GridTile;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Tile")]
public class TileObject : ScriptableObject
{
    TileType type;
    public Color color = Color.white;
    public Material material;
}