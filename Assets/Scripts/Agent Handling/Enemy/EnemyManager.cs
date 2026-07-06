using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    WorldGrid grid;
    GridTile playerSpawn;
    GameContext gameContext;
    public void Init(GameContext context)
    {
        gameContext = context;
        World world = context.world;    
        grid = world.Context.grid;
        playerSpawn = context.spawnTile;
    }

    public void SpawnSettlement()
    {
        GridTile tile = FindTile();

        if (tile == null)
        {
            Debug.LogWarning("No valid tile found for enemy settlement.");
            return; 
        }

        GameObject settlementObj = Instantiate(gameContext.enemySettlement, tile.worldPosition, Quaternion.identity);
        EnemySettlementBuilding settlement = settlementObj.GetComponent<EnemySettlementBuilding>();
        settlement.Init(gameContext);
        settlement.AddTile(tile);
        tile.Build(settlement);
    }

    GridTile FindTile()
    {
        List<GridTile> validTiles = new List<GridTile>();

        // Spawn on a tile with all neighbouring cells empty so the starting area is usable.
        foreach (GridTile tile in grid.Tiles())
        {
            bool valid = true;

            float dist = Vector2Int.Distance(tile.position, playerSpawn.position);
            if (dist < 25 || dist > 50) continue;

            foreach (Vector2Int neighbourPos in Consts.ALL_NEIGHBOURS)
            {
                GridTile neighbourTile = grid.GetTile(tile.position + neighbourPos);
                if (neighbourTile == null || !neighbourTile.IsEmpty)
                {
                    valid = false;
                    break;
                }
            }

            if (valid) validTiles.Add(tile);
        }

        if (validTiles.Count == 0) return null;

        return validTiles[Random.Range(0, validTiles.Count)];
    }
}
