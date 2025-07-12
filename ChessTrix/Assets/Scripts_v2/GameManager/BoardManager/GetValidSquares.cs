using System.Collections.Generic;
using UnityEngine;
using Chess;
using Unity.VisualScripting;

public class GetValidSquares
{
    public BoardManager boardManager;
    RuntimeChessPieceData originPiece;

    public void GetValidMoves(SquareInfo originSquare)
    {
        boardManager.allValidSquares.Clear();
        boardManager.validCaptureSquares.Clear();
        boardManager.validEmptySquares.Clear();

        originPiece = null;

        //sanitize inputs
        if (originSquare.pieceGameObject == null)
        {
            return;
        }

        originPiece = originSquare.pieceData;

        foreach (var moveType in originPiece.movements)
        {
            if (moveType == ChessPieceData.MovementType.PawnSpecific)
            {
                PawnSpecificChecks(ref originSquare.pieceData, moveType, ref originSquare.squarePosition);
                continue;
            }

            if (moveType == ChessPieceData.MovementType.LShape)
            {
                CheckDirections(originPiece, moveType, originSquare.squarePosition, 1);
                continue;
            }

            CheckDirections(originPiece, moveType, originSquare.squarePosition, 8);
        }

        boardManager.allValidSquares.AddRange(boardManager.validCaptureSquares);
        boardManager.allValidSquares.AddRange(boardManager.validEmptySquares);
    }

    private void CheckDirections(RuntimeChessPieceData piece, ChessPieceData.MovementType movementType,
        Vector2Int originSquare, int maxChecks)
    {
        var directions = PieceMovementReference.movements(movementType);

        foreach (var direction in directions)
        {
            Vector2Int current = originSquare + direction;
            for (int i = 0; i < maxChecks; i++)
            {
                SquareInfo target = UniversalFunctions.GetSquare(current.x, current.y);

                if (target == null)
                {
                    continue;
                }

                if (target.pieceGameObject != null)
                {
                    if (target.pieceData.pieceColor != originPiece.pieceColor)
                    {
                        boardManager.validCaptureSquares.Add(target);
                    }
                    break;
                }

                boardManager.validEmptySquares.Add(target);

                current += direction;
            }
        }
    }

    private void PawnSpecificChecks(ref RuntimeChessPieceData pawn, ChessPieceData.MovementType movementType,
        ref Vector2Int originSquare)
    {
        //pawn attacks
        Vector2Int[] diagonals =
        {
            originSquare + new Vector2Int(-1, -1),
            originSquare + new Vector2Int(1, -1),
        };

        foreach (var diagonal in diagonals)
        {
            SquareInfo diagSquare = UniversalFunctions.GetSquare(diagonal.x, diagonal.y);

            if (diagSquare != null &&
                diagSquare.pieceGameObject != null &&
                diagSquare.pieceData.pieceColor != pawn.pieceColor)
            {
                boardManager.validCaptureSquares.Add(diagSquare);
            }
        }

        //pawn movement down
        if (pawn.moveAmount == 0)
        {
            CheckDirections(pawn, movementType, originSquare, 2);
            return;
        }

        CheckDirections(pawn, movementType, originSquare, 1);
    }
}
