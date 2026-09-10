using Cinderwild.Command.Data;
using Cinderwild.Command.UI;
using UnityEngine;

namespace Cinderwild.Command.Runtime
{
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
