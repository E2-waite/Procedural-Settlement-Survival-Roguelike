using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoSingleton<InteractionManager>
{
    public enum GameState
    {
        Build,
        Command
    }

    //public Camera cam;
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

        if (state == GameState.Build && (Keyboard.current.escapeKey.wasPressedThisFrame || Keyboard.current.rKey.wasPressedThisFrame))
        {
            SetState(GameState.Command);
        }
    }

    public void SetState(GameState newState)
    {
        state = newState;
    }
    LayerMask GetLayerMask()
    {
        if (state == GameState.Build) return buildLayerMask;
        else return unitLayerMask;
    }

    void CastRay()
    {
        if (Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GetLayerMask()))
        {
            Debug.DrawLine(ray.origin, hit.point, Color.red);

            if (state == GameState.Build)
            {
                BuildingManager.Instance.HandleRay(hit);
            }
            else if (state == GameState.Command)
            {
                GameManager.Instance.Units.HandleHovering(hit);
            }
        }
    }

    void HandleClick()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (state == GameState.Build)
            {
                BuildingManager.Instance.CreateBuilding();
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            if (state == GameState.Command)
                GameManager.Instance.Units.Command();
            else if (state == GameState.Build)
                SetState(GameState.Command);
        }
    }


}
