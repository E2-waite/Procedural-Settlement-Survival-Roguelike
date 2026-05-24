using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    public enum WorkerState
    {
        Idle,
        Moving,
        Gathering,
        Storing
    }

    public WorkerState workerState;

    public int currentCapacity = 0, maxCapacity = 100;

    ResourceNode targetNode;

    float gatherInterval = 0.5f, gatherTimer = 0;

    protected override void Start()
    {
        base.Start();

        workerState = WorkerState.Idle;
    }

    private void Update()
    {
        if (workerState == WorkerState.Moving)
        {
            //if (!HasPath())
            //    workerState = WorkerState.Idle;
            //else
                FollowPath();
        }
        else if (workerState == WorkerState.Gathering)
        {
            if (gatherTimer > 0) gatherTimer -= Time.deltaTime;

            if (targetNode == null)
            {
                workerState = WorkerState.Idle;
            }
            else
            {
                float dist = Vector3.Distance(transform.position, targetNode.worldPosition);

                // Move towards resource if not in range
                if (dist > 1.25f)
                {
                    //Debug.Log("Following Path... Dist = " + dist);
                    FollowPath();
                }
                else
                {
                    if (gatherTimer <= 0)
                    {
                        // Gather if in range and timer has finished
                        Gather();
                    }
                }
            }
        }
    }

    public override void Command(GridTile tile)
    {
        if (tile == null) return;

        if (tile.HasResource())
        {
             // Command to gather if tile has resource
            Debug.Log("Commanding to gather!");
            ResourceNode resource = tile.GetResource();

            if (resource != null && !resource.IsEmpty())
            {
                targetNode = resource;
                workerState = WorkerState.Gathering;

                Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                RequestPath(currentPos, tile.position, resource.worldPosition);
            }    
        }
        else if (tile.HasBuilding())
        {
            // Interact with building if tile has one
        }
        else
        {
            // Move to tile if empty
            Debug.Log("Commanding to move!");
            pathRequested = false;
            currentPath.Clear();
            workerState = WorkerState.Moving;

            Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

            RequestPath(currentPos, tile.position, tile.worldPosition);
        }
    }

    public void Gather()
    {
        if (currentCapacity >= maxCapacity)
        {
            workerState = WorkerState.Storing;
        }
        else
        {
            currentCapacity += targetNode.Gather(5);
            gatherTimer = gatherInterval;

            if (targetNode.IsEmpty())
                targetNode = null;
        }
    }
}
