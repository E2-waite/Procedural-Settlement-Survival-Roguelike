using UnityEngine;
using static GlobalDefs;
[CreateAssetMenu(fileName = "RoleObject", menuName = "Scriptable Objects/Agent")]
public class AgentObject : ScriptableObject
{
    [SerializeField] private bool showHelmet = true;
    [SerializeField] private bool showArmour = true;
    [SerializeField] private bool useSkinColorOnHands = true;
    [SerializeField] private Color clothingColor = Color.white;
    [SerializeField] private Color trimColor = Color.white;
    [SerializeField] private Color armourColor = Color.white;
    [SerializeField] private Color armourTrimColor = Color.white;
    [SerializeField] private Color skinColor = Color.white;
    [SerializeField] private Color helmetColor = Color.white;
    [SerializeField] private Color helmetTrimColor = Color.white;
    [SerializeField] private Color handColor = Color.white;
    [SerializeField] private Sprite torso;
    [SerializeField] private Sprite[] torsoTrim = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] armour = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] armourTrim = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite head;
    [SerializeField] private Sprite[] eyes = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] helmet = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] helmetTrim = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] backHand = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] frontHand = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] backEquip = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] frontEquip = new Sprite[(int)SpriteDir.Max];

    [SerializeField] private Sprite torsoOutline;
    [SerializeField] private Sprite[] armourOutline = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite headOutline;
    [SerializeField] private Sprite[] helmetOutline = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] backHandOutline = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] frontHandOutline = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] backEquipOutline = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] frontEquipOutline = new Sprite[(int)SpriteDir.Max];

    public bool ShowHelmet => showHelmet;
    public bool ShowArmour => showArmour;
    public bool UseSkinColorOnHands => useSkinColorOnHands;
    public Color ClothingColor => clothingColor;
    public Color TrimColor => trimColor;
    public Color ArmourColor => armourColor;
    public Color ArmourTrimColor => armourTrimColor;
    public Color SkinColor => skinColor;
    public Color HandColor => handColor;
    public Color HelmetColor => helmetColor;
    public Color HelmetTrimColor => helmetTrimColor;

    public Sprite GetSprite(SpriteDir dir, SpriteLayers layer)
    {
        switch (layer)
        {
            case SpriteLayers.Torso:
                return torso;
            case SpriteLayers.TorsoTrim:
                return torsoTrim[(int) dir];
            case SpriteLayers.Armour:
                return armour[(int)dir];
            case SpriteLayers.ArmourTrim:
                return armourTrim[(int)dir];
            case SpriteLayers.Head:
                return head;
            case SpriteLayers.Eyes:
                return eyes[(int)dir];
            case SpriteLayers.Helmet:
                return helmet[(int)dir];
            case SpriteLayers.HelmetTrim:
                return helmetTrim[(int)dir];
            case SpriteLayers.BackHand:
                return backHand[(int)dir];
            case SpriteLayers.FrontHand:
                return frontHand[(int)dir];
            case SpriteLayers.BackEquip:
                return backEquip[(int)dir];
            case SpriteLayers.FrontEquip:
                return frontEquip[(int)dir];
        }
        return null;
    }

    public Sprite GetOutline(SpriteDir dir, SpriteLayers layer)
    {
        switch (layer)
        {
            case SpriteLayers.Torso:
                return torsoOutline;
            case SpriteLayers.Armour:
                return armourOutline[(int)dir];
            case SpriteLayers.Head:
                return headOutline;
            case SpriteLayers.Helmet:
                return helmetOutline[(int)dir];
            case SpriteLayers.BackHand:
                return backHandOutline[(int)dir];
            case SpriteLayers.FrontHand:
                return frontHandOutline[(int)dir];
            case SpriteLayers.BackEquip:
                return backEquipOutline[(int)dir];
            case SpriteLayers.FrontEquip:
                return frontEquipOutline[(int)dir];
        }
        return null;
    }
}