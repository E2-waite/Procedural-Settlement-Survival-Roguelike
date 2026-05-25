using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoSingleton<InteractionManager>
{
    public enum GameState
    {
        Build,
        Command
    }

    public Camera cam;
    public LayerMask buildLayerMask;
    public LayerMask unitLayerMask;

    public GameState state;


    private void Start()
    {
        state = GameState.Command;
    }

    void Update()
    {
        CastRay();
        HandleClick();

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            if (state == GameState.Build)
            {
                state = GameState.Command;
                BuildHandler.Instance.SetEnabled(false);
            }
            else
            {
                state = GameState.Build;
                BuildHandler.Instance.SetEnabled(true);
            }
        }
    }

    LayerMask GetLayerMask()
    {
        if (state == GameState.Build) return buildLayerMask;
        else return unitLayerMask;
    }

    void CastRay()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GetLayerMask()))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            if (state == GameState.Build)
                BuildHandler.Instance.HandleRay(hit);
            else if (state == GameState.Command)
                UnitHandler.Instance.Hover(hit);
        }
    }

    void HandleClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (state == GameState.Build)
            {
                BuildHandler.Instance.Build();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (state == GameState.Command)
                UnitHandler.Instance.CommandUnits();
        }
    }


}
