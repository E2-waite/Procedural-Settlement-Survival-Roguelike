using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class AgentCatalog : ScriptableObject
{
    public AgentObject unit;
    public AgentObject worker;
    public AgentObject fighter;
    public AgentObject enemy;
}
