using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Cinderwild.Pathfinding.Runtime
{
    // Pathfinding manager for handling pathfinding threads
    public class PathfindingManager : MonoBehaviour
    {
        private Thread workerThread;
        private bool running = true;

        private ConcurrentQueue<PathRequest> requestQueue = new ConcurrentQueue<PathRequest>();
        private ConcurrentQueue<System.Action> resultQueue = new ConcurrentQueue<System.Action>();

        public PathfindingManager Instance { get; private set; }
        
        public void Init()
        {
            // Pathfinding runs on a worker thread; callbacks are marshalled back in Update.
            workerThread = new Thread(WorkerLoop);
            workerThread.Start();
            Instance = this;
        }

        void Update()
        {
            // Execute finished path callbacks on the Unity thread.
            while (resultQueue.TryDequeue(out var action))
            {
                action?.Invoke();
            }
        }

        public void RequestPath(PathRequest request)
        {
            // Adds the request to the request queue
            requestQueue.Enqueue(request);
        }

        void WorkerLoop()
        {
            while (running)
            {
                // Requests contain plain data only, so the worker avoids touching Unity objects.
                if (requestQueue.TryDequeue(out PathRequest request))
                {
                    List<Vector2Int> path = Pathfinding.FindPath(request);

                    // Add request callback to the result queue
                    resultQueue.Enqueue(() =>
                    {
                        request.callback?.Invoke(path);
                    });
                }

                Thread.Sleep(1);
            }
        }

        void OnDestroy()
        {
            running = false;
            workerThread?.Join();
        }
    }
}
