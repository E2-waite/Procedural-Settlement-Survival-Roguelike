using UnityEngine;
using static CommandSystem;

[CreateAssetMenu(fileName = "ActionTypeObject", menuName = "Scriptable Objects/ActionType")]
public class ActionTypeObject : ScriptableObject
{
    public Sprite icon;
    public Color color;
    public InteractType type;
}
