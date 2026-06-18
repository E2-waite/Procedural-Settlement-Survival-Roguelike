using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class FormationCatalog : ScriptableObject
{
    public AgentFormation wedgeFormation;
    public AgentFormation ringFormation;
}