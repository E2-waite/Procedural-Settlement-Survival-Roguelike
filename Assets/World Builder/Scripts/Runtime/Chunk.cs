using UnityEditor;
using UnityEngine;
using Cinderwild.WorldBuilder.Rendering;
using Cinderwild.WorldBuilder.Data;

namespace Cinderwild.WorldBuilder.Runtime
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

        public void Init(WorldProperties properties, Vector2Int pos)
        {
            Vector2 worldPos = new Vector2(pos.x * properties.chunkSize, pos.y * properties.chunkSize);

            Data = new ChunkData(properties.chunkSize, worldPos, pos);

            MeshFilter = GetComponent<MeshFilter>();
            MeshRenderer = GetComponent<MeshRenderer>();
            transform.position = new Vector3(worldPos.x, 0, worldPos.y);
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

            //ResourceRenderer.Render(Data.Resources);

            sceneView.Repaint();
        }
#endif

        
    }
}