using UnityEngine;
using System.Threading;
using static Pathfinding;
using System.Collections.Concurrent;
using System.Collections.Generic;


public class PathfindingHandler : MonoSingleton<PathfindingHandler>
{
    Pathfinding pathfinding;
    private Thread workerThread;
    private bool running = true;

    private ConcurrentQueue<PathRequest> requestQueue = new ConcurrentQueue<PathRequest>();
    private ConcurrentQueue<System.Action> resultQueue = new ConcurrentQueue<System.Action>();

    protected override void Awake()
    {
        base.Awake();

        // Pathfinding runs on a worker thread; callbacks are marshalled back in Update.
        pathfinding = new Pathfinding();
        workerThread = new Thread(WorkerLoop);
        workerThread.Start();
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
                List<Vector2Int> path = pathfinding.FindPath(request);

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
