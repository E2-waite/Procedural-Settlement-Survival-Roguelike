using UnityEngine;

[System.Serializable]
public class Fire
{
    public float depletionRate = 0.001f;
    const float maxLevel = 1000f;
    public float level;
    bool extinguished = true;
    Light light;

    public Fire(bool lit)
    {
        if (lit)
        {
            level = maxLevel;
            extinguished = false;
        }
    }

    public void Init(Light light)
    {
        this.light = light;
    }

    public void Update()
    {
        if (level > 0f) level -= depletionRate * Time.deltaTime;

        if (level <= 0f && !extinguished)
        {
            Extinguish();
        }

        float v = Mathf.Clamp(level, 0, maxLevel) / maxLevel;
        light.intensity = Mathf.Lerp(0, 10, v);
    }

    public void Light()
    {
        level = maxLevel;
        extinguished = false;

        light.intensity = 10f;
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
        light.intensity = 10f;
    }

    void Extinguish()
    {
        extinguished = true;
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
