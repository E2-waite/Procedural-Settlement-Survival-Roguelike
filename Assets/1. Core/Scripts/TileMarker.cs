using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TileMarker : MonoBehaviour
{
    public Color validColour, invalidColour;

    public GameObject tilePrefab;
    //MeshRenderer[,] markers = new MeshRenderer[3, 3];

    private Dictionary<Vector2Int, MeshRenderer> markers = new Dictionary<Vector2Int, MeshRenderer>();
    private Vector2Int maxSize = new Vector2Int(0, 0);
    WorldGrid grid;

    public void Init(GameContext context)
    {
        WorldBuilder world = context.world;
        grid = world.Context.grid;
    }

    public void HighlightTiles(Vector2Int highlightPos, Vector2Int selectSize, bool canAfford)
    {
        if (selectSize.x > maxSize.x) maxSize.x = selectSize.x;
        if (selectSize.y > maxSize.y) maxSize.y = selectSize.y;

        bool valid = true;

        if (!canAfford)
        {
            valid = false;
        }

        Vector2Int tilePos = new Vector2Int();
        for (int x = highlightPos.x; x < highlightPos.x + maxSize.x; x++)
        {
            for (int y = highlightPos.y; y < highlightPos.y + maxSize.y; y++)
            {
                tilePos.x = x; 
                tilePos.y = y;
                GridTile tile = grid.GetTile(tilePos);

                Vector2Int markerPos = new Vector2Int(x - highlightPos.x, y - highlightPos.y);
                MeshRenderer marker = null;

                if (markers.ContainsKey(markerPos))
                {
                    marker = markers[markerPos];
                }
                else
                {
                    GameObject tileObj = Instantiate(tilePrefab, transform);
                    tileObj.transform.localPosition = new Vector3(markerPos.x - 1, 0.001f, markerPos.y - 1);
                    marker = tileObj.GetComponent<MeshRenderer>();
                    markers[markerPos] = marker;
                }

                if (markerPos.x >= selectSize.x || markerPos.y >= selectSize.y)
                {
                    marker.gameObject.SetActive(false);
                }
                else
                {
                    marker.gameObject.SetActive(true);

                    if (tile == null || !tile.Buildable)
                    {
                        valid = false;
                    }
                }
            }
        }

        for (int x = highlightPos.x; x < highlightPos.x + maxSize.x; x++)
        {
            for (int y = highlightPos.y; y < highlightPos.y + maxSize.y; y++)
            {
                tilePos.x = x;
                tilePos.y = y;
                GridTile tile = grid.GetTile(tilePos);

                MeshRenderer marker = null;
                Vector2Int markerPos = new Vector2Int(x - highlightPos.x, y - highlightPos.y);

                if (markers.ContainsKey(markerPos))
                {
                    marker = markers[markerPos];
                }

                marker.material.color = (valid) ? validColour : invalidColour;
            }
        }


    }



}
