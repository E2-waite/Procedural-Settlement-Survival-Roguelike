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
    private HoverTarget target = new HoverTarget();
    private RaycastHit lastHit;
    private bool initialized = false;
    private float rayInterval = 0.01f, rayTimer = 0f;
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

        // Updates the hover target with the ray hit
        target.Update(hit);
        lastHit = hit;

        if (state == GameState.Build && target.IsTile)
        {
            BuildingSystem.Instance.HandleHover(target.Tile);
        }
    }

    // Casts ray from the mouse position
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, state == GameState.Build ? buildMask : commandMask))
        {
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
            if (target.IsTile)
                BuildingSystem.Instance.TryPlace(target.Tile);
        }
    }

    void OnLeftHeld(Vector2 diff, float time)
    {
        if (state == GameState.Command)
        {
            // Starts commanding hoving unit when LMB held for over .25 seconds
            if (time >= .25f && target.IsFollower)
            {
                CommandSystem.Instance.StartCommanding(target.Follower);
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
                if (!target.IsUnit)
                    CommandSystem.Instance.StopCommanding();
                else
                {
                    if (target.IsFollower)
                    {
                        FollowerUnit follower = target.Follower;

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
                CommandSystem.Instance.Command(target);
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
}
