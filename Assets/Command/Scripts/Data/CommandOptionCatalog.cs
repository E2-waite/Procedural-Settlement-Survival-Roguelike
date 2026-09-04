
using UnityEngine;

namespace Cinderwild.Command.Data
{
    [CreateAssetMenu(fileName = "CommandCatalog", menuName = "Scriptable Objects/CommandCatalog")]
    public class CommandOptionCatalog : ScriptableObject
    {
        public CommandOption move;
        public CommandOption attack;
        public CommandOption gather;
    }
}