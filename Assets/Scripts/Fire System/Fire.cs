using UnityEngine;

[System.Serializable]
public class Fire
{
    public float depletionRate = 0.001f;
    const float maxLevel = 100f;
    public float level;
    bool extinguished = true;
    FireEffect effect;

    public Fire(bool lit)
    {
        if (lit)
        {
            level = maxLevel;
            extinguished = false;
        }
    }

    public void Init(FireEffect effect)
    {
        this.effect = effect;
    }

    public void Update()
    {
        if (level > 0f) level -= depletionRate * Time.deltaTime;

        if (level <= 0f && !extinguished)
        {
            Extinguish();
        }

        if (!extinguished)
        {
            float v = Mathf.Clamp(level, 0, maxLevel) / maxLevel;
            effect.UpdateEffect(level, maxLevel);
        }
    }

    public void Light()
    {
        level = maxLevel;
        extinguished = false;
    }

    public void Light(ref float fuel)
    {
        float space = maxLevel - level;

        if (level + fuel > maxLevel)
        {
            float diff = maxLevel - level;
            level = maxLevel;
            fuel -= diff;
        }
        else
        {
            level += fuel;
            fuel = 0;
        }

        extinguished = false;
    }

    void Extinguish()
    {
        extinguished = true;
        effect.Extinguish();
        Debug.Log("Fire extinguished");
    }

    public bool Depleted()
    {
        return level <= 0;
    }

    public bool Lit()
    {
        return level > 0;
    }
}
