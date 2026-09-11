using System.Collections.Generic;
using Shadowvale.Command.Data;
using Shadowvale.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Shadowvale.Command.Runtime
{
    /// <summary>
    /// Handles interaction with the command widget and execution of commands when in command state
    /// </summary>
    public class CommandInteractionHandler : IInteractionHandler
    {
        private InteractionManager interaction;
        private CommandManager command;

        private InteractionTarget currentTarget = null;


        public CommandInteractionHandler(InteractionManager interaction, CommandManager command)
        {
            this.interaction = interaction;
            this.command = command;
        }

        // Display command widget
        public void Enable(InteractionTarget target, Vector2 mousePos)
        {
            currentTarget = new InteractionTarget(target);
            Cursor.visible = false;

            List<CommandOption> options = new List<CommandOption>();

            foreach (CommandOption option in command.Catalog.options)
            {
                if (option != null) options.Add(option);
            }

            command.Widget.Show(mousePos, options);
        }

        // Execute command if there is one
        public void Disable()
        {
            CommandType commandType = command.Widget.Hide();

            Cursor.visible = true;

            if (commandType != CommandType.None)
            {
                command.System.ExecuteCommand(commandType, currentTarget);
            }
        }


        #region Input
        public void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            command.Widget.UpdateSelector(diff);
        }

        public void OnLeftDown(InteractionTarget target)
        {
        }

        public void OnLeftHeld(Vector2 diff, float time)
        {
        }

        public void OnLeftUp(Vector2 diff, float time)
        {

        }

        public void OnRightDown(InteractionTarget target)
        {

        }

        public void OnRightHeld(Vector2 pos, Vector2 diff, float time)
        {

        }

        public void OnRightUp(Vector2 diff, float time)
        {
            // Execute command, hide widget and switch state
            interaction.System.SelectHandler(HandlerType.Player);
        }

        public void OnKeyPressed(Key key)
        {
        }

        public void OnKeyReleased(Key key)
        {

        }

        public void OnMoveInput(Vector2 move)
        {
        }
        #endregion
    }
}