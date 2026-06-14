using UnityEngine;
using static GlobalDefs;
[CreateAssetMenu(fileName = "RoleObject", menuName = "Scriptable Objects/Agent")]
public class AgentObject : ScriptableObject
{
    [SerializeField] private bool showHelmet = true;
    [SerializeField] private bool useSkinColorOnHands = true;
    [SerializeField] private Color clothingColor = Color.white;
    [SerializeField] private Color trimColor = Color.white;
    [SerializeField] private Color skinColor = Color.white;
    [SerializeField] private Color helmetColor = Color.white;
    [SerializeField] private Color helmetTrimColor = Color.white;
    [SerializeField] private Color handColor = Color.white;
    [SerializeField] private Sprite torso;
    [SerializeField] private Sprite[] torsoTrim = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite head;
    [SerializeField] private Sprite[] eyes = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] helmet = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] helmetTrim = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] frontHand = new Sprite[(int)SpriteDir.Max];
    [SerializeField] private Sprite[] backHand = new Sprite[(int)SpriteDir.Max];
    public bool ShowHelmet => showHelmet;
    public bool UseSkinColorOnHands => useSkinColorOnHands;
    public Color ClothingColor => clothingColor;
    public Color TrimColor => trimColor;
    public Color SkinColor => skinColor;
    public Color HandColor => handColor;
    public Color HelmetColor => helmetColor;
    public Color HelmetTrimColor => helmetTrimColor;

    public Sprite GetTorso(SpriteDir dir) => torso;
    public Sprite GetTorsoTrim(SpriteDir dir) => torsoTrim[(int)dir];
    public Sprite GetHead(SpriteDir dir) => head;
    public Sprite GetEyes(SpriteDir dir) => eyes[(int)dir];
    public Sprite GetHelmet(SpriteDir dir) => helmet[(int)dir];
    public Sprite GetHelmetTrim(SpriteDir dir) => helmetTrim[(int)dir];
    public Sprite GetFrontHand(SpriteDir dir) => frontHand[(int)dir];
    public Sprite GetBackHand(SpriteDir dir) => backHand[(int)dir];

    public Sprite GetSprite(SpriteDir dir, SpriteLayers layer)
    {
        switch (layer)
        {
            case SpriteLayers.Torso:
                return GetTorso(dir);
            case SpriteLayers.TorsoTrim:
                return GetTorsoTrim(dir);
            case SpriteLayers.Head:
                return GetHead(dir);
            case SpriteLayers.Eyes:
                return GetEyes(dir);
            case SpriteLayers.Helmet:
                return GetHelmet(dir);
            case SpriteLayers.HelmetTrim:
                return GetHelmetTrim(dir);
            case SpriteLayers.BackHand:
                return GetBackHand(dir);
            case SpriteLayers.FrontHand:
                return GetFrontHand(dir);
        }
        return null;
    }
}