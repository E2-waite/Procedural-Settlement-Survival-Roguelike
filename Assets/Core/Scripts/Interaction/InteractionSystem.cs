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

    // TODO: consider making this class non-static
    public static class InteractionSystem
    {


        private static bool initialized = false;
        private static Vector2 mousePos; // The current mouse position
        private static HandlerType selectedHandler = HandlerType.Player;

        private static IInteractionHandler[] handlers = new IInteractionHandler[(int)HandlerType.Max];
        private static InteractionTarget currentTarget = new(); // The current interaction target


        public static void Init(Context context)
        {
            if (!initialized)
            {
                context.Input.RayHit += OnHover;
                context.Input.MouseMoved += OnMouseMoved;
                context.Input.LeftClick += OnLeftDown;
                context.Input.RightClick += OnRightDown;
                context.Input.LeftClickHeld += OnLeftHeld;
                context.Input.RightClickHeld += OnRightHeld;
                context.Input.LeftClickReleased += OnLeftUp;
                context.Input.RightClickReleased += OnRightUp;
                context.Input.KeyPressed += OnKeyPressed;
                context.Input.KeyReleased += OnKeyReleased;
                context.Input.Moved += OnMoveInput;
                initialized = true;

                handlers[(int)HandlerType.Player] = new PlayerInteractionHandler(context);
                handlers[(int)HandlerType.Command] = new CommandInteractionHandler(context);
            }
        }

        public static void Shutdown(Context context)
        {
            if (initialized)
            {
                context.Input.RayHit -= OnHover;
                context.Input.MouseMoved -= OnMouseMoved;
                context.Input.LeftClick -= OnLeftDown;
                context.Input.RightClick -= OnRightDown;
                context.Input.LeftClickHeld -= OnLeftHeld;
                context.Input.RightClickHeld -= OnRightHeld;
                context.Input.LeftClickReleased -= OnLeftUp;
                context.Input.RightClickReleased -= OnRightUp;
                context.Input.KeyPressed -= OnKeyPressed;
                context.Input.KeyReleased -= OnKeyReleased;
                context.Input.Moved -= OnMoveInput;
            }
        }

        #region Input Handling
        // Called from mouse raycast
        static void OnHover(RaycastHit hit)
        {
            switch (hit.transform.tag)
            {
                case "Agent":
                    Agent agent = hit.transform.GetComponentInParent<Agent>();
                    if (agent != null) currentTarget.SelectAgent(agent);
                    break;
                case "World":
                    TileData tile = WorldSystem.GetTile(hit.point);
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
        static void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            mousePos = pos;
            handlers[(int)selectedHandler]?.OnMouseMoved(pos, diff);
        }

        static void OnLeftDown()
        {
            handlers[(int)selectedHandler]?.OnLeftDown(currentTarget);
        }

        static void OnLeftHeld(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnLeftHeld(diff, time);
        }

        static void OnLeftUp(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnLeftUp(diff, time);
        }

        static void OnRightDown()
        {
            handlers[(int)selectedHandler]?.OnRightDown(currentTarget);
        }

        static void OnRightHeld(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnRightHeld(mousePos, diff, time);
        }

        static void OnRightUp(Vector2 diff, float time)
        {
            handlers[(int)selectedHandler]?.OnRightUp(diff, time);
        }

        static void OnKeyPressed(Key key)
        {
            handlers[(int)selectedHandler]?.OnKeyPressed(key);
        }

        static void OnKeyReleased(Key key)
        {
            handlers[(int)selectedHandler]?.OnKeyReleased(key);
        }

        // Consumes context.InputManager's Move action on WASD pressed
        static void OnMoveInput(Vector2 move)
        {
            handlers[(int)selectedHandler]?.OnMoveInput(move);
        }
        #endregion

        #region Handler Switching
        public static void SelectHandler(HandlerType handlerType)
        {
            if (handlerType == selectedHandler) return;

            handlers[(int)selectedHandler]?.Disable();

            selectedHandler = handlerType;
            handlers[(int)selectedHandler]?.Enable(currentTarget, mousePos);
        }
        #endregion
    }
}
