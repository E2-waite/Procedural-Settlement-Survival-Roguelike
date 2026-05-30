using UnityEngine;

public class FireBuilding : Building
{
    [SerializeField] protected Fire fire = new Fire(false);
    public Light fireLight;

    protected override void Start()
    {
        base.Start();
        fire.Init(fireLight);
    }

    protected override void FinishBuilding()
    {
        base.FinishBuilding();

        if (FireHandler.Instance != null)
        {
            FireHandler.Instance.Add(this);
        }
    }

    private void Update()
    {
        fire.Update();
    }

    public bool Lit()
    {
        return fire.Lit();
    }

    public void Light(Fire other)
    {
        if (fire != null && other != null)
        {
            if (other.Lit())
            {
                fire.Light();
            }

            if (fire.Lit())
            {
                other.Light();
            }
        }

    }
}
