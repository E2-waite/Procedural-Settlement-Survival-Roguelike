using System.Collections;
using UnityEngine;
using static GlobalDefs;
[System.Serializable]
public class AgentSprite
{
    private AgentObject agentSprites;
    private SpriteRenderer[] outlineLayers = new SpriteRenderer[(int)SpriteLayers.Max];
    private SpriteRenderer[] spriteLayers = new SpriteRenderer[(int)SpriteLayers.Max];
    private Color[] startColors = new Color[(int)SpriteLayers.Max];
    bool hasOutline = false;
    public void Init(Transform body, Transform outline, AgentObject agentSprites)
    {
        for (int i = 0; i < body.transform.childCount; i++)
        {
            spriteLayers[i] = body.transform.GetChild(i).GetComponent<SpriteRenderer>();
        }

        hasOutline = outline != null;

        if (hasOutline)
        {
            for (int i = 0; i < outline.transform.childCount; i++)
            {
                outlineLayers[i] = outline.transform.GetChild(i).GetComponent<SpriteRenderer>();
            }
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
        SetDirection(new Vector3(1, 0, -1));
    }

    public void SetDirection(Vector3 dir)
    {
        if (agentSprites == null) return;

        SpriteDir spriteDir = GetSpriteDir(Quaternion.Euler(0, -45, 0) * dir);
        for (SpriteLayers i = SpriteLayers.Torso; i < SpriteLayers.Max; i++)
        {
            spriteLayers[(int)i].sprite = agentSprites.GetSprite(spriteDir, i);

            if (hasOutline)
            {
                Sprite outlineSprite = agentSprites.GetOutline(spriteDir, i);
                if (outlineSprite != null)
                {
                    outlineLayers[(int)i].sprite = outlineSprite;
                }
            }
        }
    }

    //public void SetDirection(Vector3 dir)
    //{
    //    if (dir == Vector3.zero) return;

    //    SetDirection(new Vector2(dir.x, dir.z));
    //}

    float lastX = 0, lastY = 0;


    private SpriteDir GetSpriteDir(Vector3 dir)
    {
        SpriteDir spriteDir = SpriteDir.DownLeft;


        if (dir.z > 0f)
        {
            if (dir.x > 0f) spriteDir = SpriteDir.UpRight;
            else if (dir.x < 0f) spriteDir = SpriteDir.UpLeft;
        }
        else if (dir.z < 0f)
        {
            if (dir.x > 0f) spriteDir = SpriteDir.DownRight;
            else if (dir.x < 0f) spriteDir = SpriteDir.DownLeft;
        }

        return spriteDir;
    }

    public void ShowOverlay(bool show)
    {
        for (int i = 0; i < (int)SpriteLayers.Max; i++)
        {
            spriteLayers[i].material.SetFloat("_OverlayStrength", show ? 1.0f : 0.0f);
        }
    }

    public void OutlineColour(Color color)
    {
        if (!hasOutline) return;

        for (SpriteLayers i = SpriteLayers.Torso; i < SpriteLayers.Max; i++)
        {
            if (outlineLayers[(int)i] != null) outlineLayers[(int)i].color = color;
        }
    }

    public void ShowOutline(bool show)
    {
        if (!hasOutline) return;

        for (SpriteLayers i = SpriteLayers.Torso; i < SpriteLayers.Max; i++)
        {
            if ((i == SpriteLayers.Helmet && !agentSprites.ShowHelmet) || (i == SpriteLayers.Armour && !agentSprites.ShowArmour)) continue;

            if (outlineLayers[(int)i] != null) outlineLayers[(int)i].enabled = show;
        }
    }

    public void SetOutlineColor(Color color)
    {
        if (!hasOutline) return;

        for (SpriteLayers i = SpriteLayers.Torso; i < SpriteLayers.Max; i++)
        {
            if (outlineLayers[(int)i] != null) outlineLayers[(int)i].color = color;
        }
    }
}

