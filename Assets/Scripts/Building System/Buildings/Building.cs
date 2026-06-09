using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;
public class Building : Destructable
{
    protected Faction owner;
    public virtual Faction Owner => Faction.Unit;
    public override TargetType Type => TargetType.Building;

    [SerializeField] bool prebuild = false;
    public int id;
    bool built = false, destroyed = false;
    public bool Built => built && !destroyed;
    public MeshRenderer mesh;
    private bool hovering = false;
    private List<GridTile> tiles = new List<GridTile>();

    public virtual void Init(GameContext context) { }

    protected virtual void Start()
    {
        if (!Built)
            mesh.material.color = Color.red;

        if (prebuild)
        {
            FinishBuilding();
        }
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
        built = true;
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
