using UnityEngine;
using Chess;
using Unity.VisualScripting;
using System.Net;

public class HoverSquares
{
    public BoardManager boardManager;

    public SquareInfo HoveredSquare(Vector2Int squarePosition)
    {
        return UniversalFunctions.GetSquare(
            squarePosition.x, squarePosition.y);
    }

    public void UpdateHover(Vector2Int squarePosition, Vector2 mouseWorldPos)
    {
        TrackCurrentHoveredSquares(ref squarePosition);
        VisualizeHoveredSquare();
    }

    private void TrackCurrentHoveredSquares(ref Vector2Int squarePosition)
    {
        //if mouse goes out of bounds
        if (squarePosition.x < 0 || squarePosition.y < 0 || squarePosition.x > 7 || squarePosition.y > 7)
        {
            if (boardManager.currentHoveredSquare == null)
            {
                return;
            }

            if (boardManager.currentHoveredSquare != boardManager.currentSelectedSquare)
            {
                Juice_Square.SquareUnHover(boardManager.currentHoveredSquare.gameObject);

                if (boardManager.currentHoveredSquare.pieceGameObject != null)
                {
                    GameObject prevPiece = boardManager.currentHoveredSquare.pieceGameObject;
                    Juice_Piece.PieceUnHover(ref prevPiece);
                }
            }

            boardManager.previousHoveredSquare = boardManager.currentHoveredSquare;
            boardManager.currentHoveredSquare = null;
            return;
        }

        //check if hoveredSquare and previousHoveredSquare are the same or different
        SquareInfo currentHoveredSquare = HoveredSquare(squarePosition);
        SquareInfo previousHoveredSquare = boardManager.currentHoveredSquare;

        if (previousHoveredSquare == currentHoveredSquare)
        {
            return;
        }

        boardManager.previousHoveredSquare = boardManager.currentHoveredSquare;
        boardManager.currentHoveredSquare = currentHoveredSquare;
    }

    private void VisualizeHoveredSquare()
    {
        if (boardManager.previousHoveredSquare != null)
        {
            if (boardManager.previousHoveredSquare != boardManager.currentSelectedSquare)
            {
                Juice_Square.SquareUnHover(boardManager.previousHoveredSquare.gameObject);
            }

            if (boardManager.previousHoveredSquare.pieceGameObject != null)
            {
                Juice_Piece.PieceUnHover(ref boardManager.previousHoveredSquare.pieceGameObject);
            }
        }

        if (boardManager.currentHoveredSquare == null)
        {
            return;
        }

        if (boardManager.currentHoveredSquare.pieceGameObject != null)
        {
            InputManager inputManager = GameManager.instance.inputManager;

            Juice_Piece.PieceHover(ref boardManager.currentHoveredSquare.pieceGameObject,
                inputManager.selectSquares.SquareDistance(
                inputManager.trackMousePos.MousePosition(), boardManager.currentHoveredSquare.transform.position));
        }

        if (!boardManager.allValidSquares.Contains(boardManager.currentHoveredSquare))
        {
            return;
        }

        Color highlight = Color.black; //if it's black, something went wrong

        highlight =
            UniversalFunctions.CheckIfSquareValidCapture(ref boardManager.currentHoveredSquare)
            ? boardManager.boardData.validCaptureMove : boardManager.boardData.validEmptyMove;

        Juice_Square.SquareHover(boardManager.currentHoveredSquare.gameObject, highlight);
    }
}
