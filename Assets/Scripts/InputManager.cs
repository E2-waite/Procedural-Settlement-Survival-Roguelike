using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Manager for taking inputs
public class InputManager : MonoSingleton<InputManager>
{
    Vector2 mousePos;

    public event Action<RaycastHit> MouseHoverHit;

    public event Action LeftClick;
    public event Action RightClick;
    public event Action EscapePressed;
    public event Action FPressed;
    public event Action<Vector2> Moved;
    public LayerMask unitLayerMask;

    [HideInInspector] public PlayerController player;
    private PlayerControls controls;
    private Vector2 moveInput;

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    protected override void Awake()
    {
        base.Awake();
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
    }


    private void Update()
    {
        mousePos = Mouse.current.position.ReadValue();

        CastRay();
        HandleClick();
        HandleKeys();
        HandleMove();
    }

    // Casts ray from the mouse position
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask))
        {
            MouseHoverHit?.Invoke(hit);
        }
    }

    // Invoke click actions on mouse click
    void HandleClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            LeftClick?.Invoke();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            RightClick?.Invoke();
    }

    // Invokes keypress actions on key input
    void HandleKeys()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            EscapePressed?.Invoke();
        if (Keyboard.current.fKey.wasPressedThisFrame)
            FPressed?.Invoke(); 
    }

    // Invokes Moved action on move input (WASD)
    void HandleMove()
    {
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            Moved?.Invoke(moveInput);
        }
    }
}
