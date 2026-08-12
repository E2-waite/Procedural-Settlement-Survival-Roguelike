using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Consts;
public class ConvertBuilding : Building
{
    [SerializeField] public float convertTime = 5f; // Seconds
    [SerializeField] private int maxCapacity = 3;
    protected int activeConversions = 0;
    private CommandSystem commandSystem;
    protected virtual IUnitRole Role => null;
    public virtual bool SameType(IUnitRole unitRole) => false;
    WorldGrid grid;
    public override void Init(GameContext context) 
    {
        commandSystem = context.commandSystem;
        WorldBuilder world = context.world;
        grid = world.Context.grid;
    }

    public virtual bool Convert(Unit unit)
    {
        if (Role == null) return false;

        if (activeConversions < maxCapacity)
        {
            activeConversions++;
            StartCoroutine(ConvertWorker(unit));
            return true;
        }
        else return false;
    }
    protected virtual IEnumerator ConvertWorker(Unit unit)
    {
        Debug.Log("Converting unit");
        if (unit == null)
        {
            activeConversions--;
            yield break;
        }

        // Disable worker
        commandSystem.StopCommanding(unit);
        unit.gameObject.SetActive(false);
        unit.SetState(Unit.State.Idle);
        yield return new WaitForSeconds(convertTime);
        unit.SetRole(Role.New());
        unit.gameObject.SetActive(true);
        activeConversions--;

        // TODO: stop units spawning in the same position
       // unit.transform.position = FindTile().Center;
    }

    GridTile FindTile()
    {
        // Breadth-first search gives the closest node in grid steps, capped by maxSearchRange.
        Queue<(Vector2Int pos, int distance)> queue = new();

        HashSet<Vector2Int> visited = new();

        // Add the first tile to the queue
        foreach (GridTile tile in tiles)
        {
            queue.Enqueue((tile.position, 0));
        }
        

        while (queue.Count > 0)
        {
            var (currentPos, distance) = queue.Dequeue();

            // Skip nodes outside of range
            if (distance > 5) continue;

            GridTile currentTile = grid.GetTile(currentPos);

            if (currentTile == null) continue;

            if (distance > 1)
            {
                // Return the current node if it exists and is not empty
                return currentTile;
            }

            List<Vector2Int> directions = new(ADJ_NEIGHBOURS);

            // Fisher-Yates shuffle
            for (int i = directions.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (directions[i], directions[j]) = (directions[j], directions[i]);
            }

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbour = currentPos + dir;

                // Skip visited nodes
                if (visited.Contains(neighbour)) continue;

                visited.Add(neighbour);

                // Add the next neighbour to the queue
                queue.Enqueue((neighbour, distance + 1));
            }
        }

        // No resource found
        return null;
    }
}
