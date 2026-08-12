using UnityEngine;

namespace Cinderwild.World.Data
{
    public enum SurfaceType
    {
        Water,
        Sand,
        Grass,
        Stone
    }

    public enum TopoType
    {
        Sloped,
        Stepped,
        Smooth
    }

    public enum VertexDir
    {
        BL,
        TL,
        TR,
        BR,
        Max
    }

    public enum AdjDirs
    {
        Up,
        Right,
        Down,
        Left
    }

    public static class Defs
    {
        public static readonly Vector2Int[] corners =
        {
            new Vector2Int(0, 0),   // Bottom left
            new Vector2Int(0, 1),   // Top Left
            new Vector2Int(1, 1),   // Top Right
            new Vector2Int(1, 0)    // Bottom Right
        };

        public static readonly Vector2Int[] adjacent =
{
            Vector2Int.up,
            Vector2Int.right,
            Vector2Int.down,
            Vector2Int.left
        };

        public static readonly Vector2Int[] diagonal =
        {
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1)
        };
    }
}