using System.Collections.Generic;
using System.Resources;
using Mono.Cecil;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static GridTile;
using static UnityEngine.Rendering.DebugUI;

// Handles generation and rendering of resources for a chunk
public class ChunkResources
{
    private const int MAX_BATCH = 1023;

    public List<int> ids = new List<int>();

    private ResourceNode[] resources;
    private Matrix4x4[] matrices;
    private Matrix4x4[] batchBuffer = new Matrix4x4[MAX_BATCH];

    private Chunk thischunk;
    private int count;


    public ChunkResources(Chunk chunk)
    {
        //return;
        thischunk = chunk;

        count = chunk.size * chunk.size;

        matrices = new Matrix4x4[count];
        resources = new ResourceNode[count];
        int index = 0;

        for (int x = 0; x < chunk.size; x++)
        {
            for (int y = 0; y < chunk.size; y++)
            {
                Vector3 worldPos = new Vector3(chunk.position.x * chunk.size + x, 0, chunk.position.y * chunk.size + y);
                float noise = Mathf.PerlinNoise((worldPos.x + WorldHandler.Instance.seedOffset.x + 1000) * 0.05f, (worldPos.z + WorldHandler.Instance.seedOffset.y + 1000) * 0.05f);
                //if (noise > 0.5f)
                {
                    GridTile tile = WorldHandler.grid.GetTile(worldPos);

                    ResourceObject resource = null;
                    float rand = Random.Range(0, 100);


                    if (tile.type == TileType.Forest)
                    {
                        resource = ResourceHandler.Instance.treeObj;
                    }
                    else if (tile.type == TileType.Grass)
                    {

                        if (rand >= 90f)
                            resource = ResourceHandler.Instance.stoneObj;
                    }

                    if (resource != null)
                    {
                        resources[index] = new ResourceNode(index, worldPos, resource.type, tile, resource);
                        ids.Add(index);
                        UpdateMatrix(index);
                        index++;
                    }

                }
            }
        }
    }
    void UpdateMatrix(int id)
    {
        float scale = 1f;

        ResourceNode resource = resources[id];

        matrices[id] =
            Matrix4x4.TRS(
                resources[id].tile.Center(),
                Quaternion.identity,
                Vector3.one * scale
            );
    }

    public void Render()
    {
        if (ids.Count == 0) return;

        int batchCount = 0;
        ResourceObject lastObj= null;

        for (int i = 0; i < ids.Count; i++)
        {
            int id = ids[i];
            ResourceNode resource = resources[id];

            // Don't render if it's been gathered
            if (resource.IsEmpty())
            {
                continue;
            }

            //if (lastObj == null)
            //    lastObj = resource.resourceObj;

            if (lastObj != null && (resource.type != lastObj.type || batchCount == MAX_BATCH))
            {
                DrawBatch(batchCount, lastObj);
                batchCount = 0;
            }

            batchBuffer[batchCount++] = matrices[id];
            lastObj = resource.resourceObj;
        }

        if (batchCount > 0 && lastObj != null)
        {
            DrawBatch(batchCount, lastObj);
        }
    }

    void DrawBatch(int count, ResourceObject obj)
    {
        Graphics.DrawMeshInstanced(
            obj.mesh,
            0,
            obj.material,
            batchBuffer,
            count
        );
    }
}
