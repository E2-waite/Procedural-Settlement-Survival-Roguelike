using UnityEngine;
using Cinderwild.Gameplay.Agents;
using UnityEngine.InputSystem;

namespace Cinderwild.Core
{
    public class PlayerInteractionHandler : IInteractionHandler
    {
        private Player player;
        private CameraController camera;
        public PlayerInteractionHandler(Context context)
        {
            player = context.Player;
            camera = context.Camera;
        }

        public void Enable(InteractionTarget target, Vector2 mousePos)
        {
        }

        public void Disable()
        {
        }

        public void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
        }

        public void OnLeftDown()
        {
        }

        public void OnLeftHeld(Vector2 diff, float time)
        {
            if (time > .1f)
                InteractionSystem.SelectHandler(HandlerType.Command);
        }

        public void OnLeftUp(Vector2 diff, float time)
        { 
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
            if (key == Key.Comma) // '<'
            {
                camera.Rotate(-1);
            }
            else if (key == Key.Period) // '>'
            {
                camera.Rotate(1);
            }
        }

        public void OnKeyReleased(Key key)
        {

        }

        public void OnMoveInput(Vector2 move)
        {
            player?.Controller?.MovePlayer(move);
        }
    }
}
