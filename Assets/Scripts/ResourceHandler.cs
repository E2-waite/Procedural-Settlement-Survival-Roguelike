using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ResourceHandler : MonoSingleton<ResourceHandler>
{
    public ResourceObject treeObj;
    public ResourceObject stoneObj;
    public int maxSearchRange = 10;

    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];

    private static readonly Vector2Int[] Directions =
    {
        new Vector2Int(1, 0),
        new Vector2Int(-1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(0, -1)
    };

    private void Start()
    {
        resourceCount[0] = 25;
        ResourcesPanel.Instance.UpdateCount(0, resourceCount[0]);

    }

    public int GetResourceCount(int type)
    {
        return resourceCount[type];
    }

    public void ConsumeResource(int type, int count)
    {
        resourceCount[type] -= count;

        ResourcesPanel.Instance.UpdateCount((ResourceNode.Type)type, resourceCount[type]);

    }

    // Searches for the closest neighbouring resource node
    public ResourceNode GetClosestNeighbour(ResourceNode node, bool sameType = true)
    {
        GridTile tile = node.gridTile;

        return ClosestNode(tile.position, node.type, sameType);
    }

    // Searches for the closest node to the passed store
    public ResourceNode GetClosestNode(ResourceStore store, bool sameType = true)
    {
        Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

        return ClosestNode(storePos, store.type, sameType);
    }

    ResourceNode ClosestNode(Vector2Int pos, ResourceNode.Type type, bool sameType)
    {
        Queue<(Vector2Int pos, int distance)> queue = new();

        HashSet<Vector2Int> visited = new();

        // Add the first tile to the queue
        queue.Enqueue((pos, 0));

        while (queue.Count > 0)
        {
            var (currentPos, distance) = queue.Dequeue();

            // Skip nodes outside of range
            if (distance > maxSearchRange) continue;

            ResourceNode currentNode = Grid.Instance.getTile(currentPos).GetResource();
            if (distance > 0 && currentNode != null && !currentNode.IsEmpty() &&
                ((sameType && currentNode.type == type) || !sameType))
            {
                // Return the current node if it exists and is not empty
                return currentNode;
            }

            foreach (Vector2Int dir in Directions)
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

    public void StoreResource(ResourceNode.Type type, int count)
    {
        resourceCount[(int)type] += count;

        ResourcesPanel.Instance.UpdateCount(type, resourceCount[(int)type]);
    }
}
