using UnityEngine;

public static class GlobalDefs
{
    public enum Faction
    {
        Friendly,
        Neutral,
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
        Torso,
        TorsoTrim,
        Armour,
        ArmourTrim,
        BackHand,
        FrontHand,
        Head,
        Eyes,
        Helmet,
        HelmetTrim,
        BackEquip,
        FrontEquip,
        Max
    }
}
