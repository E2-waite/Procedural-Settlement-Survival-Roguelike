using UnityEditor;
using UnityEngine;
using Cinderwild.World.Runtime;

namespace Cinderwild.World.Editor
{
    [CustomEditor(typeof(Runtime.WorldManager))]
    public class WorldBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            Runtime.WorldManager world = (Runtime.WorldManager)target;

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
