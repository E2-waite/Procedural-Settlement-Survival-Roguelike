using System.ComponentModel;
using UnityEngine;
using static InteractionController;

public class BuildInteractionHandler : IInteractionHandler
{
    readonly private InteractionController   _controller;
    readonly private BuildingSystem          _buildSystem;
    readonly private TileMarker              _tileMarker;
    private GridTile                        _lastTile;

    public BuildInteractionHandler(GameContext context)
    {
        _controller = context.interactionController;
        _buildSystem = context.buildingSystem;
        _tileMarker = context.tileMarker;
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

    public void SetSelection(BuildingObject selected)
    {
        _buildSystem.Select(selected);
    }

    public void OnHover(HoverTarget target)
    {
        if (target.IsTile && target.Tile != _lastTile)
        {
            _lastTile = target.Tile;

            // No preview is shown until the player has selected a building type.
            if (_buildSystem.Selected != null)
            {
                _tileMarker.HighlightTiles(target.Tile.position, _buildSystem.Selected.size, _buildSystem.Selected != null && _buildSystem.CanAfford());
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
            if (_buildSystem.TryPlace(_controller.Target.Tile, _buildSystem.Selected))
            {
                _tileMarker.HighlightTiles(_controller.Target.Tile.position, _buildSystem.Selected.size, _buildSystem.Selected != null && _buildSystem.CanAfford());
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
        _controller.SetState(GameState.Select);
    }

    public void OnRightHeld(Vector2 diff, float time)
    {

    }
    public void OnRightUp(Vector2 diff, float time)
    {

    }
    public void OnEscape()
    {
        _controller.SetState(GameState.Select);
    }
    public void OnFKey()
    {

    }
    public void OnMoveInput(Vector2 move)
    {

    }

    public void OnLookChanged(Quaternion rot)
    {

    }
}
