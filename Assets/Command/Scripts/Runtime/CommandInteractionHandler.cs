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
        private CommandOptionCatalog commandOptions = null;
        public CommandInteractionHandler(Context context)
        {
            widget = context.CommandWidget;
            commandOptions = context.CommandOptions;
        }

        public void Enable(InteractionTarget target, Vector2 mousePos)
        {
            Cursor.visible = false;

            List<CommandOption> options = new List<CommandOption>();
            options.Add(commandOptions.move);
            options.Add(commandOptions.attack);
            options.Add(commandOptions.gather);

            widget.Show(mousePos, options);
        }

        public void Disable()
        {
            CommandType commandType = widget.Hide();
            Debug.Log("Command Type: " + commandType);
            Cursor.visible = true;
        }

        #region Input
        public void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            widget.UpdateSelector(diff);
        }

        public void OnLeftDown()
        {
        }

        public void OnLeftHeld(Vector2 diff, float time)
        {
        }

        public void OnLeftUp(Vector2 diff, float time)
        {
            // Execute command, hide widget and switch state
            InteractionSystem.SelectHandler(HandlerType.Player);
        }

        public void OnRightDown()
        {

        }

        public void OnRightHeld(Vector2 pos, Vector2 diff, float time)
        {

        }

        public void OnRightUp(Vector2 diff, float time)
        {

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