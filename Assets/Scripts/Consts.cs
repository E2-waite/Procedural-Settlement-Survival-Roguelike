using UnityEngine;

public static class Consts
{
    public const int IDLE_STATE = 0;
    public const int MOVING_STATE = 1;
    public const int FOLLOWING_STATE = 2;
    public const int ATTACKING_STATE = 3;

    public static readonly Vector2Int[] ADJ_NEIGHBOURS =
    {
    new Vector2Int(0, -1),
    new Vector2Int(0, 1),
    new Vector2Int(-1, 0),
    new Vector2Int(1, 0)
    };

    public static readonly Vector2Int[] DIAG_NEIGHBOURS =
    {
    new Vector2Int(-1, -1),
    new Vector2Int(1, -1),
    new Vector2Int(1, 1),
    new Vector2Int(-1, 1)
    };

    public static readonly Vector2Int[] ALL_NEIGHBOURS =
    {
    new Vector2Int(0, -1),
    new Vector2Int(0, 1),
    new Vector2Int(-1, 0),
    new Vector2Int(1, 0),
    new Vector2Int(-1, -1),
    new Vector2Int(1, -1),
    new Vector2Int(1, 1),
    new Vector2Int(-1, 1)
    };
}
