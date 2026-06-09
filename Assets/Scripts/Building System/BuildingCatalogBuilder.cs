#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class AgentCatalogBuilder
{
    [MenuItem("Tools/Refresh Buildings")]
    public static void Refresh()
    {
        string path = "Assets/ScriptableObjects/Buildings";

        var catalog = AssetDatabase.LoadAssetAtPath<BuildingCatalog>("Assets/ScriptableObjects/BuildingCatalog.asset");
        catalog.buildings.Clear();

        string[] guids = AssetDatabase.FindAssets("t:BuildingObject", new[] { path });

        foreach (var guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<BuildingObject>(assetPath);

            if (obj != null)
                catalog.buildings.Add(obj);
        }

        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
    }
}
#endif