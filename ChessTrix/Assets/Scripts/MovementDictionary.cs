using System.Collections.Generic;
using UnityEngine;
using static ChessPieceData;

public static class MovementDictionary
{
    public static List<Vector2Int> GetOffsets(MovementType type)
    {
        switch (type)
        {
            case MovementType.Horizontal:
                return new List<Vector2Int>() { Vector2Int.left, Vector2Int.right };
            case MovementType.Vertical:
                return new List<Vector2Int>() { Vector2Int.up, Vector2Int.down };
            case MovementType.Diagonal:
                return new List<Vector2Int>
                {
                    new Vector2Int(1, 1), new Vector2Int(-1, 1),
                    new Vector2Int(-1, -1), new Vector2Int(1, -1)
                };
            case MovementType.LShape:
                return new List<Vector2Int>
                {
                    new Vector2Int(2, 1), new Vector2Int(1, 2),
                    new Vector2Int(-2, 1), new Vector2Int(-1, 2),
                    new Vector2Int(2, -1), new Vector2Int(1, -2),
                    new Vector2Int(-2, -1), new Vector2Int(-1, -2)
                };
            case MovementType.SingleSpace:
                return new List<Vector2Int> {
                    Vector2Int.up, Vector2Int.down,
                    Vector2Int.left, Vector2Int.right,
                    new Vector2Int(1, 1), new Vector2Int(-1, 1),
                    new Vector2Int(1, -1), new Vector2Int(-1, -1)
                };
            case MovementType.PawnSpecific:
                return new List<Vector2Int>() { Vector2Int.down };
            default: return new List<Vector2Int>();
        }
    }
}
