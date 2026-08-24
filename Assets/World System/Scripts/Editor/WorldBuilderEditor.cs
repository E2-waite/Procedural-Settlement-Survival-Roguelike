using UnityEditor;
using UnityEngine;
using Cinderwild.WorldBuilder.Runtime;

namespace Cinderwild.WorldBuilder.Editor
{
    [CustomEditor(typeof(Runtime.WorldBuilder))]
    public class WorldBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Runtime.WorldBuilder world = (Runtime.WorldBuilder)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate World"))
            {
                Undo.RegisterFullObjectHierarchyUndo(
                    world.gameObject,
                    "Build World");

                world.Generate();

                EditorUtility.SetDirty(world);
            }

            if (GUILayout.Button("Clear World"))
            {
                Undo.RegisterFullObjectHierarchyUndo(
                    world.gameObject,
                    "Clear World");

                world.Clear();

                EditorUtility.SetDirty(world);
            }
        }
    }
}
