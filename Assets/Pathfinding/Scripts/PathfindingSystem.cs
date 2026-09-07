using Cinderwild.Core;
using Cinderwild.World.Data;
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

        public static void RequestPath(Vector3 start, Vector3 end, Action<List<Vector2Int>> callback, bool includeTarget = false)
        {
            RequestPath(
                new Vector2Int(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.z)),
                new Vector2Int(Mathf.FloorToInt(end.x), Mathf.FloorToInt(end.z)),
                callback,
                includeTarget);
        }

        // Request a path to the position
        public static void RequestPath(Vector2Int start, Vector2Int target, Action<List<Vector2Int>> callback, bool includeTarget = false)
        {
            // Build a bounding box with padding between the start and the target
            int padding = 4;

            int minX = Mathf.Min(start.x, target.x) - padding;
            int minY = Mathf.Min(start.y, target.y) - padding;
            int maxX = Mathf.Max(start.x, target.x) + padding;
            int maxY = Mathf.Max(start.y, target.y) + padding;

            int sizeX = Mathf.Max(3, maxX - minX + 1);
            int sizeY = Mathf.Max(3, maxY - minY + 1);

            bool[,] pathable = new bool[sizeX, sizeY];

            Vector2Int origin = new Vector2Int(minX, minY);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Vector2Int tilePos = new Vector2Int(origin.x + x, origin.y + y);

                    TileData tile = worldData.GetTile(tilePos);

                    pathable[x, y] = !(tile == null || !tile.IsWalkable);

                    if (includeTarget && tilePos == target)
                    {
                        pathable[x, y] = true;
                    }
                }
            }

            // Construct the pathing request
            PathRequest request = new PathRequest
            {
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