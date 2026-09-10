using Cinderwild.World.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cinderwild.Pathfinding.Runtime
{
    // Pathfinding manager for handling pathfinding threads
    public class PathfindingManager : MonoBehaviour
    {
        private bool running = true;
        private WorldData worldData;
        private ConcurrentQueue<PathRequest> requestQueue = new ConcurrentQueue<PathRequest>();
        private ConcurrentQueue<System.Action> resultQueue = new ConcurrentQueue<System.Action>();
        private Thread workerThread;
        
        public void Init(WorldData worldData)
        {
            this.worldData = worldData;
            workerThread = new Thread(WorkerLoop);
            workerThread.Start();
        }

        public void RequestPath(Vector3 start, Vector3 end, Action<List<Vector2Int>> callback, bool includeTarget = false)
        {
            RequestPath(
                new Vector2Int(Mathf.FloorToInt(start.x), Mathf.FloorToInt(start.z)),
                new Vector2Int(Mathf.FloorToInt(end.x), Mathf.FloorToInt(end.z)),
                callback,
                includeTarget);
        }

        // Request a path to the position
        public void RequestPath(Vector2Int start, Vector2Int target, Action<List<Vector2Int>> callback, bool includeTarget = false)
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
            requestQueue.Enqueue(request);
        }

        private void Update()
        {
            // Execute finished path callbacks on the Unity thread.
            while (resultQueue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        private void WorkerLoop()
        {
            while (running)
            {
                // Requests contain plain data only, so the worker avoids touching Unity objects.
                if (requestQueue.TryDequeue(out PathRequest request))
                {
                    List<Vector2Int> path = AStar.FindPath(request);

                    // Add request callback to the result queue
                    resultQueue.Enqueue(() =>
                    {
                        request.callback?.Invoke(path);
                    });
                }

                Thread.Sleep(1);
            }
        }

        private void OnDestroy()
        {
            running = false;
            workerThread?.Join();
        }
    }
}
