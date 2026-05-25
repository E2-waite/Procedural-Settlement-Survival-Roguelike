using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    // YOU MUST ENSURE CONSISTENCY 0 = idle, 1 = moving, 2 = following
    public enum WorkerState
    {
        Idle,
        Moving,
        following, 
        Gathering,
        Storing
    }

    public WorkerState workerState;

    public int currentCapacity = 0, maxCapacity = 100;

    ResourceNode targetResource;

    float gatherInterval = 0.5f, gatherTimer = 0;

    protected override void Start()
    {
        base.Start();
        workerState = WorkerState.Idle;
    }

    protected override int GetState()
    {
        return (int)workerState;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override bool HandleStates()
    {
        bool handled = false;

        if (workerState == WorkerState.Moving)
        {
            FollowPath();
            handled = true;
        }
        else if (workerState == WorkerState.Gathering)
        {
            if (gatherTimer > 0) gatherTimer -= Time.deltaTime;

            if (targetResource == null)
            {
                workerState = WorkerState.Idle;
            }
            else
            {
                float dist = Vector3.Distance(transform.position, targetResource.worldPosition);

                // Move towards resource if not in range
                if (dist > 1.25f)
                {
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
            handled = true;
        }

        return handled;
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
                targetResource = resource;
                workerState = WorkerState.Gathering;

                Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                RequestPath(currentPos, tile.position, resource.worldPosition);
            }    
        }
        else if (tile.HasBuilding())
        {
            // Interact with building if tile has one
            
            // TODO: implement building interaction

            // If building is broken, repair

            // If building hasn't finished building, build

            // If building is repaired and built, interact

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

    public override void StartFollowing(PlayerController thePlayer)
    {
        base.StartFollowing(thePlayer);

        workerState = WorkerState.following;
    }

    public void Gather()
    {
        if (currentCapacity >= maxCapacity)
        {
            // TODO: store resources
            workerState = WorkerState.Storing;
        }
        else
        {
            // TODO: move to next resource 
            currentCapacity += targetResource.Gather(5);
            gatherTimer = gatherInterval;

            if (targetResource.IsEmpty())
                targetResource = null;
        }
    }
}
