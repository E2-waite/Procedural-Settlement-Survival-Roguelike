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
    private Vector2Int ma;

    //void UpdateHighlight(Vector2Int selectSize)
    //{
    //    for (int x = 0; x < maxSize.x; x++)
    //    {
    //        for (int y = 0; y < maxSize.y; y++)
    //        {
    //            int i = GetVertexIndex(x, y);

    //            bool inside =
    //                x < selectSize.x &&
    //                y < selectSize.y;

    //            if (!inside)
    //            {
    //                SetColor(i, transparent);
    //                continue;
    //            }

    //            var tile = Grid.Instance.getTile(new Vector2Int(x, y));

    //            SetColor(i, tile.Buildable() ? validColour : invalidColour);
    //        }
    //    }

    //    mesh.colors = colors;
    //}

    public void HighlightTiles(Vector2Int highlightPos, Vector2Int selectSize)
    {
        if (selectSize.x > maxSize.x) maxSize.x = selectSize.x;
        if (selectSize.y > maxSize.y) maxSize.y = selectSize.y;

        bool valid = true;

        Vector2Int tilePos = new Vector2Int();
        for (int x = highlightPos.x; x < highlightPos.x + maxSize.x; x++)
        {
            for (int y = highlightPos.y; y < highlightPos.y + maxSize.y; y++)
            {
                tilePos.x = x; 
                tilePos.y = y;
                GridTile tile = Grid.Instance.getTile(tilePos);

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

                    if (tile == null || !tile.Buildable())
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
                GridTile tile = Grid.Instance.getTile(tilePos);

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
