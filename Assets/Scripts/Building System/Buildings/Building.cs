using System.Collections.Generic;
using UnityEngine;

public class Building : Damageable
{
    public override TargetType Type => TargetType.Building;

    public bool autoBuild = false;
    public int id;
    bool built = false, destroyed = false;
    public bool Built => built && !destroyed;
    public MeshRenderer mesh;
    private bool hovering = false;
    private List<GridTile> tiles = new List<GridTile>();
    protected virtual void Start()
    {
        if (!Built)
            mesh.material.color = Color.red;

        if (autoBuild)
        {
            FinishBuilding();
        }
    }

    public void Init(int id)
    {
    }


    public bool Build(float val)
    {
        health.Heal(val);

        if (health.Full())
        {
            if (!built)
            {
                built = true;
                FinishBuilding();
            }
            else
            {
                destroyed = false;
            }

            return true;
        }
        return false;
    }

    protected virtual void FinishBuilding()
    {
        mesh.material.color = Color.green;
    }

    public void SetHovering()
    {
        hovering = true;
    }

    public void ClearHovering()
    {
        hovering = false;
    }

    public void AddTile(GridTile tile)
    {
        tiles.Add(tile);
    }

    public GridTile Tile => tiles[0];
}
