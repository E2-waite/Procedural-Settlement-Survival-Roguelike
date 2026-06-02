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
    public LayerMask unitLayerMask;

    private void Update()
    {
        mousePos = Mouse.current.position.ReadValue();

        CastRay();
        HandleClick();
        HandleKeys();
    }

    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitLayerMask))
        {
            MouseHoverHit?.Invoke(hit);
        }
    }

    void HandleClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
            LeftClick?.Invoke();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            RightClick?.Invoke();
    }

    void HandleKeys()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            EscapePressed?.Invoke();
        if (Keyboard.current.fKey.wasPressedThisFrame)
            FPressed?.Invoke(); 
    }
}
