using UnityEngine;

public class Building : Damageable
{
    public Vector2Int tilePos;
    public bool autoBuild = false;

    public int id;

    bool built = false, destroyed = false;

    public MeshRenderer mesh;

    protected virtual void Start()
    {
        if (!Built())
            mesh.material.color = Color.red;

        if (autoBuild)
        {
            FinishBuilding();
        }
    }

    public void Init(int id)
    {
        this.id = id;
    }

    public bool Built()
    {
        return built && !destroyed;
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
}
