using UnityEngine;
using UnityEngine.InputSystem;
using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;
using Cinderwild.Command.Runtime;

namespace Cinderwild.Core
{
    public enum HandlerType
    {
        Player,
        Command,
        Max
    }

    public class InteractionSystem
    {
        private WorldManager world;

        private bool initialized = false;
        private Vector2 mousePos; // The current mouse position
        private HandlerType selectedHandler = HandlerType.Player;

        private IInteractionHandler[] handlers = new IInteractionHandler[(int)HandlerType.Max];
        private InteractionTarget currentTarget = new(); // The current interaction target


        public InteractionSystem(InputManager input, WorldManager world, PlayerInteractionHandler playerHandler, CommandInteractionHandler commandHandler)
        {
            if (!initialized)
            {
                this.world = world;
                input.RayHit += OnHover;
                input.MouseMoved += OnMouseMoved;
                input.LeftClick += OnLeftDown;
                input.RightClick += OnRightDown;
                input.LeftClickHeld += OnLeftHeld;
                input.RightClickHeld += OnRightHeld;
                input.LeftClickReleased += OnLeftUp;
                input.RightClickReleased += OnRightUp;
                input.KeyPressed += OnKeyPressed;
                input.KeyReleased += OnKeyReleased;
                input.Moved += OnMoveInput;
                initialized = true;

                handlers[(int)HandlerType.Player] = playerHandler;
                handlers[(int)HandlerType.Command] = commandHandler;
            }
        }

        public void Shutdown(InputManager input)
        {
            if (initialized)
            {
                input.RayHit -= OnHover;
                input.MouseMoved -= OnMouseMoved;
                input.LeftClick -= OnLeftDown;
                input.RightClick -= OnRightDown;
                input.LeftClickHeld -= OnLeftHeld;
                input.RightClickHeld -= OnRightHeld;
                input.LeftClickReleased -= OnLeftUp;
                input.RightClickReleased -= OnRightUp;
                input.KeyPressed -= OnKeyPressed;
                input.KeyReleased -= OnKeyReleased;
                input.Moved -= OnMoveInput;
            }
        }

        #region Input Handling
        // Called from mouse raycast
        void OnHover(RaycastHit hit)
        {
            switch (hit.transform.tag)
            {
                case "Agent":
                    Agent agent = hit.transform.GetComponentInParent<Agent>();
                    if (agent != null) currentTarget.SelectAgent(agent);
                    break;
                case "World":
                    TileData tile = world.Data.GetTile(hit.point);
                    if (tile != null)
                    {
                        currentTarget.SelectTile(tile);
                    }
                    break;
                case "Building":
                    break;
            }
        }

        // Casts ray only when mouse has moved
        void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            mousePos = pos;
            handlers[(int)selectedHandler]?.OnMouseMoved(pos, diff);
        }

        void OnLeftDown()
        {
            handlers[(int)selectedHandler]?.OnLeftDown(currentTarget);
        }

        void OnLeftHeld(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnLeftHeld(diff, time);
        }

        void OnLeftUp(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnLeftUp(diff, time);
        }

        void OnRightDown()
        {
            handlers[(int)selectedHandler]?.OnRightDown(currentTarget);
        }

        void OnRightHeld(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnRightHeld(mousePos, diff, time);
        }

        void OnRightUp(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnRightUp(diff, time);
        }

        void OnKeyPressed(Key key)
        {
            handlers[(int)selectedHandler]?.OnKeyPressed(key);
        }

        void OnKeyReleased(Key key)
        {
            handlers[(int)selectedHandler]?.OnKeyReleased(key);
        }

        // Consumes context.InputManager's Move action on WASD pressed
        void OnMoveInput(Vector2 move)
        {
            handlers[(int)selectedHandler]?.OnMoveInput(move);
        }
        #endregion

        #region Handler Switching
        public void SelectHandler(HandlerType handlerType)
        {
            if (handlerType == selectedHandler) return;

            handlers[(int)selectedHandler]?.Disable();

            selectedHandler = handlerType;
            handlers[(int)selectedHandler]?.Enable(currentTarget, mousePos);
        }
        #endregion
    }
}
