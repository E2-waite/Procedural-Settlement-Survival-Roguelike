using System;
using UnityEngine;

public class DayNightSystem
{
    public enum DayPhase { Day, Night }
    private DayPhase phase;
    public DayPhase Phase => phase;
    DayNightHandler handler;

    public event Action<DayPhase> PhaseChanged;

    public void Init(GameContext context)
    {
        phase = DayPhase.Day;
        handler = context.dayNightHandler;
        handler.Init(this);
    }

    public void SetPhase(DayPhase phase)
    {
        if (this.phase == phase) return;
        this.phase = phase;

        PhaseChanged?.Invoke(phase);
    }
}
