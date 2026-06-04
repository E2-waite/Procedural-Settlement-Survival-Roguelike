using UnityEngine;
using static CommandSystem;

[CreateAssetMenu(fileName = "CommandTypeObject", menuName = "Scriptable Objects/CommandTypeObject")]
public class CommandTypeObject : ScriptableObject
{
    public Sprite icon;
    public Color color;
    public CommandType type;
}
