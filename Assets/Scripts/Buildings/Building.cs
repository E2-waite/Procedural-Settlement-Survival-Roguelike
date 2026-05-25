using UnityEngine;

public class Building : MonoBehaviour
{
    public Vector2Int tilePos;

    bool built = false, broken = false;
    float health = 0, maxHealth = 100;

    public MeshRenderer rend;

    public int[] resourceCost = new int[(int)ResourceNode.Type.Max];


    private void Start()
    {
        if (!Built())
            rend.material.color = Color.red;

        resourceCost[(int)ResourceNode.Type.Wood] = 30;
    }

    public bool Built()
    {
        return built && !broken;
    }

    public bool Build(float val)
    {
        health = Mathf.Clamp(health + val, 0, maxHealth);

        if (health >= maxHealth)
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
            Debug.Log("FINISHED BUILDING");

            return true;
        }
        return false;
    }

    public void Damage(float val)
    {
        health = Mathf.Clamp(health - val, 0, maxHealth);

        if (health <= 0) broken = true;
    }
}
