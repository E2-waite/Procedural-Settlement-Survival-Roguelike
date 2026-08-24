using Cinderwild.Core.Data;
using Cinderwild.WorldBuilder.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.Pathfinding.Runtime
{
    public static class PathfindingSystem
    {
        private static PathfindingManager manager;
        private static WorldData worldData;
        public static void Init(Context context)
        {
            manager = context.Pathfinding;
            worldData = context.World.Data;
        }

        // Request a path to the position
        public static void RequestPath(Vector2Int start, Vector2Int target, Action<List<Vector2Int>> callback, bool includeTarget = false)
        {
            int pathRange = Mathf.RoundToInt(Vector2Int.Distance(start, target));
            int size = pathRange * 4;

            bool[,] pathable = new bool[size, size];

            Vector2Int origin = new Vector2Int(
                                start.x - pathRange,
                                start.y - pathRange);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2Int tilePos = new Vector2Int(origin.x + x, origin.y + y);

                    TileData tile = worldData.GetTile(tilePos);
                    if (tile == null || !tile.IsWalkable)
                        pathable[x, y] = false;
                    else
                        pathable[x, y] = true;

                    if (includeTarget && tilePos == target)
                    {
                        pathable[x, y] = true;
                    }
                }
            }

            // Construct the pathing request
            PathRequest request = new PathRequest
            {
                size = size,
                start = start,
                target = target,
                origin = origin,
                pathable = pathable,
                callback = callback
            };

            // Send pathfinding request to the manager
            manager.RequestPath(request);
        }
    }
}