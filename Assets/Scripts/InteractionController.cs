using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.PlayerSettings;

// Interprets input events according to the current player interaction mode.
public class InteractionController : MonoSingleton<InteractionController>
{
    public enum GameState
    {
        Build,
        Command
    }

    [SerializeField] private LayerMask buildMask;
    [SerializeField] public LayerMask commandMask;

    public GameState state;
    private RaycastHit lastHit;
    private GridTile hoveringTile = null;
    private Unit hoveringUnit = null;
    private Building hoveringBuilding = null;
    private bool initialized = false;
    private float rayInterval = 0.1f, rayTimer = 0f;
    private Vector2 mousePos;

    public void Init()
    {
        if (!initialized)
        {
            // Subscribe after InputManager creates controls, but before gameplay input is enabled.
            InputManager.Instance.MouseMoved += OnMouseMoved;
            InputManager.Instance.LeftClick += OnLeftDown;
            InputManager.Instance.RightClick += OnRightDown;
            InputManager.Instance.LeftClickHeld += OnLeftHeld;
            InputManager.Instance.RightClickHeld += OnRightHeld;
            InputManager.Instance.LeftClickReleased += OnLeftUp;
            InputManager.Instance.RightClickReleased += OnRightUp;
            InputManager.Instance.RightClick += OnRightDown;
            InputManager.Instance.EscapePressed += OnEscape;
            InputManager.Instance.FPressed += OnFKey;
            InputManager.Instance.Moved += OnMove;
            initialized = true;
        }

    }

    void OnDisable()
    {
        // Keep subscriptions paired with Init so disabled controllers do not keep handling input.
        InputManager.Instance.MouseMoved -= OnMouseMoved;
        InputManager.Instance.LeftClick -= OnLeftDown;
        InputManager.Instance.RightClick -= OnRightDown;
        InputManager.Instance.LeftClickHeld -= OnLeftHeld;
        InputManager.Instance.RightClickHeld -= OnRightHeld;
        InputManager.Instance.LeftClickReleased -= OnLeftUp;
        InputManager.Instance.RightClickReleased -= OnRightUp;
        InputManager.Instance.EscapePressed -= OnEscape;
        InputManager.Instance.FPressed -= OnFKey;
        InputManager.Instance.Moved -= OnMove;
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

    // Called from mouse raycast
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

    // Casts ray from the mouse position
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, state == GameState.Build ? buildMask : commandMask))
        {
            Debug.Log("Ray hit " + hit.collider.name);
            OnHover(hit);
        }
    }

    // Casts ray only when mouse has moved
    void OnMouseMoved(Vector2 pos, Vector2 diff)
    {
        mousePos = pos;
    }

    // Consumes InputManager's LeftClick action
    void OnLeftDown()
    {
        if (state == GameState.Build)
        {
            if (hoveringTile != null)
                BuildingSystem.Instance.TryPlace(hoveringTile);
        }
    }

    void OnLeftHeld(Vector2 diff, float time)
    {
        if (state == GameState.Command)
        {
            // Starts commanding hoving unit when LMB held for over .25 seconds
            if (time >= .25f && hoveringUnit != null && hoveringUnit is FollowerUnit)
            {
                CommandSystem.Instance.StartCommanding((FollowerUnit)hoveringUnit);
            }
        }
    }

    void OnLeftUp(Vector2 diff, float time)
    {
        // Stops commanding units when LMB released and held for less than .25 seconds (tap) and not hovering over unit
        if (time < .25f)
        {
            if (state == GameState.Command)
            {
                if (hoveringUnit == null)
                    CommandSystem.Instance.StopCommanding();
                else
                {
                    if (hoveringUnit is FollowerUnit)
                    {
                        FollowerUnit follower = (FollowerUnit)hoveringUnit;

                        if (follower.Commanding)
                            CommandSystem.Instance.StopCommanding(follower);
                        else
                            CommandSystem.Instance.StartCommanding(follower);
                    }
                }
            }
        }
    }

    // Consumes InputManager's RightClick action
    void OnRightDown()
    {
        if (state == GameState.Build)
        {
            SetState(GameState.Command);
        }
        if (state == GameState.Command)
        {
            CommandPanel.Instance.SetWidgetPos(mousePos);
        }
    }

    void OnRightHeld(Vector2 diff, float time)
    {
        if (diff.y > 10 || diff.y < -10)
        {
            CommandPanel.Instance.ShowWidget();
            CommandPanel.Instance.UpdateWidget(diff.y);
        }
        else
        {
            CommandPanel.Instance.HideWidget();
        }
    }

    void OnRightUp(Vector2 diff, float time)
    {
        if (diff.y > 10)
        {
            if (state == GameState.Command)
            {
                // TODO: command to interact with object at mouse pos when the click started rather than the current position

                if (hoveringTile != null)
                    CommandSystem.Instance.Command(hoveringTile);
                else if (hoveringUnit != null)
                    CommandSystem.Instance.Command(hoveringUnit);
                else if (hoveringBuilding != null)
                    CommandSystem.Instance.Command(hoveringBuilding);
            }
        }
        else if (diff.y < -10)
        {
            if (state == GameState.Command)
            {
                CommandSystem.Instance.CommandFollow();
            }
        }

        CommandPanel.Instance.HideWidget();


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
        if (state == GameState.Command && GameManager.Player != null)
        {
            CommandSystem.Instance.StartCommanding(GameManager.Player.NearbyUnits);
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
