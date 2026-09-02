
using Cinderwild.Command.UI;
using Cinderwild.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Cinderwild.Command.Runtime
{
    public class CommandInteractionHandler : IInteractionHandler
    {
        private CommandWidget widget = null;

        public CommandInteractionHandler(Context context)
        {
            widget = context.CommandWidget;
        }


        public void Enable(InteractionTarget target, Vector2 mousePos)
        {
            Debug.Log("Enabled Command Handler");
            Cursor.visible = false;
            widget.Show(mousePos);
        }

        public void Disable()
        {
            widget.Hide();
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