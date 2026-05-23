using UnityEngine;

public class UnitHandler : MonoSingleton<UnitHandler>
{
    public Unit selectedUnit = null;
    public Unit hoveringUnit = null;
    public GridTile hoveringTile = null;
    public void Hover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.TryGetComponent<Unit>(out Unit unit))
        {
            hoveringUnit = unit;
            hoveringTile = null;
        }
        else
        {
            hoveringUnit = null;
            hoveringTile = Grid.Instance.getTile(hit.point);
        }
        
    }

    public void ClearHover()
    {
        hoveringUnit = null;
    }
    public void ClearSelection()
    {
        selectedUnit = null;
    }

    public void SelectUnit()
    {
        if (hoveringUnit == null)
            selectedUnit = null;
        else
            selectedUnit = hoveringUnit;
    }

    public void MoveUnit()
    {
        if (!selectedUnit) return;

        if (hoveringTile != null)
        {
            Vector3 movePos = new Vector3(hoveringTile.position.x, 0.5f, hoveringTile.position.y);
            selectedUnit.MoveTo(movePos);
        }
    }
}
