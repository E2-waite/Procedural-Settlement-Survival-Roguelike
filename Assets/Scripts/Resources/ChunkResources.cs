using System.Collections.Generic;
using UnityEngine;
using static GridTile;

// Handles resource node placement and instanced rendering for one chunk.
public class ChunkResources
{
    private const int MAX_BATCH = 1023;
    public List<int> ids = new List<int>();

    private ResourceNode[] resources;
    private Matrix4x4[] matrices;
    private Matrix4x4[] batchBuffer = new Matrix4x4[MAX_BATCH];

    private int chunkSize;
    private WorldGrid grid;

    public Vector2 seedOffset;
    private Chunk chunk;
    private int count;
    private ResourceCatalog resourceCatalog;

    public ChunkResources(World world, Chunk chunk)
    {
        this.chunk = chunk;
        this.resourceCatalog = world.Context.resourceCatalog;

        chunkSize = world.Context.chunkSize;
        seedOffset = world.Context.seedOffset;
        grid = world.Context.grid;

        // Store matrices once at generation time; Render only batches visible resource instances.
        count = chunkSize * chunkSize;

        matrices = new Matrix4x4[count];
        resources = new ResourceNode[count];
        int index = 0;

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3 worldPos = new Vector3(chunk.position.x * chunkSize + x, 0, chunk.position.y * chunkSize + y);
                
                float noise = Mathf.PerlinNoise(
                    (worldPos.x + seedOffset.x + 1000) * 0.05f, 
                    (worldPos.z + seedOffset.y + 1000) * 0.05f);

                //if (noise > 0.5f)
                {
                    GridTile tile = grid.GetTile(worldPos);

                    ResourceObject resource = null;
                    float rand = Random.Range(0, 100);

                    // Resource type is currently derived from tile biome plus a random chance.
                    if (tile.type == TileType.Forest)
                    {
                        resource = resourceCatalog.tree;
                    }
                    else if (tile.type == TileType.Grass)
                    {
                        if (rand >= 90f)
                            resource = resourceCatalog.stone;
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
                resources[id].tile.Center,
                Quaternion.identity,
                Vector3.one * scale
            );
    }

    public void Render()
    {
        if (ids.Count == 0) return;

        // Unity's DrawMeshInstanced limit is 1023 matrices per draw call.
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
        obj.material.enableInstancing = true;


        Graphics.DrawMeshInstanced(
            obj.mesh,
            0,
            obj.material,
            batchBuffer,
            count
        );
    }
}
