using UnityEngine;

[System.Serializable]
public class AgentSprite
{
    private SpriteRenderer spriteRenderer;
    private AgentObject agentSprites;

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

        dir.Normalize();

        spriteRenderer.sprite = GetDiagonalSprite(dir);
    }

    public void SetDirection(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        SetDirection(new Vector2(dir.x, dir.z));
    }

    private Sprite GetDiagonalSprite(Vector2 dir)
    {
        bool up = dir.y > 0;
        bool right = dir.x > 0;

        if (up && right) return agentSprites.upRight;
        if (up && !right) return agentSprites.upLeft;
        if (!up && right) return agentSprites.downRight;
        return agentSprites.downLeft;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
