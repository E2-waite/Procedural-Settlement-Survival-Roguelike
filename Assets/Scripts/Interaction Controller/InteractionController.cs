using UnityEngine;
using static Calculations;
// Interprets input events according to the current player interaction mode.
public class InteractionController : MonoBehaviour
{
    public enum GameState : int
    {
        Select = 0,
        Build,
        Command,
        Max
    }

    private IInteractionHandler[] handlers = new IInteractionHandler[(int)GameState.Max];
    private IInteractionHandler currentHandler = null;

    private LayerMask buildMask;
    private LayerMask commandMask;
    private GameState currentState = GameState.Select;
    private HoverTarget target = new HoverTarget();
    public HoverTarget Target => target;
    private bool initialized = false;
    private float rayInterval = 0.01f, rayTimer = 0f;
    private Vector2 mousePos;
    public Vector2 MousePos => mousePos;
    private bool commanding = false;
    private WorldGrid grid;
    private HighlightHandler highlighter = new HighlightHandler();
    private Player player;
    private Vector3 lookVec;
    private Quaternion lookRot, lastRot;

    public void Init(GameContext context)
    {
        if (!initialized)
        {
            World world = context.world;
            grid = world.Context.grid;

            buildMask = context.buildMask;
            commandMask = context.commandMask;

            player = context.player;

            // Subscribe after context.inputManager creates controls, but before gameplay input is enabled.
            context.inputManager.MouseMoved         += OnMouseMoved;
            context.inputManager.LeftClick          += OnLeftDown;
            context.inputManager.RightClick         += OnRightDown;
            context.inputManager.LeftClickHeld      += OnLeftHeld;
            context.inputManager.RightClickHeld     += OnRightHeld;
            context.inputManager.LeftClickReleased  += OnLeftUp;
            context.inputManager.RightClickReleased += OnRightUp;
            context.inputManager.EscapePressed      += OnEscape;
            context.inputManager.FPressed           += OnFKey;
            context.inputManager.Moved              += OnMoveInput;
            initialized = true;

            handlers[(int)GameState.Build] = new BuildInteractionHandler(context);
            handlers[(int)GameState.Command] = new CommandInteractionHandler(context);
            handlers[(int)GameState.Select] = new SelectInteractionHandler(context);
            currentHandler = handlers[(int)currentState];

            InitHandlers();
            highlighter.Init(context);
        }

    }

    void OnDisable()
    {
        // Keep subscriptions paired with Init so disabled controllers do not keep handling input.
        //context.inputManager.MouseMoved -= OnMouseMoved;
        //context.inputManager.LeftClick -= OnLeftDown;
        //context.inputManager.RightClick -= OnRightDown;
        //context.inputManager.LeftClickHeld -= OnLeftHeld;
        //context.inputManager.RightClickHeld -= OnRightHeld;
        //context.inputManager.LeftClickReleased -= OnLeftUp;
        //context.inputManager.RightClickReleased -= OnRightUp;
        //context.inputManager.EscapePressed -= OnEscape;
        //context.inputManager.FPressed -= OnFKey;
        //context.inputManager.Moved -= OnMoveInput;
        initialized = false;
    }
    private void InitHandlers()
    {

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

    public void EnterBuildState(BuildingObject building)
    {
        SetState(GameState.Build);

        // TODO: clean this up
        BuildInteractionHandler handler = (BuildInteractionHandler)handlers[(int)GameState.Build];
        handler.SetSelection(building);
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
        if (commanding || hit.collider == null || currentState == GameState.Command)
        {
            return;
        }

        highlighter.Clear(target);
        // Updates the hover target with the ray hit
        target.Update(hit, grid);
        highlighter.Set(target);

        player.OnHover(hit);


        if (currentHandler != null) currentHandler.OnHover(target);
    }

    // Casts ray from the mouse position
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, currentState == GameState.Build ? buildMask : commandMask))
        {
            OnHover(hit);

            UpdateLook(hit);
        }

    }

    private void UpdateLook(RaycastHit hit)
    {
        if (player == null) return;

        lookVec = hit.point - player.transform.position;
        lookRot = SnappedRotation(lookVec);

        if (lookRot != lastRot)
        {
            lastRot = lookRot;
            player?.OnLookChanged(lookVec, lookRot);
            currentHandler?.OnLookChanged(lookVec, lastRot);
        }
    }


    // Casts ray only when mouse has moved
    void OnMouseMoved(Vector2 pos, Vector2 diff)
    {
        mousePos = pos;
    }

    // Consumes context.inputManager's LeftClick action
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



    // Consumes context.inputManager's RightClick action
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

    // Consumes context.inputManager's EscapePressed action on Esc key pressed
    void OnEscape()
    {
        if (currentHandler != null) currentHandler.OnEscape();
    }

    // Consumes context.inputManager's FKeyPressed action on F key pressed
    void OnFKey()
    {
        if (currentHandler != null) currentHandler.OnFKey();
    }

    // Consumes context.inputManager's Move action on WASD pressed
    void OnMoveInput(Vector2 move)
    {
        if (currentHandler != null) currentHandler.OnMoveInput(move);
    }
}
