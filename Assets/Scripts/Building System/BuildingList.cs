using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Stores buildable definitions and keeps the UI selection in sync with interaction mode.
public class BuildingList : MonoSingleton<BuildingList>
{
    [SerializeField] private List<BuildingObject> buildings = new List<BuildingObject>();
    private BuildingObject selected;
    public int NumBuildings => buildings.Count;
    public BuildingObject Selected => selected;

    public void Init()
    {
        // Building definitions are assigned in the inspector or refreshed in the editor.
        BuildPanel.Instance.UpdateDisplay(buildings);
    }

    public void SelectBuilding(int index)
    {
        if (index < buildings.Count)
        {
            selected = buildings[index];
            InteractionController.Instance.SetState(InteractionController.GameState.Build);
        }
            


    }

#if UNITY_EDITOR
    [ContextMenu("Refresh Building List")]
    public void RefreshBuildings()
    {
        // Editor-only helper: repopulates the inspector list from BuildingObject assets.
        string buildingsPath = "Assets/Buildings/Objects";

        buildings.Clear();
        string[] guids = AssetDatabase.FindAssets("t:BuildingObject", new[] { buildingsPath });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            BuildingObject obj = AssetDatabase.LoadAssetAtPath<BuildingObject>(path);

            if (obj != null)
                buildings.Add(obj);
        }


    }
#endif
}
