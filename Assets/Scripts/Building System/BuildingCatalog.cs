using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class BuildingCatalog : ScriptableObject
{
    public List<BuildingObject> buildings = new List<BuildingObject>();
}
