using UnityEngine;
using UnityEngine.InputSystem;
using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;

namespace Cinderwild.Core
{
    public static class InteractionSystem
    {
        private static bool initialized = false;
        private static Vector2 mousePos;
        private static IInteractionHandler currentHandler;
        private static InteractionTarget selection = new();

        public static void Init(Context context)
        {
            if (!initialized)
            {
                // Subscribe after context.InputManager creates controls, but before gameplay input is enabled.
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

                currentHandler = new PlayerInteractionHandler(context);
            }
        }

        public static void Tick()
        {

        }

        // Called from mouse raycast
        static void OnHover(RaycastHit hit)
        {
            switch (hit.transform.tag)
            {
                case "Agent":
                    Agent agent = hit.transform.GetComponentInParent<Agent>();
                    if (agent != null) selection.SelectAgent(agent);
                    break;
                case "World":
                    TileData tile = WorldSystem.GetTile(hit.point);
                    if (tile != null) selection.SelectTile(tile);
                    break;
                case "Building":
                    break;
            }
        }

        // Casts ray only when mouse has moved
        static void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            mousePos = pos;
        }

        // Consumes context.InputManager's LeftClick action
        static void OnLeftDown()
        {
            currentHandler?.OnLeftDown();
        }

        static void OnLeftHeld(Vector2 diff, float time)
        {
            currentHandler?.OnLeftHeld(diff, time);
        }

        static void OnLeftUp(Vector2 diff, float time)
        {
            currentHandler?.OnLeftUp(diff, time);
        }



        // Consumes context.InputManager's RightClick action
        static void OnRightDown()
        {
            currentHandler?.OnRightDown();
        }


        static void OnRightHeld(Vector2 diff, float time)
        {
            currentHandler?.OnRightHeld(mousePos, diff, time);
        }

        static void OnRightUp(Vector2 diff, float time)
        {
            currentHandler?.OnRightUp(diff, time);
        }

        static void OnKeyPressed(Key key)
        {
            currentHandler?.OnKeyPressed(key);
        }

        static void OnKeyReleased(Key key)
        {
            currentHandler?.OnKeyReleased(key);
        }

        // Consumes context.InputManager's Move action on WASD pressed
        static void OnMoveInput(Vector2 move)
        {
            currentHandler?.OnMoveInput(move);
        }
    }
}
