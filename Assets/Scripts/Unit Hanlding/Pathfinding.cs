using UnityEngine;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using System;
public class Pathfinding
{
    public struct PathRequest
    {
        public int size;
        public Vector2Int start;
        public Vector2Int target;
        public Vector2Int origin;
        public bool[,] pathable;
        public Action<List<Vector2Int>> callback;
    }

    private class Node
    {
        public Vector2Int pos;
        public int gCost;
        public int hCost;
        public int fCost => gCost + hCost;
        public Node parent;
    }


    public List<Vector2Int> FindPath(PathRequest request)
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<Vector2Int>();

        Vector2Int startLocal = request.start - request.origin;
        Vector2Int targetLocal = request.target - request.origin;

        Node startNode = new Node
        {
            pos = startLocal,
            gCost = 0,
            hCost = GetDist(startLocal, targetLocal)
        };

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node current = GetLowestFCost(openSet);

            // Reverse and return the path if we have reached the target
            if (current.pos == targetLocal)
            {
                List<Vector2Int> path = RetracePath(current);

                for (int i = 0; i < path.Count; i++)
                {
                    path[i] += request.origin;
                }

                return path;
            }

            openSet.Remove(current);
            closedSet.Add(current.pos);

            foreach (Vector2Int neighbourPos in GetNeighbours(current.pos))
            {
                if (neighbourPos.x >= request.size || neighbourPos.x < 0 || 
                    neighbourPos.y >= request.size || neighbourPos.y < 0 ||
                    (neighbourPos != targetLocal && !request.pathable[neighbourPos.x, neighbourPos.y]))
                    continue;

                // Skip closed positions
                if (closedSet.Contains(neighbourPos)) continue;

                int tentativeG = current.gCost + GetDist(current.pos, neighbourPos);

                // TODO: replace this as it is expensive
                Node existing = openSet.Find(n => n.pos == neighbourPos);

                if (existing == null)
                {
                    // If this node doesn't exist, create it
                    Node neighbourNode = new Node
                    {
                        pos = neighbourPos,
                        gCost = tentativeG,
                        hCost = GetDist(neighbourPos, targetLocal),
                        parent = current
                    };

                    openSet.Add(neighbourNode);
                }
                else if (tentativeG < existing.gCost)
                {
                    existing.gCost = tentativeG;
                    existing.parent = current;
                }
            }
        }

        return null; // No path found
    }

    public List<Vector2Int> GetNeighbours(Vector2Int pos)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                if (x == pos.x && y == pos.y)
                    continue;

                neighbours.Add(new Vector2Int(x, y));
            }
        }

        return neighbours;
    }

    private List<Vector2Int> RetracePath(Node endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Node current = endNode;

        while (current != null)
        {
            path.Add(current.pos);
            current = current.parent;
        }

        path.Reverse();

        return path;
    }

    // Gets the cheapest node from the open set
    private Node GetLowestFCost(List<Node> nodes)
    {
        Node bestNode = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (nodes[i].fCost < bestNode.fCost ||
                (nodes[i].fCost == bestNode.fCost && nodes[i].hCost < bestNode.hCost))
            {
                bestNode = nodes[i];
            }
        }

        return bestNode;
    }

    private int GetDist(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);

        return Mathf.Max(dx, dy);
    }

}
