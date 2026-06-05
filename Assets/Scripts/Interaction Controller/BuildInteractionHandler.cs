using System.ComponentModel;
using UnityEngine;
using static InteractionController;

public class BuildInteractionHandler : IInteractionHandler
{
    private InteractionController _controller;
    private BuildingSystem _system;
    private TileMarker _tileMarker;
    private GridTile _lastTile;
    public BuildInteractionHandler(InteractionController controller, BuildingSystem system, TileMarker tileMarker)
    {
        _controller = controller;
        _system = system;
        _tileMarker = tileMarker;
    }

    public void Enable()
    {
        Cursor.visible = false;
        _tileMarker.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _tileMarker.gameObject.SetActive(false);
    }

    public void OnHover(HoverTarget target)
    {
        if (target.IsTile && target.Tile != _lastTile)
        {
            _lastTile = target.Tile;

            BuildingObject selected = _system.SelectedBuilding();

            // No preview is shown until the player has selected a building type.
            if (selected != null)
            {
                _tileMarker.HighlightTiles(target.Tile.position, selected.size, selected != null && selected.CanAfford());
                _tileMarker.transform.position = new Vector3(target.Tile.position.x + 1.5f, 0, target.Tile.position.y + 1.5f);
            }
        }
    }

    public void OnMouseMoved(Vector2 pos, Vector2 diff)
    {

    }

    public void OnLeftDown()
    {
        if (_controller.Target.IsTile)
        {
            if (_system.TryPlace(_controller.Target.Tile))
            {
                BuildingObject selected = _system.SelectedBuilding();
                _tileMarker.HighlightTiles(_controller.Target.Tile.position, selected.size, selected != null && selected.CanAfford());
            }
        }
    }
    public void OnLeftHeld(Vector2 diff, float time)
    {

    }
    public void OnLeftUp(Vector2 diff, float time)
    {

    }
    public void OnRightDown()
    {
        _controller.SetState(GameState.Control);
    }

    public void OnRightHeld(Vector2 diff, float time)
    {

    }
    public void OnRightUp(Vector2 diff, float time)
    {

    }
    public void OnEscape()
    {
        _controller.SetState(GameState.Control);
    }
    public void OnFKey()
    {

    }
    public void OnMoveInput(Vector2 move)
    {

    }
}
