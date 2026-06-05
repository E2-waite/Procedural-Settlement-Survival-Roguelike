using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu]
public class ResourceDefs : MonoSingleton<ResourceDefs>
{
    public ResourceObject tree;
    public ResourceObject stone;
}
