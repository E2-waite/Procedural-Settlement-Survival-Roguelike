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
        pathfinding = new Pathfinding();
        workerThread = new Thread(WorkerLoop);
        workerThread.Start();
    }

    void Update()
    {
        // Try to trigger the callback from the result queue (asynchronously)
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
            // Try to dequeue requests from the queue (if there are any) and find the path asynchronously
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
