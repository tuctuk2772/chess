using UnityEngine;
using Chess;
using System.Collections.Generic;

public class SelectSquares
{
    public bool dragged;
    public bool movedOutsideBounds = false;

    public bool snapped = false;

    public GameObject draggedPiece;
    public List<SquareInfo> squares;

    private SquareInfo current;
    private SquareInfo previous;

    private bool ValidityChecks(ref Vector2Int squarePosition, ref BoardManager boardManager)
    {
        Juice_Square.UnMarkSquare(boardManager.allValidSquares);

        boardManager.allValidSquares.Clear();
        boardManager.validCaptureSquares.Clear();
        boardManager.validEmptySquares.Clear();

        //reset hovered square colors
        foreach (var square in squares)
        {
            Juice_Square.SquareUnHover(square.gameObject);
        }

        //highlight squares that are valid
        if (squarePosition == new Vector2Int(-1, -1))
        {
            return false;
        }

        return true;
    }

    public void SelectSquare(Vector2Int squarePosition)
    {
        //this is before valid, because the selected squares always need to change, even if null
        BoardManager boardManager = GameManager.instance.boardManager;

        current = UniversalFunctions.GetSquare(squarePosition.x, squarePosition.y);
        previous = boardManager.currentSelectedSquare;

        boardManager.previousSelectedSquare = previous;
        boardManager.currentSelectedSquare = current;

        boardManager.clickAgain = false;

        if (!ValidityChecks(ref squarePosition, ref boardManager))
        {
            return;
        }

        if (boardManager.currentSelectedSquare == boardManager.previousSelectedSquare)
        {
            boardManager.clickAgain = true;
        }

        //checks if the currentSelectedSquare has a valid chess piece in the GetValidMoves
        boardManager.getValidSquares.GetValidMoves(current);

        if (boardManager.allValidSquares != null && boardManager.allValidSquares.Count > 0)
        {
            Juice_Square.MarkSquare(boardManager.allValidSquares);
        }

        previous = current;

        if (current.pieceGameObject == null)
        {
            dragged = false;
            return;
        }

        Juice_Square.ChangeSquareColor(current.gameObject, GameManager.instance.boardManager.boardData.selected, true);
        dragged = true;
        draggedPiece = current.pieceGameObject;
    }

    public void DeSelectSquare(Vector2Int squarePosition)
    {
        BoardManager boardManager = GameManager.instance.boardManager;

        ValidityChecks(ref squarePosition, ref boardManager);

        //if player clicked the same selected square, deactivates selection
        if (boardManager.currentSelectedSquare == boardManager.previousSelectedSquare ||
            boardManager.currentSelectedSquare == null)
        {
            boardManager.clickAgain = true;

            if (boardManager.previousSelectedSquare != null)
            {
                Juice_Square.ChangeSquareColor(boardManager.previousSelectedSquare.gameObject,
                boardManager.previousSelectedSquare.gameObject.GetComponent<SpriteRenderer>().material.GetColor("_DefaultColor"),
                false);
            }

            if (boardManager.allValidSquares != null)
            {
                Juice_Square.UnMarkSquare(boardManager.allValidSquares);
            }

            boardManager.currentSelectedSquare = null;
            boardManager.previousSelectedSquare = null;
        }

        draggedPiece = null;
        dragged = false;
    }

    public void StopDrag(Vector2Int squarePosition)
    {
        dragged = false;
        movedOutsideBounds = false;

        if (draggedPiece == null)
        {
            return;
        }

        Juice_Piece.ReturnPiece(draggedPiece);
        draggedPiece = null;
    }

    public void DragPiece(Vector2 mousePosition, Transform movementBounds)
    {
        if (draggedPiece == null)
        {
            return;
        }

        Vector2 center = movementBounds.position;
        Vector2 size = movementBounds.localScale / 2f;

        //no point in shifting if the player hasn't moved the piece off of the square - assume they just quick-tapped
        bool stillInsideOriginalSquare = SquareDistanceCheck(
            SquareDistance(mousePosition, center), ref size, 1.15f);

        if (stillInsideOriginalSquare)
        {
            if (movedOutsideBounds)
            {
                Juice_Piece.SnapPieceToSquare(ref draggedPiece, center);
            }
            else
            {
                Juice_Piece.TapPiece(draggedPiece);
            }

            return;
        }

        movedOutsideBounds = true;

        //snaps dragged piece into valid spots
        BoardManager boardManager = GameManager.instance.boardManager;

        if (boardManager.allValidSquares.Contains(boardManager.currentHoveredSquare))
        {
            Vector2 validSquareCenter = boardManager.currentHoveredSquare.transform.position;
            Vector2 validSquareSize = boardManager.currentHoveredSquare.transform.localScale / 2f;

            bool nearValidSquare = SquareDistanceCheck(
                SquareDistance(mousePosition, validSquareCenter), ref validSquareSize, 0.5f);

            if (nearValidSquare)
            {
                //might bring this back, right now it changes color when piece is hovered over, not when it snaps
                /*Color snappedColor = Color.red; //red means error

                snappedColor =
                    UniversalFunctions.CheckIfSquareValidCapture(ref boardManager.currentHoveredSquare)
                    ? boardManager.boardData.validCaptureMove : boardManager.boardData.validEmptyMove;

                Juice_Square.SquareHover(boardManager.currentHoveredSquare.gameObject, snappedColor);*/

                snapped = true;
                if (boardManager.currentHoveredSquare.pieceGameObject != null)
                {
                    Juice_Piece.PieceFade(ref boardManager.currentHoveredSquare.pieceGameObject);
                }
                Juice_Piece.SnapPieceToSquare(ref draggedPiece, validSquareCenter);
            }

            if (snapped && !nearValidSquare)
            {
                snapped = false;
                if (boardManager.currentHoveredSquare.pieceGameObject != null)
                {
                    Juice_Piece.PieceUnFade(ref boardManager.currentHoveredSquare.pieceGameObject);
                }
            }
        }

        if (boardManager.allValidSquares.Contains(boardManager.previousHoveredSquare))
        {
            Juice_Square.SquareUnHover(boardManager.previousHoveredSquare.gameObject);
        }

        Vector3 clampedPosition = GameManager.instance.inputManager.trackMousePos.ClampToBoard(mousePosition);
        Juice_Piece.MovePieceAround(ref draggedPiece, ref clampedPosition,
            GameManager.instance.inputManager.trackMousePos.MouseVelocity());
    }

    public Vector2 SquareDistance(Vector2 mousePosition, Vector2 center)
    {
        Vector2 distance = new Vector2(
            Mathf.Abs(mousePosition.x - center.x),
            Mathf.Abs(mousePosition.y - center.y)
            );

        return distance;
    }

    private bool SquareDistanceCheck(Vector2 distance, ref Vector2 size, float decimalOfSquare)
    {
        return distance.x < size.x * decimalOfSquare && distance.y < size.y * decimalOfSquare;
    }
}
