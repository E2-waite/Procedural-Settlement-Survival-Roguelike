using System.Collections;
using UnityEngine;
using static GlobalDefs;
[System.Serializable]
public class AgentSprite
{
    private AgentObject agentSprites;
    Vector2 lastDir = Vector2.zero;
    Vector2 facing = Vector2.zero;
    private SpriteRenderer[] spriteLayers = new SpriteRenderer[(int)SpriteLayers.Max];
    private Color[] startColors = new Color[(int)SpriteLayers.Max];
    public void Init(GameObject body, AgentObject agentSprites)
    {
        for (int i = 0; i < body.transform.childCount; i++)
        {
            spriteLayers[i] = body.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
        this.agentSprites = agentSprites;

        spriteLayers[(int)SpriteLayers.Torso].color = agentSprites.ClothingColor;
        spriteLayers[(int)SpriteLayers.TorsoTrim].color = agentSprites.TrimColor;
        spriteLayers[(int)SpriteLayers.Armour].color = agentSprites.ArmourColor;
        spriteLayers[(int)SpriteLayers.ArmourTrim].color = agentSprites.ArmourTrimColor;
        spriteLayers[(int)SpriteLayers.Head].color = agentSprites.SkinColor;
        spriteLayers[(int)SpriteLayers.Helmet].color = agentSprites.HelmetColor;
        spriteLayers[(int)SpriteLayers.HelmetTrim].color = agentSprites.HelmetTrimColor;
        spriteLayers[(int)SpriteLayers.FrontHand].color = agentSprites.UseSkinColorOnHands ? agentSprites.SkinColor : agentSprites.HandColor;
        spriteLayers[(int)SpriteLayers.BackHand].color = agentSprites.UseSkinColorOnHands ? agentSprites.SkinColor : agentSprites.HandColor;

        for (int i = 0; i < (int)SpriteLayers.Max; i++)
        {
            startColors[i] = spriteLayers[i].color;
        }

        if (agentSprites.ShowHelmet)
        {
            spriteLayers[(int)SpriteLayers.Helmet].enabled = true;
            spriteLayers[(int)SpriteLayers.HelmetTrim].enabled = true;
        }
        else
        {
            spriteLayers[(int)SpriteLayers.Helmet].enabled = false;
            spriteLayers[(int)SpriteLayers.HelmetTrim].enabled = false;
        }

        if (agentSprites.ShowArmour)
        {
            spriteLayers[(int)SpriteLayers.Armour].enabled = true;
            spriteLayers[(int)SpriteLayers.ArmourTrim].enabled = true;
        }
        else
        {
            spriteLayers[(int)SpriteLayers.Armour].enabled = false;
            spriteLayers[(int)SpriteLayers.ArmourTrim].enabled = false;
        }
        SetDirection(new Vector2(1, -1));
    }

    public void SetDirection(Vector2 dir)
    {
        if (agentSprites == null) return;
        if (dir == Vector2.zero) return;

        if (dir.x != 0) facing.x = dir.x;
        facing.y = dir.y;

        dir.Normalize();

        if (dir.magnitude > 0.1f)
        {
            SpriteDir spriteDir = GetSpriteDir(dir);
            for (SpriteLayers i = SpriteLayers.Torso; i < SpriteLayers.Max; i++)
            {
                spriteLayers[(int)i].sprite = agentSprites.GetSprite(spriteDir, i);
            }
        }
    }

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        SetDirection(new Vector2(dir.x, dir.z));
    }

    float lastX = 0, lastY = 0;


    private SpriteDir GetSpriteDir(Vector2 dir)
    {
        if (dir.x == 0) dir.x = lastX;
        else lastX = dir.x;

        if (dir.y == 0) dir.y = lastY;
        else lastY = dir.y;

        bool up = dir.y > 0;
        bool down = dir.y <= 0;
        bool left = dir.x < 0;
        bool right = dir.x > 0;

        SpriteDir SpriteDir = SpriteDir.DownLeft;

        if (up && right) SpriteDir = SpriteDir.UpRight;
        else if (up && left) SpriteDir = SpriteDir.UpLeft;
        else if (down && right) SpriteDir = SpriteDir.DownRight;
        else if (down && left) SpriteDir = SpriteDir.DownLeft;

        return SpriteDir;
    }

    public void ShowOverlay(bool show)
    {
        for (int i = 0; i < (int)SpriteLayers.Max; i++)
        {
            spriteLayers[i].material.SetFloat("_OverlayStrength", show ? 1.0f : 0.0f);
        }
    }
}
