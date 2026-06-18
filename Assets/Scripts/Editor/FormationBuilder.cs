#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AgentFormation))]
public class FormationBuilder : Editor
{
    public override void OnInspectorGUI()
    {
        AgentFormation formation = (AgentFormation)target;

        formation.width = EditorGUILayout.IntField("Width", formation.width);
        formation.height = EditorGUILayout.IntField("Height", formation.height);
        formation.spacing = EditorGUILayout.IntField("Spacing", formation.spacing);

        formation.width = Mathf.Max(1, formation.width);
        formation.height = Mathf.Max(1, formation.height);

        int size = formation.width * formation.height;

        if (formation.slots == null || formation.slots.Length != size)
        {
            int[] newSlots = new int[size];

            if (formation.slots != null)
            {
                for (int i = 0; i < Mathf.Min(formation.slots.Length, size); i++)
                    newSlots[i] = formation.slots[i];
            }

            formation.slots = newSlots;
        }

        EditorGUILayout.Space();

        for (int y = 0; y < formation.height; y++)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < formation.width; x++)
            {
                int index = y * formation.width + x;
                formation.slots[index] = EditorGUILayout.IntField(formation.slots[index], GUILayout.Width(30));
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(formation);
        }
    }
}
#endif