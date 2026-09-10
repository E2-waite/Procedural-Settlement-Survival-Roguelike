using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Cinderwild.Core
{
    // Converts raw Input System state into simple gameplay events for other systems.
    public class InputManager : MonoBehaviour
    {
        public enum MouseButton : int
        {
            Left = 0,
            Middle,
            Right,
            Max
        }

        bool initialized = false;
        Vector2 mousePos, lastMousePos;

        public event Action LeftClick;
        public event Action RightClick;
        public event Action<Vector2, float> LeftClickHeld;
        public event Action<Vector2, float> RightClickHeld;
        public event Action<Vector2, float> LeftClickReleased;
        public event Action<Vector2, float> RightClickReleased;
        public event Action<Key> KeyPressed;
        public event Action<Key> KeyReleased;

        public event Action<Vector2, Vector2> MouseMoved;
        public event Action<Vector2> Moved;
        public event Action<RaycastHit> RayHit;
        private PlayerControls controls;
        private Vector2 moveInput;
        private bool[] held = new bool[(int)MouseButton.Max];
        private Vector2[] clickStartPos = new Vector2[(int)MouseButton.Max];
        private float[] clickStartTime = new float[(int)MouseButton.Max];
        private float rayInterval = 0.01f, rayTimer = 0f;

        public void Init()
        {
            if (initialized) return;

            // PlayerControls creates an InputActionAsset, so it must be created after Unity construction.
            controls = new PlayerControls();
            controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
            controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

            initialized = true;
        }

        private void OnDisable()
        {
            controls?.Player.Disable();
        }

        public void EnableGameplayInput()
        {
            // Keep enabling separate from Init so listeners can subscribe before input starts firing.
            controls.Player.Enable();
        }

        private void Update()
        {
            if (!initialized) return;

            HandleMouse();
            HandleClick();
            HandleKeys();
            HandleMove();
            HandleRay();
        }

        private void OnDestroy()
        {
            controls?.Dispose();
        }

        private void HandleMouse()
        {
            Vector2 newPos = Mouse.current.position.ReadValue();

            if (mousePos != newPos)
            {
                lastMousePos = mousePos;
                mousePos = newPos;
                MouseMoved?.Invoke(mousePos, mousePos - lastMousePos);
            }
        }

        // Invoke click actions on mouse click
        void HandleClick()
        {
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                held[(int)MouseButton.Left] = false;
                Vector2 posDiff = mousePos - clickStartPos[(int)MouseButton.Left];
                LeftClickReleased?.Invoke(posDiff, Time.time - clickStartTime[(int)MouseButton.Left]);
            }

            if (Mouse.current.rightButton.wasReleasedThisFrame)
            {
                held[(int)MouseButton.Right] = false;
                Vector2 posDiff = mousePos - clickStartPos[(int)MouseButton.Right];
                RightClickReleased?.Invoke(posDiff, Time.time - clickStartTime[(int)MouseButton.Right]);
            }

            if (held[(int)MouseButton.Left])
            {
                Vector2 posDiff = mousePos - clickStartPos[(int)MouseButton.Left];
                LeftClickHeld?.Invoke(posDiff, Time.time - clickStartTime[(int)MouseButton.Left]);
            }

            if (held[(int)MouseButton.Right])
            {
                Vector2 posDiff = mousePos - clickStartPos[(int)MouseButton.Right];
                RightClickHeld?.Invoke(posDiff, Time.time - clickStartTime[(int)MouseButton.Right]);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                held[(int)MouseButton.Left] = true;
                clickStartTime[(int)MouseButton.Left] = Time.time;
                clickStartPos[(int)MouseButton.Left] = mousePos;
                LeftClick?.Invoke();
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                held[(int)MouseButton.Right] = true;
                clickStartTime[(int)MouseButton.Right] = Time.time;
                clickStartPos[(int)MouseButton.Right] = mousePos;
                RightClick?.Invoke();
            }
        }

        // Invokes keypress actions on key input
        void HandleKeys()
        {
            if (Keyboard.current == null) return;

            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame)
                {
                    KeyPressed?.Invoke(key.keyCode);
                }
                else if (key.wasReleasedThisFrame)
                {
                    KeyReleased?.Invoke(key.keyCode);
                }
            }
        }

        // Invokes Moved action on move input (WASD)
        void HandleMove()
        {
            if (moveInput.x != 0 || moveInput.y != 0)
            {
                Moved?.Invoke(moveInput);
            }
        }

        void HandleRay()
        {
            if (rayTimer <= 0)
            {
                rayTimer = rayInterval;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    RayHit?.Invoke(hit);
                }
            }
            else
            {
                rayTimer -= Time.deltaTime;
            }
        }
    }
}
