using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;
public class Building : Destructable
{
    protected Faction owner;
    public virtual Faction Faction => Faction.Friendly;
    public override TargetType Type => TargetType.Building;

    [SerializeField] bool preBuild = false;
    public virtual bool PreBuild => preBuild;
    public int id;
    bool built = false, destroyed = false;
    public bool Built => built && !destroyed;
    private MeshRenderer mesh;
    private bool hovering = false;
    protected List<GridTile> tiles = new List<GridTile>();
    private Material outlineMat;

    public virtual void Init(GameContext context) { }

    protected virtual void Start()
    {
        if (transform.childCount > 0)
        {
            mesh = transform.GetChild(0).GetComponent<MeshRenderer>();
            outlineMat = mesh.materials[1];

            if (outlineMat != null)
            {
                outlineMat.SetColor("_OutlineColor", Faction == Faction.Enemy ? Color.red : Color.green);
            }
        }

        if (!Built && mesh != null)
            mesh.material.color = Color.red;

        if (preBuild)
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

    public void Highlight(bool active)
    {
        outlineMat.SetFloat("_OutlineWidth", active ? 4 : 0);
    }

    //public void SetHovering()
    //{
    //    hovering = true;
    //    outlineMat.SetFloat("_OutlineWidth", 4);
    //}

    //public void ClearHovering()
    //{
    //    hovering = false;
    //    outlineMat.SetFloat("_OutlineWidth", 0);
    //}

    public void AddTile(GridTile tile)
    {
        tiles.Add(tile);
    }

    public GridTile Tile => tiles[0];
}
