using UnityEngine;
using Cinderwild.Command.Runtime;

namespace Cinderwild.Command.Data
{
    /// <summary>
    /// Defines a command option for use by the command widget
    /// </summary>
    [CreateAssetMenu(fileName = "CommandOption", menuName = "Scriptable Objects/CommandOption")]
    public class CommandOption : ScriptableObject
    {
        public Sprite sprite;
        public Color color;
        public CommandType commandType;
    }
}