using Cinderwild.Core;
using Cinderwild.Pathfinding.Runtime;
using UnityEngine;

namespace Cinderwild.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Context context;

        private void OnEnable()
        {
            context.Input.Init();
            context.Input.EnableGameplayInput();
            context.World.Generate();
            context.Player.Init(context);
            context.Agent.Init(context);
            context.Camera.Init(context);
            context.Pathfinding.Init();

            PathfindingSystem.Init(context);
            InteractionSystem.Init(context);
        }

        private void OnDisable()
        {
            context.Input.Shutdown();
        }
    }
}
