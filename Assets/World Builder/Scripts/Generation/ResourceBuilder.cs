using Cinderwild.WorldBuilder.Data;
using Cinderwild.WorldBuilder.Runtime;
using UnityEngine;

namespace Cinderwild.WorldBuilder.Generation
{
    public static class ResourceBuilder
    {
        private static WorldProperties properties;

        private static bool initialized = false;
        public static void Init(World builder)
        {
            if (!initialized)
            {
                properties = builder.Properties;
                initialized = true;
            }
        }

        public static void Build(ChunkData chunk)
        {
            ChunkResources resources = new ChunkResources(chunk);

            int index = 0;

            for (int x = 0; x < chunk.Size; x++)
            {
                for (int z = 0; z < chunk.Size; z++)
                {
                    // Offset by 1 to account for padding
                    TileData tile = chunk.GetTile(x + 1, z + 1);

                    if (tile.Object.resourceTypes.Count == 0) continue;



                    foreach (ResourceConfig config in tile.Object.resourceTypes)
                    {
                        if (config == null) continue;

                        float noise = Noise.GetNoise(x - 1 + chunk.Position.x, z - 1 + chunk.Position.y, properties.resourceScale, config.noiseOffset);


                        if (noise > config.noiseThresh)
                        {
                            float rand = Random.Range(0f, 1f);

                            //float thresh = 1f - resourceConfig.rate;

                            if (config.sizes.Count > 0)
                            {
                                ResourceNode node = new ResourceNode(config, tile, index, Random.Range(0, config.sizes.Count));
                                resources.SetNode(index, node);
                                resources.ids.Add(index);
                                UpdateMatrix(resources, node, tile, index);
                                index++;
                                break;
                            }
                        }
                    }
                }
            }

            chunk.Resources = resources;
        }

        private static void UpdateMatrix(ChunkResources resources, ResourceNode node, TileData tile, int id)
        {
            ResourceConfig config = node.Config;
            if (config == null) return;

            float heightVariation = config.heightVariation * config.height;
            float widthVariation = config.widthVariation * config.width;
            float scaleVariation = config.scaleVariation * config.scale;

            float height = Random.Range(config.height - heightVariation, config.height + heightVariation);
            float width = Random.Range(config.width - widthVariation, config.width + widthVariation);
            Vector3 scale = new Vector3(width, height, width) * Random.Range(config.scale - scaleVariation, config.scale + scaleVariation);


            scale *= properties.tileScale; // Scale to match tile scale
            scale *= node.Config.sizes[node.nodeSize].Scale; // Scale to match node size's scale

            Vector3 pos = tile.Center;
            Vector3 offset = Vector3.zero;
            offset.x = Random.Range(-config.offsetVariation, config.offsetVariation);
            offset.z = Random.Range(-config.offsetVariation, config.offsetVariation);
            offset *= properties.tileScale;
            pos += offset;

            Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            resources.Matrices[id] =
                Matrix4x4.TRS(
                    pos,
                    rotation,
                    scale
                );
        }
    }
}
