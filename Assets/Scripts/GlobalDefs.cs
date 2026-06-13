using UnityEngine;

public static class GlobalDefs
{
    public enum Faction
    {
        Unit,
        Enemy
    }

    public enum CombatType
    {
        Melee,
        Ranged
    }

    public enum AgentDir
    {
        DownLeft,
        DownRight,
        UpLeft,
        UpRight
    }
}
