using Cinderwild.WorldBuilder.Runtime;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cinderwild.WorldBuilder.Data
{
    [CreateAssetMenu(fileName = "WorldProperties", menuName = "Scriptable Objects/WorldProperties")]
    public class WorldProperties : ScriptableObject
    {
        public List<TileConfig> tileTypes = new List<TileConfig>();
        public ResourceConfig tree, stone;
        [Range(0.001f, .3f)] public float noiseScale = 0.05f;
        [Range(0.001f, .3f)] public float resourceScale = 0.05f;
        [Range(0.01f, 10f)] public float heightMultiplier = 2.5f;
        [Range(0.01f, 10f)] public float heightScale = 1f;
        [Range(0.01f, 2f)] public float tileScale = 1f;
        [Range(0.01f, 1f)] public float seaLevel = 0.35f;
        [Range(1, 100)] public int chunkSize = 20;
        [Range(1, 10)] public int streamDist = 2; 
        public Vector2Int worldSize = new Vector2Int(1, 1);

        public void Init()
        {
            for (int i = 0; i < tileTypes.Count; i++)
            {
                TileConfig prev = null;
                if (i > 0) prev = tileTypes[i - 1];
                tileTypes[i]?.Init(i, prev);
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
                World.OnPropertiesChanged?.Invoke();
            };
#else
        WorldGenerator.OnPropertiesChanged?.Invoke();
#endif
        }

    }
}
