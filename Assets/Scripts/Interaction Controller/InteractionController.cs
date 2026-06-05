using UnityEngine;

// Interprets input events according to the current player interaction mode.
public class InteractionController : MonoSingleton<InteractionController>
{
    public enum GameState : int
    {
        Control = 0,
        Build,
        Command,
        Max
    }

    private IInteractionHandler[] handlers = new IInteractionHandler[(int)GameState.Max];
    private IInteractionHandler currentHandler = null;
    [SerializeField] private LayerMask buildMask;
    [SerializeField] public LayerMask commandMask;

    private GameState currentState = GameState.Control;
    private HoverTarget target = new HoverTarget();
    public HoverTarget Target => target;
    private bool initialized = false;
    private float rayInterval = 0.01f, rayTimer = 0f;
    private Vector2 mousePos;
    public Vector2 MousePos => mousePos;
    private bool commanding = false;

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
            InputManager.Instance.Moved += OnMoveInput;
            initialized = true;

            InitHandlers();
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
        InputManager.Instance.Moved -= OnMoveInput;
        initialized = false;
    }
    private void InitHandlers()
    {
        handlers[(int)GameState.Build] = new BuildInteractionHandler(this);
        handlers[(int)GameState.Command] = new CommandInteractionHandler(this);
        handlers[(int)GameState.Control] = new ControlInteractionHandler(this);

        currentHandler = handlers[(int)currentState];
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
        if (newState != currentState)
        {
            currentHandler.Disable();
            handlers[(int)newState].Enable();

            currentHandler = handlers[(int)newState];
            currentState = newState;
        }
    }

    // Called from mouse raycast
    void OnHover(RaycastHit hit)
    {
        if (commanding || hit.collider == null)
        {
            return;
        }

        // Updates the hover target with the ray hit
        target.Update(hit);

        if (currentState == GameState.Build && target.IsTile)
        {
            BuildingSystem.Instance.HandleHover(target.Tile);
        }
    }

    // Casts ray from the mouse position
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, currentState == GameState.Build ? buildMask : commandMask))
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
        if (currentHandler != null) currentHandler.OnLeftDown();
    }

    void OnLeftHeld(Vector2 diff, float time)
    {
        if (currentHandler != null) currentHandler.OnLeftHeld(diff, time);
    }

    void OnLeftUp(Vector2 diff, float time)
    {
        if (currentHandler != null) currentHandler.OnLeftUp(diff, time);
    }



    // Consumes InputManager's RightClick action
    void OnRightDown()
    {
        if (currentHandler != null) currentHandler.OnRightDown();
    }


    void OnRightHeld(Vector2 diff, float time)
    {
        if (currentHandler != null) currentHandler.OnRightHeld(diff, time);
    }

    void OnRightUp(Vector2 diff, float time)
    {
        if (currentHandler != null) currentHandler.OnRightUp(diff, time);

    }

    // Consumes InputManager's EscapePressed action on Esc key pressed
    void OnEscape()
    {
        if (currentHandler != null) currentHandler.OnEscape();
    }

    // Consumes InputManager's FKeyPressed action on F key pressed
    void OnFKey()
    {
        if (currentHandler != null) currentHandler.OnFKey();
    }

    // Consumes InputManager's Move action on WASD pressed
    void OnMoveInput(Vector2 move)
    {
        if (currentHandler != null) currentHandler.OnMoveInput(move);
    }
}
