#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine;
using static GlobalDefs;

[CustomEditor(typeof(AgentObject))]
public class AgentObjectEditor : Editor
{
    private Texture2D torsoTrimSheet;
    private Texture2D eyesSheet;
    private Texture2D helmetSheet;
    private Texture2D helmetTrimSheet;
    private Texture2D frontHandSheet;
    private Texture2D backHandSheet;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Auto Assign Sprite Sheets", EditorStyles.boldLabel);

        torsoTrimSheet = DrawSheetField("Torso Trim", torsoTrimSheet);
        eyesSheet = DrawSheetField("Eyes", eyesSheet);
        helmetSheet = DrawSheetField("Helmet", helmetSheet);
        helmetTrimSheet = DrawSheetField("Helmet Trim", helmetTrimSheet);
        frontHandSheet = DrawSheetField("Front Hand", frontHandSheet);
        backHandSheet = DrawSheetField("Back Hand", backHandSheet);


        if (GUILayout.Button("Assign Sprite Sheets"))
        {

            AssignArray("torsoTrim", torsoTrimSheet);
            AssignArray("eyes", eyesSheet);
            AssignArray("helmet", helmetSheet);
            AssignArray("helmetTrim", helmetTrimSheet);
            AssignArray("frontHand", frontHandSheet);
            AssignArray("backHand", backHandSheet);

            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
            AssetDatabase.SaveAssets();
        }
    }

    private Texture2D DrawSheetField(string label, Texture2D current)
    {
        return (Texture2D)EditorGUILayout.ObjectField(
            label,
            current,
            typeof(Texture2D),
            false
        );
    }

    private void AssignArray(string propertyName, Texture2D sheet)
    {
        if (sheet == null) return;

        Sprite[] sprites = LoadSprites(sheet);
        Debug.Log("Assigning  " + propertyName);
        SerializedProperty arrayProperty = serializedObject.FindProperty(propertyName);
        arrayProperty.arraySize = (int)SpriteDir.Max;

        for (int i = 0; i < (int)SpriteDir.Max; i++)
        {
            SerializedProperty element = arrayProperty.GetArrayElementAtIndex(i);
            element.objectReferenceValue = i < sprites.Length ? sprites[i] : null;
        }
    }

    private Sprite[] LoadSprites(Texture2D sheet)
    {
        string path = AssetDatabase.GetAssetPath(sheet);

        return AssetDatabase
            .LoadAllAssetRepresentationsAtPath(path)
            .OfType<Sprite>()
            .ToArray();
    }
}
#endif