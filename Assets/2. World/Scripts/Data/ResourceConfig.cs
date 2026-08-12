using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cinderwild.World.Generation;

namespace Cinderwild.World.Data
{
    [System.Serializable]
    public class ResourceSize
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private float scale = 1f;
        public Mesh Mesh => mesh;
        public float Scale => scale;
    }

    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "World Builder/Resource")]
    public class ResourceConfig : ScriptableObject
    {
        [Range(0.1f, 10f)] public float scale = 1f;
        [Range(0f, 25f)] public float width = 1f;
        [Range(0f, 25f)] public float height = 1f;
        [Range(0f, 1f)] public float scaleVariation = .1f;
        [Range(0f, 1f)] public float widthVariation = .1f;
        [Range(0f, 1f)] public float heightVariation = .1f;

        [Range(0f, 1f)] public float offsetVariation = .1f;
        [Range(0f, 1f)] public float rate = .5f;
        [Range(0f, 1f)] public float noiseThresh = .5f;
        [Range(0f, 10000f)] public float noiseOffset = 250f;

        [SerializeField] public List<ResourceSize> sizes = new List<ResourceSize>(); 
        public List<Material> materials = new List<Material>();

        private void OnValidate()
        {
#if UNITY_EDITOR
            EditorApplication.delayCall += () =>
            {
                WorldGenerator.OnPropertiesChanged?.Invoke();
            };
#else
        WorldGenerator.OnPropertiesChanged?.Invoke();
#endif
        }
    }
}
