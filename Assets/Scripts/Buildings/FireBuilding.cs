using UnityEngine;

public class FireBuilding : Building
{
    protected Fire fire = new Fire(false);
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
}
