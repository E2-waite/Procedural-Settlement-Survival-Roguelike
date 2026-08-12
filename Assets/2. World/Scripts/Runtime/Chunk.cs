using Cinderwild.World.Data;
using UnityEditor;
using UnityEngine;
using Cinderwild.World.Rendering;

namespace Cinderwild.World.Runtime
{
    [ExecuteAlways]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class Chunk : MonoBehaviour
    {
        public Mesh Mesh { get; set; }
        public MeshFilter MeshFilter { get; private set; }
        public MeshRenderer MeshRenderer { get; private set; }

        [SerializeField] private MeshFilter meshFilter;

        public ChunkData Data { get; private set; }

        public void Init(WorldProperties properties, Vector2 pos, Vector2Int gridPos)
        {
            Data = new ChunkData(properties.chunkSize, pos, gridPos);

            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (Data == null || Data.Resources == null)
                return;

            ResourceRenderer.Render(Data.Resources);

            sceneView.Repaint();
        }
#endif

        
    }
}