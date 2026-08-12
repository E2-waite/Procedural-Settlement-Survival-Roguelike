using UnityEditor;
using UnityEngine;
using Cinderwild.World.Generation;

namespace Cinderwild.World.Editor
{
    [CustomEditor(typeof(WorldGenerator))]
    public class WorldBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WorldGenerator world = (WorldGenerator)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate World"))
            {
                Undo.RegisterFullObjectHierarchyUndo(
                    world.gameObject,
                    "Build Sprite Rig");

                world.GenerateWorld();

                EditorUtility.SetDirty(world);
            }
        }
    }
}
