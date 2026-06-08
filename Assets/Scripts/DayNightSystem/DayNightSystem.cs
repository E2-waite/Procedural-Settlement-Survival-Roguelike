using UnityEngine;

public class DayNightSystem
{
    public enum Phase { Day, Night }
    private Phase phase;
    DayNightHandler handler;

    public void Init(GameContext context)
    {
        phase = Phase.Day;
        handler = context.dayNightHandler;
        handler.Init(this);
    }

    public void SetPhase(Phase phase)
    {
        this.phase = phase;
    }
}
