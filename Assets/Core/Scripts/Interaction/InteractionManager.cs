using Cinderwild.Command.Runtime;
using Cinderwild.Core;
using Cinderwild.World.Runtime;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public InteractionSystem System {  get; private set; }
    private InputManager input = null;

    public void Init(InputManager input, WorldManager world, PlayerInteractionHandler playerHandler, CommandInteractionHandler commandHandler)
    {
        this.input = input;
        System = new InteractionSystem(input, world, playerHandler, commandHandler);
    }

    private void OnDisable()
    {
        System?.Shutdown(input);
    }
}
