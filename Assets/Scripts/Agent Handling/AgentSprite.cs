using UnityEngine;

[System.Serializable]
public class AgentSprite
{
    private SpriteRenderer spriteRenderer;
    private AgentObject agentSprites;
    Vector2 lastDir = Vector2.zero;
    Vector2 facing = Vector2.zero;
    public void Init(SpriteRenderer spriteRenderer, AgentObject agentSprites)
    {
        this.spriteRenderer = spriteRenderer;
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
            spriteRenderer.sprite = GetDiagonalSprite(facing);
    }

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        SetDirection(new Vector2(dir.x, dir.z));
    }

    private Sprite GetDiagonalSprite(Vector2 dir)
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

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
