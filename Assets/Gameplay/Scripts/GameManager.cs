using Cinderwild.Core;
using UnityEngine;

namespace Cinderwild.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Context context;
        public static Context Context;

        private void Start()
        {
            context.Input.Init();
            context.Input.EnableGameplayInput();
            context.World.Generate();
            context.Player.Init(context);
            context.Camera.Init(context);

            InteractionSystem.Init(context);

            Context = context;
        }
    }
}
