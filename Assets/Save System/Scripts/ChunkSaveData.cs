using Cinderwild.WorldBuilder.Data;
using System.Collections.Generic;

[System.Serializable]
public class ChunkSaveData
{
    List<TileSaveData> tiles = new List<TileSaveData>();

    SaveVector2Int position;

    public ChunkSaveData(Chunk chunk)
    {
        position = new SaveVector2Int(chunk.position);
        foreach (GridTile tile in chunk.tiles.Values)
        {
            tiles.Add(new TileSaveData(tile));
        }
    }
}

[System.Serializable]
public class TileSaveData
{
    int type;
    SaveVector2Int position;
    SaveVector3 worldPosition;
    ResourceSaveData resource;
    BuildingSaveData building;

    public TileSaveData(TileData tile)
    {
        type = (int)tile.type;
        position = new SaveVector2Int(tile.Position);
        worldPosition = new SaveVector3(tile.worldPosition);
        resource = new ResourceSaveData(tile.Resource);
        building = new BuildingSaveData(tile.Building);
    }
}

[System.Serializable]
public class ResourceSaveData
{
    int type;
    int remaining;

    public ResourceSaveData(ResourceNode resource)
    {
        type = (int)resource.type;
        remaining = resource.remaining;
    }
}

[System.Serializable]
public class BuildingSaveData
{
    public BuildingSaveData(Building building)
    {

    }
}