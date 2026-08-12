using UnityEngine;
using static GlobalDefs;
[CreateAssetMenu(fileName = "RoleObject", menuName = "Scriptable Objects/Agent")]
public class AgentObject : ScriptableObject
{
    public enum RoleType
    {
        None,
        Melee,
        Ranged,
        Max
    }


    [SerializeField] private RoleType roleType = RoleType.None;

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

    public RoleType AgentType => roleType;
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
        return layer switch
        {
            SpriteLayers.Torso => torso,
            SpriteLayers.TorsoTrim => torsoTrim[(int)dir],
            SpriteLayers.Armour => armour[(int)dir],
            SpriteLayers.ArmourTrim => armourTrim[(int)dir],
            SpriteLayers.Head => head,
            SpriteLayers.Eyes => eyes[(int)dir],
            SpriteLayers.Helmet => helmet[(int)dir],
            SpriteLayers.HelmetTrim => helmetTrim[(int)dir],
            SpriteLayers.BackHand => backHand[(int)dir],
            SpriteLayers.FrontHand => frontHand[(int)dir],
            SpriteLayers.BackEquip => backEquip[(int)dir],
            SpriteLayers.FrontEquip => frontEquip[(int)dir],
            _ => null
        };  
    }

    public Sprite GetOutline(SpriteDir dir, SpriteLayers layer)
    {
        return layer switch
        {
            SpriteLayers.Torso => torsoOutline,
            SpriteLayers.Armour => armourOutline[(int)dir],
            SpriteLayers.Head => headOutline,
            SpriteLayers.Helmet => helmetOutline[(int)dir],
            SpriteLayers.BackHand => backHandOutline[(int)dir],
            SpriteLayers.FrontHand => frontHandOutline[(int)dir],
            SpriteLayers.BackEquip => backEquipOutline[(int)dir],
            SpriteLayers.FrontEquip => frontEquipOutline[(int)dir],
            _ => null
        };
    }
}