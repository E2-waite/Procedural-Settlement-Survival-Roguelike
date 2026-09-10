using Cinderwild.Command.Runtime;
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
        [SerializeField] private PathfindingManager pathfinding;
        [SerializeField] private CommandManager command;


        private void OnEnable()
        {
            input.Init();
            input.EnableGameplayInput();
            world.Generate();
            player.Init(world, camera);
            agent.Init(world, camera, pathfinding);
            camera.Init(player);
            pathfinding.Init(world.Data);
            command.Init();
            PlayerInteractionHandler playerHandler = new PlayerInteractionHandler(interaction, command, player, camera);
            CommandInteractionHandler commandHandler = new CommandInteractionHandler(interaction, command);
            interaction.Init(input, world, playerHandler, commandHandler);

            Destroy(this);
        }
    }
}
