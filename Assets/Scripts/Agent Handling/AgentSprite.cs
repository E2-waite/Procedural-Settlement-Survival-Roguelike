using UnityEngine;
using static GlobalDefs;
[System.Serializable]
public class AgentSprite
{
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer eyesRenderer;
    private AgentObject agentSprites;
    Vector2 lastDir = Vector2.zero;
    Vector2 facing = Vector2.zero;
    public void Init(SpriteRenderer spriteRenderer, SpriteRenderer eyesRenderer, AgentObject agentSprites)
    {
        this.spriteRenderer = spriteRenderer;
        this.eyesRenderer = eyesRenderer;
        this.agentSprites = agentSprites;
        spriteRenderer.sprite = agentSprites.downRight;
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
            AgentDir agentDir = GetAgentDir(dir);
            spriteRenderer.sprite = agentSprites.GetAgentSprite(agentDir);
            eyesRenderer.sprite = agentSprites.GetAgentEyes(agentDir);
        }
    }

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        SetDirection(new Vector2(dir.x, dir.z));
    }
    private Sprite GetEyeSprite(Vector2 dir)
    {
        bool up = dir.y > 0;
        bool down = dir.y <= 0;
        bool left = dir.x < 0;
        bool right = dir.x > 0;

        if (up && right) return agentSprites.upRight;
        else if (up && left) return agentSprites.upLeft;
        else if (down && right) return agentSprites.downRight;

        return agentSprites.downLeft;
    }
    private AgentDir GetAgentDir(Vector2 dir)
    {
        bool up = dir.y > 0;
        bool down = dir.y <= 0;
        bool left = dir.x < 0;
        bool right = dir.x > 0;

        AgentDir agentDir = AgentDir.DownLeft;

        if (up && right) agentDir = AgentDir.UpRight;
        else if (up && left) agentDir = AgentDir.UpLeft;
        else if (down && right) agentDir = AgentDir.DownRight;
        else if (down && left) agentDir = AgentDir.DownLeft;

        return agentDir;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
