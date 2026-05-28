using UnityEngine;

[CreateAssetMenu(fileName = "ResourceObject", menuName = "Scriptable Objects/ResourceObject")]
public class ResourceObject : ScriptableObject
{
    public ResourceNode.Type type;
    public Mesh mesh;
    public Material material;
}
