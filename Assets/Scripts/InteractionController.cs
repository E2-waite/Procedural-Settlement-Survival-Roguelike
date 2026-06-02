using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Interprets input events according to the current player interaction mode.
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
    private bool initialized = false;


    public void Init()
    {
        if (!initialized)
        {
            // Subscribe after InputManager creates controls, but before gameplay input is enabled.
            InputManager.Instance.MouseHoverHit += OnHover;
            InputManager.Instance.LeftClick += OnLeftClick;
            InputManager.Instance.RightClick += OnRightClick;
            InputManager.Instance.EscapePressed += OnEscape;
            InputManager.Instance.FPressed += OnFKey;
            InputManager.Instance.Moved += OnMove;
            initialized = true;
        }

    }

    void OnDisable()
    {
        // Keep subscriptions paired with Init so disabled controllers do not keep handling input.
        InputManager.Instance.MouseHoverHit -= OnHover;
        InputManager.Instance.LeftClick -= OnLeftClick;
        InputManager.Instance.RightClick -= OnRightClick;
        InputManager.Instance.EscapePressed -= OnEscape;
        InputManager.Instance.FPressed -= OnFKey;
        InputManager.Instance.Moved -= OnMove;
        initialized = false;
    }

    // Consumes InputManager's MouseHoverHit action
    void OnHover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        lastHit = hit;

        // Gets the hovering object (Unit, tile or building) from the hit transform
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

    // Sets the current game state
    public void SetState(GameState newState)
    {
        if (newState != state)
        {
            // Enter/exit hooks for interaction modes live here.
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


    // Consumes InputManager's LeftClick action
    void OnLeftClick()
    {
        if (state == GameState.Build)
        {
            if (hoveringTile != null)
                BuildingSystem.Instance.TryPlace(hoveringTile);
        }
    }

    // Consumes InputManager's RightClick action
    void OnRightClick()
    {
        if (state == GameState.Build)
        {
            SetState(GameState.Command);
        }
        else if (state == GameState.Command)
        {
            if (hoveringTile != null)
                CommandSystem.Instance.Command(hoveringTile);
            else if (hoveringUnit != null)
                CommandSystem.Instance.Command(hoveringUnit);
            else if (hoveringBuilding != null)
                CommandSystem.Instance.Command(hoveringBuilding);
        }
    }


    // Consumes InputManager's EscapePressed action on Esc key pressed
    void OnEscape()
    {
        if (state == GameState.Build)
        {
            SetState(GameState.Command);
        }
    }

    // Consumes InputManager's FKeyPressed action on F key pressed
    void OnFKey()
    {
        if (state == GameState.Command)
        {
            GameManager.Player.CallUnits();
        }
    }

    // Consumes InputManager's Move action on WASD pressed
    void OnMove(Vector2 move)
    {
        GameManager.Player.OnMove(move);
    }

    // Sets the current hovering tile and clears other hover types
    void SetHovering(GridTile tile)
    {
        Debug.Log("Hovering tile pos " + tile.position);
        hoveringTile = tile;
        hoveringBuilding = null;
        hoveringUnit = null;
    }

    // Sets the current hovering unit and clears other hover types
    void SetHovering(Unit unit)
    {
        hoveringUnit = unit;
        hoveringBuilding = null;
        hoveringTile = null;
    }

    // Sets the current hovering building and clears other hover types
    void SetHovering(Building building)
    {
        hoveringBuilding = building;
        hoveringTile = null;
        hoveringUnit = null;
    }
}
