using UnityEditor;
using UnityEngine;

namespace Haztech.WorldBuilder.Editor
{
    [CustomEditor(typeof(global::WorldBuilder))]
    public class WorldBuilderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            global::WorldBuilder world = (global::WorldBuilder)target;

            EditorGUILayout.Space();

            if (GUILayout.Button("Generate World"))
            {
                Undo.RegisterFullObjectHierarchyUndo(
                    world.gameObject,
                    "Build Sprite Rig");

                world.Generate(world.gameContext);

                EditorUtility.SetDirty(world);
            }
        }
    }
}
