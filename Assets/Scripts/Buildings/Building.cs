using UnityEngine;

public class Building : MonoBehaviour
{
    [SerializeField] Health health = new Health();
    public Vector2Int tilePos;

    public int id;

    public void Init(int id)
    {
        this.id = id;
    }

    bool built = false, broken = false;

    public MeshRenderer rend;

    private void Start()
    {
        if (!Built())
            rend.material.color = Color.red;
    }

    public bool Built()
    {
        return built && !broken;
    }

    public bool Build(float val)
    {
        health.Heal(val);

        if (health.Full())
        {
            if (!built)
            {
                built = true;
            }
            else
            {
                broken = false;
            }
            rend.material.color = Color.green;

            return true;
        }
        return false;
    }

    public void Damage(float val)
    {
        health.Damage(val);
        if (health.Empty()) broken = true;
    }


}
