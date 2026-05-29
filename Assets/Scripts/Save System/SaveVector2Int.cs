using UnityEngine;

[System.Serializable]
public struct SaveVector2Int
{
    int x;
    int y;

    public SaveVector2Int(Vector2Int v)
    {
        x = v.x;
        y = v.y;
    }

    public Vector2Int ToPublic2Int()
    {
        return new Vector2Int(x, y);
    }
}
