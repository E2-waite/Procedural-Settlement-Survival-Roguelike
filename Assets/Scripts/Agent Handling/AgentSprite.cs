using UnityEngine;

[System.Serializable]
public class AgentSprite
{
    public SpriteRenderer spriteRenderer;
    public Sprite upRight;
    public Sprite upLeft;
    public Sprite downRight;
    public Sprite downLeft;

    public void Init(SpriteRenderer spriteRenderer)
    {
        this.spriteRenderer = spriteRenderer;
    }

    public void SetDirection(Vector2 dir)
    {
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

        if (up && right) return upRight;
        if (up && !right) return upLeft;
        if (!up && right) return downRight;
        return downLeft;
    }

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }
}
