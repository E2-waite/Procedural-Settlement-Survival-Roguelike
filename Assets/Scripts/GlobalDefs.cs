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

    public enum SpriteDir
    {
        DownRight, DownLeft, UpRight, UpLeft, Max
    }

    public enum SpriteLayers
    {
        BackHand,
        Torso,
        TorsoTrim,
        FrontHand,
        Head,
        Eyes,
        Helmet,
        HelmetTrim,
        Max
    }
}
