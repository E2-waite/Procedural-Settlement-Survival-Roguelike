using System.Collections.Generic;
using Cinderwild.Command.Data;
using Cinderwild.Command.UI;
using Cinderwild.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cinderwild.Command.Runtime
{
    public class CommandInteractionHandler : IInteractionHandler
    {
        private CommandWidget widget = null;
        private CommandOptionCatalog commandCatalog = null;
        private InteractionTarget currentTarget = null;

        public CommandInteractionHandler(Context context)
        {
            widget = context.CommandWidget;
            commandCatalog = context.CommandCatalog;
        }

        // Display command widget
        public void Enable(InteractionTarget target, Vector2 mousePos)
        {
            currentTarget = new InteractionTarget(target);
            Cursor.visible = false;

            List<CommandOption> options = new List<CommandOption>();

            foreach (CommandOption option in commandCatalog.options)
            {
                if (option != null) options.Add(option);
            }

            widget.Show(mousePos, options);
        }

        // Execute command if there is one
        public void Disable()
        {
            CommandType commandType = widget.Hide();

            Cursor.visible = true;

            if (commandType != CommandType.None)
            {
                CommandSystem.ExecuteCommand(commandType, currentTarget);
            }
        }


        #region Input
        public void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            widget.UpdateSelector(diff);
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
            InteractionSystem.SelectHandler(HandlerType.Player);
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