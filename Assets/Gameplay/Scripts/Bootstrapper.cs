using Cinderwild.Command.Data;
using Cinderwild.Command.Runtime;
using Cinderwild.Command.UI;
using Cinderwild.Core;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;
using Cinderwild.World.Runtime;
using UnityEngine;

namespace Cinderwild.Gameplay
{
    public class Bootstrapper : MonoBehaviour
    {
        [SerializeField] private WorldManager world;
        [SerializeField] private InputManager input;
        [SerializeField] private InteractionManager interaction;
        [SerializeField] private Player player;
        [SerializeField] private Agent agent;
        [SerializeField] private CameraController camera;
        [SerializeField] private CommandOptionCatalog commandCatalog;
        [SerializeField] private CommandWidget commandWidget;
        [SerializeField] private PathfindingManager pathfinding;


        private void OnEnable()
        {
            input.Init();
            input.EnableGameplayInput();
            world.Generate();
            player.Init(world.System, camera);
            agent.Init(world.System, camera, pathfinding);
            camera.Init(player);
            pathfinding.Init(world.Data);

            PlayerInteractionHandler playerHandler = new PlayerInteractionHandler(interaction, player, camera);
            CommandInteractionHandler commandHandler = new CommandInteractionHandler(interaction, commandCatalog, commandWidget);
            interaction.Init(input, world, playerHandler, commandHandler);

            Destroy(this);
        }
    }
}
