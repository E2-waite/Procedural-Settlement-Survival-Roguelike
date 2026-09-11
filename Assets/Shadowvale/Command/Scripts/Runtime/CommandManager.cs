using Shadowvale.Command.Data;
using Shadowvale.Command.UI;
using UnityEngine;

namespace Shadowvale.Command.Runtime
{
    /// <summary>
    /// Acts as the point of entry for interacting with the command system
    /// </summary>
    public class CommandManager : MonoBehaviour
    {
        public CommandSystem System { get; private set; }
        public CommandOptionCatalog Catalog => commandCatalog;
        public CommandWidget Widget => commandWidget;
        [SerializeField] private CommandOptionCatalog commandCatalog;
        [SerializeField] private CommandWidget commandWidget;
        public void Init()
        {
            System = new();
        }
    }
}
