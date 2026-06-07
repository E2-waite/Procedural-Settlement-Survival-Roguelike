using UnityEngine;
using static CommandSystem;

[CreateAssetMenu(fileName = "CommandTypeObject", menuName = "Scriptable Objects/CommandType")]
public class CommandTypeObject : ScriptableObject
{
    public Sprite icon;
    public Color color;
    public CommandType type;
}
