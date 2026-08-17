using Cinderwild.Core.Data;
using Cinderwild.Core.Input;
using UnityEngine;

namespace Cinderwild.Gameplay
{
    public class Bootstrapper : MonoBehaviour
    {
        public Context context;

        private void Start()
        {
            context.Input.Init();
            context.Input.EnableGameplayInput();
            context.Interaction.Init(context);
            context.World.Generate();
            context.Player.Init(context);
            Destroy(this);
        }
    }
}
