using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Controller for interpreting inputs
public class InteractionController : MonoSingleton<InteractionController>
{
    public enum GameState
    {
        Build,
        Command
    }

    public GameState state;
    private RaycastHit lastHit;

    private GridTile hoveringTile = null;
    private Unit hoveringUnit = null;
    private Building hoveringBuilding = null;


    void Start()
    {
        InputManager.Instance.MouseHoverHit += OnHover;
        InputManager.Instance.LeftClick += OnLeftClick;
        InputManager.Instance.RightClick += OnRightClick;
        InputManager.Instance.EscapePressed += OnEscape;
        InputManager.Instance.FPressed += OnFKey;
        InputManager.Instance.Moved += OnMove;
    }

    void OnDisable()
    {
        InputManager.Instance.MouseHoverHit -= OnHover;
        InputManager.Instance.LeftClick -= OnLeftClick;
        InputManager.Instance.RightClick -= OnRightClick;
        InputManager.Instance.EscapePressed -= OnEscape;
        InputManager.Instance.FPressed -= OnFKey;
        InputManager.Instance.Moved -= OnMove;
    }

    void OnHover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        lastHit = hit;

        switch (hit.transform.tag)
        {
            case "Unit":
                SetHovering(hit.transform.GetComponent<Unit>());
                break;
            case "Tile":
                SetHovering(WorldManager.grid.GetTile(hit.point));
                break;
            case "Building":
                SetHovering(hit.transform.GetComponent<Building>());
                break;
        }

        if (state == GameState.Build && hoveringTile != null)
        {
            BuildingSystem.Instance.HandleHover(hoveringTile);
        }
    }

    public void SetState(GameState newState)
    {
        if (newState != state)
        {


            if (newState == GameState.Build)
            {
                BuildingSystem.Instance.SetEnabled(true);
            }
            else if (newState == GameState.Command)
            {
                
            }

            if (state == GameState.Build)
            {
                BuildingSystem.Instance.SetEnabled(false);
            }

            state = newState;
        }

       
    }



    void OnLeftClick()
    {
        if (state == GameState.Build)
        {
            if (hoveringTile != null)
                BuildingSystem.Instance.TryPlace(hoveringTile);
        }
    }

    void OnRightClick()
    {
        if (state == GameState.Build)
        {
            SetState(GameState.Command);
        }
        else if (state == GameState.Command)
        {
            if (hoveringTile != null)
                UnitSystem.Instance.Command(hoveringTile);
            else if (hoveringUnit != null)
                UnitSystem.Instance.Command(hoveringUnit);
            else if (hoveringBuilding != null)
                UnitSystem.Instance.Command(hoveringBuilding);
        }
    }

    void OnEscape()
    {
        if (state == GameState.Build)
        {
            SetState(GameState.Command);
        }
    }

    void OnFKey()
    {
        if (state == GameState.Command)
        {
            UnitSystem.Instance.StartFollowing();
        }
    }

    void OnMove(Vector2 move)
    {
        GameManager.Player.Move(move);
    }

    void SetHovering(GridTile tile)
    {
        Debug.Log("Hovering tile pos " + tile.position);
        hoveringTile = tile;
        hoveringBuilding = null;
        hoveringUnit = null;
    }

    void SetHovering(Unit unit)
    {
        hoveringUnit = unit;
        hoveringBuilding = null;
        hoveringTile = null;
    }

    void SetHovering(Building building)
    {
        hoveringBuilding = building;
        hoveringTile = null;
        hoveringUnit = null;
    }
}
