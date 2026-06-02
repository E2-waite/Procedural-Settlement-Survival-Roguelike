using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// Class for storing building objects and automatically initializing the build panel
public class BuildingList : MonoSingleton<BuildingList>
{
    [SerializeField] private List<BuildingObject> buildings = new List<BuildingObject>();
    private BuildingObject selected;
    public int NumBuildings => buildings.Count;
    public BuildingObject Selected => selected;

    public void Init()
    {
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
