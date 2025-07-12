using UnityEngine;
using System.Collections.Generic;
using Chess;
using System.Linq;

public class MoveAllPieces
{
    public void MoveAllPiecesDirection(
        GameObject exception = null,
        Vector2Int? direction = null,
        System.Action onComplete = null)
    {
        Vector2Int moveDirection = direction ?? Vector2Int.down; //by default, make it down

        if (Mathf.Abs(moveDirection.x) != Mathf.Abs(moveDirection.y)
            && moveDirection.x != 0 && moveDirection.y != 0)
        {
            int maxValue = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.y));

            maxValue *= Mathf.Max((int)Mathf.Sign(moveDirection.x), (int)Mathf.Sign(moveDirection.y));

            Debug.Log($"You didn't listen! Switching ({moveDirection.x}, {moveDirection.y}) to ({maxValue}, {maxValue})!");
            moveDirection = new Vector2Int(maxValue, maxValue);
        }

        Juice_Square.SquareMoveDirection(GameManager.instance.boardManager.allSquares, onComplete, moveDirection);

        List<SquareInfo> sortedSquares = new List<SquareInfo>();
        List<RuntimeChessPieceData> pieces = new List<RuntimeChessPieceData>();

        int clampedDirX = Mathf.Clamp(moveDirection.x, -1, 1);
        int clampedDirY = Mathf.Clamp(moveDirection.y, -1, 1);

        Vector2Int clampedDir = new Vector2Int(clampedDirX, clampedDirY);

        sortedSquares = SortSquares(GameManager.instance.boardManager.allSquares, clampedDir);
        sortedSquares.Reverse(); //it was easier to collect info in reverse and then flip it for diagonals

        foreach (var square in sortedSquares)
        {
            if (square.pieceGameObject == null)
            {
                continue;
            }

            pieces.Add(square.pieceData);
        }

        foreach (var pieceData in pieces)
        {
            GameObject pieceObject = pieceData.gameObject;

            if (pieceObject == exception)
            {
                continue;
            }

            SquareInfo originalSquare = pieceData.currentSquare;
            Vector2Int originalSquarePos = originalSquare.squarePosition;

            Vector2Int targetSquarePos = originalSquarePos;

            Vector2Int signDir = new Vector2Int(
                moveDirection.x == 0 ? 0 : (int)Mathf.Sign(moveDirection.x),
                moveDirection.y == 0 ? 0 : (int)Mathf.Sign(moveDirection.y)
                );

            int steps = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.y));

            for (int i = 1; i <= steps; i++)
            {
                Vector2Int checkPos = originalSquarePos + new Vector2Int(i * signDir.x, i * signDir.y);
                SquareInfo checkSquare = UniversalFunctions.GetSquare(checkPos.x, checkPos.y);

                if (checkSquare != null)
                {
                    if (checkSquare.pieceGameObject == null) //separated due to potential modifiers
                    {
                        targetSquarePos = checkSquare.squarePosition;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            Vector2Int pieceOffset = targetSquarePos - originalSquarePos;
            SquareInfo targetSquare = UniversalFunctions.GetSquare(targetSquarePos.x, targetSquarePos.y);

            //clear out junk
            originalSquare.pieceGameObject = null;
            originalSquare.pieceData = null;

            pieceData.currentSquare = targetSquare;
            targetSquare.pieceData = pieceData;
            targetSquare.pieceGameObject = pieceObject;

            Juice_Piece.PieceMoveDirection(pieceObject, moveDirection, pieceOffset, () =>
            {
                pieceObject.transform.parent = targetSquare.transform;
                pieceObject.transform.localPosition = new Vector3(0, 0, -1f);
            });

        }
    }

    private List<SquareInfo> SortSquares(List<SquareInfo> squares, Vector2Int clampedDir)
    {
        List<SquareInfo> sorted = new List<SquareInfo>();

        int xStart = clampedDir.x < 0 ? 7 : 0;
        int yStart = clampedDir.y < 0 ? 7 : 0;

        Vector2Int startPos = new Vector2Int(xStart, yStart);

        if (clampedDir.x == 0 && clampedDir.y == 0)
        {
            sorted = SortRowsAndColumns(squares, ref clampedDir, ref startPos);
        }
        else
        {
            sorted = SortDiagonals(squares, startPos);
        }

        return sorted;
    }

    private List<SquareInfo> SortRowsAndColumns(List<SquareInfo> squares, ref Vector2Int clampedDir, ref Vector2Int startPos)
    {
        List<SquareInfo> result = squares;

        // Determine scan order (row-major: X fast, Y slow)
        bool xFast = true;

        // Determine direction for X
        int xEnd = clampedDir.x < 0 ? -1 : 8;
        int xStep = clampedDir.x < 0 ? -1 : 1;

        // Determine direction for Y
        int yEnd = clampedDir.y < 0 ? -1 : 8;
        int yStep = clampedDir.y < 0 ? -1 : 1;

        if (clampedDir.x == 0) // If x is 0, then we're scanning vertically, y is fast
        {
            xFast = true;
        }

        else if (clampedDir.y == 0) // If y is 0, we're scanning horizontally, x is fast
        {
            xFast = false;
        }

        // Row-major scan: X inside, Y outside
        if (xFast)
        {
            for (int y = startPos.y; y != yEnd; y += yStep)
            {
                for (int x = startPos.x; x != xEnd; x += xStep)
                {
                    result.Add(UniversalFunctions.GetSquare(x, y));
                }
            }
        }
        else // Column-major scan: Y inside, X outside
        {
            for (int x = startPos.x; x != xEnd; x += xStep)
            {
                for (int y = startPos.y; y != yEnd; y += yStep)
                {
                    result.Add(UniversalFunctions.GetSquare(x, y));
                }
            }
        }

        return result;
    }

    private List<SquareInfo> SortDiagonals(List<SquareInfo> squares, Vector2Int startPos)
    {
        List<SquareInfo> result = squares;

        result.Sort((a, b) =>
        {
            float distA = Vector2.Distance(a.squarePosition, startPos);
            float distB = Vector2.Distance(b.squarePosition, startPos);
            return distA.CompareTo(distB);
        });

        return result;
    }
}
