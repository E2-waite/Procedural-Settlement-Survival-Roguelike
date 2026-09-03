using UnityEngine;
using Cinderwild.Command.Runtime;

namespace Cinderwild.Command.Data
{
    [CreateAssetMenu(fileName = "CommandOption", menuName = "Scriptable Objects/CommandOption")]
    public class CommandOption : ScriptableObject
    {
        public Sprite sprite;
        public CommandType commandType;
    }
}