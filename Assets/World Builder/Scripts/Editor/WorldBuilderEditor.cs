using UnityEditor;
using UnityEngine;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.WorldBuilder.Editor
{
    [CustomEditor(typeof(World))]
    public class WorldBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            World world = (World)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate World"))
            {
                Undo.RegisterFullObjectHierarchyUndo(
                    world.gameObject,
                    "Build Sprite Rig");

                world.Generate();

                EditorUtility.SetDirty(world);
            }
        }
    }
}
