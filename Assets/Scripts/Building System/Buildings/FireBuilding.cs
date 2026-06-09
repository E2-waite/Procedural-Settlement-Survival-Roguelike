using UnityEngine;

public class FireBuilding : Building
{
    public override TargetType Type => TargetType.Fire;

    [SerializeField] protected Fire fire = new Fire(false);
    public Light fireLight;
    private FireSystem fireSystem;
    public override void Init(GameContext context)
    {
        fireSystem = context.fireSystem;
    }

    protected override void Start()
    {
        base.Start();
        fire.Init(fireLight);
    }

    protected override void FinishBuilding()
    {
        base.FinishBuilding();

        if (fireSystem != null)
        {
            fireSystem.Add(this);
        }
    }

    protected override void OnDeathStart()
    {
        fireSystem.Remove(this);
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
