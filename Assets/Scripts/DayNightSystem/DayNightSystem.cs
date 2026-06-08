using UnityEngine;

public class DayNightSystem
{
    public enum DayPhase { Day, Night }
    private DayPhase phase;
    public DayPhase Phase => phase;
    DayNightHandler handler;

    public void Init(GameContext context)
    {
        phase = DayPhase.Day;
        handler = context.dayNightHandler;
        handler.Init(this);
    }

    public void SetPhase(DayPhase phase)
    {
        this.phase = phase;
    }
}
