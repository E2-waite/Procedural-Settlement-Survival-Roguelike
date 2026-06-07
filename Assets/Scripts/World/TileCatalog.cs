using UnityEngine;

[CreateAssetMenu(fileName = "TileCatalog")]
public class TileCatalog : ScriptableObject
{
    public TileObject water;
    public TileObject sand;
    public TileObject grass;
    public TileObject forest;
}