using UnityEngine;

[CreateAssetMenu(fileName = "ResourceObject", menuName = "Scriptable Objects/Resource")]
public class ResourceObject : ScriptableObject
{
    public ResourceNode.Type type;
    public Mesh mesh;
    public Material material;
    public Material outlineMaterial;
    public Material outlineMask;
}
