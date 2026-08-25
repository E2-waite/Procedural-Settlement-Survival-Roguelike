using UnityEngine;
using UnityEngine.InputSystem;
using Cinderwild.Core.Data;
using Cinderwild.Gameplay.Agents;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;

namespace Cinderwild.Core.Interaction
{
    public class InteractionController : MonoBehaviour
    {
        private bool initialized = false;
        private float rayInterval = 0.01f, rayTimer = 0f;
        private Vector2 mousePos;
        private Quaternion lookRot, lastRot;
        private RaycastHit lastHit;
        private float lookDist = 0, lastDist = 0;
        private IInteractionHandler currentHandler;
        Context context = null;
        private InteractionTarget selection = new();
        public void Init(Context context)
        {
            if (!initialized)
            {
                this.context = context;
                // Subscribe after context.InputManager creates controls, but before gameplay input is enabled.
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

        void OnDisable()
        {
            // Keep subscriptions paired with Init so disabled controllers do not keep handling input.
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
            initialized = false;
        }

        private void Update()
        {
            if (rayTimer <= 0)
            {
                rayTimer = rayInterval;
                CastRay();
            }
            else
            {
                rayTimer -= Time.deltaTime;
            }
        }

        // Called from mouse raycast
        void OnHover(RaycastHit hit)
        {
            switch(hit.transform.tag)
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

        // Casts ray from the mouse position
        void CastRay()
        {
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                OnHover(hit);
            }

        }

        // Casts ray only when mouse has moved
        void OnMouseMoved(Vector2 pos, Vector2 diff)
        {
            mousePos = pos;
        }

        // Consumes context.InputManager's LeftClick action
        void OnLeftDown()
        {
            currentHandler?.OnLeftDown();
        }

        void OnLeftHeld(Vector2 diff, float time)
        {
            currentHandler?.OnLeftHeld(diff, time);
        }

        void OnLeftUp(Vector2 diff, float time)
        {
            currentHandler?.OnLeftUp(diff, time);
        }



        // Consumes context.InputManager's RightClick action
        void OnRightDown()
        {
            currentHandler?.OnRightDown();
        }


        void OnRightHeld(Vector2 diff, float time)
        {
            currentHandler?.OnRightHeld(mousePos, diff, time);
        }

        void OnRightUp(Vector2 diff, float time)
        {
            currentHandler?.OnRightUp(diff, time);
        }

        void OnKeyPressed(Key key)
        {
            currentHandler?.OnKeyPressed(key);
        }

        void OnKeyReleased(Key key)
        {
            currentHandler?.OnKeyReleased(key);
        }

        // Consumes context.InputManager's Move action on WASD pressed
        void OnMoveInput(Vector2 move)
        {
            currentHandler?.OnMoveInput(move);
        }
    }
}
