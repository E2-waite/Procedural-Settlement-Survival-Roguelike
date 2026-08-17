using UnityEngine;
using Cinderwild.Gameplay.Agents;
using Cinderwild.Core.Data;

namespace Cinderwild.Core.Input
{
    public class PlayerInteractionHandler : IInteractionHandler
    {
        private InteractionController controller;
        private Player player;

        public PlayerInteractionHandler(Context context)
        {
            player = context.Player;
        }

        public void Enable()
        {
        }

        public void Disable()
        {
        }

        public void OnHover()
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
        }

        public void OnLeftUp(Vector2 diff, float time)
        { 
        }

        public void OnRightDown()
        {

        }
        public void OnRightHeld(Vector2 diff, float time)
        {
           
        }
        public void OnRightUp(Vector2 diff, float time)
        {
            
        }
        public void OnEscape()
        {
        }
        public void OnFKey()
        {
        }
        public void OnMoveInput(Vector2 move)
        {
            Debug.Log("Handler");
            player.OnMove(move);
        }

        public void OnLookChanged(Quaternion rot)
        {
        }

        public void OnLookDistChanged(float dist)
        {
        }
    }
}
