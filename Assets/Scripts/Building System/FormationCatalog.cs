using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class FormationCatalog : ScriptableObject
{
    public AgentFormation meleeCommand;
    public AgentFormation meleeFollow;
    public AgentFormation rangedCommand;
    public AgentFormation rangedFollow;
}