using UnityEngine;
using Cinderwild.Core.Data;
using UnityEngine.InputSystem;

namespace Cinderwild.Core.Input
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
                //handlers[(int)GameState.Build] = new BuildInteractionHandler(context);
                //handlers[(int)GameState.Action] = new ActionInteractionHandler(context);
                //handlers[(int)GameState.Command] = new CommandInteractionHandler(context);
                //handlers[(int)GameState.Select] = new PlayerInteractionHandler(context);
                //currentHandler = handlers[(int)currentState];

                //highlighter.Init(context);
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
