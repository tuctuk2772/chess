using System.Collections.Generic;
using UnityEngine;
using Chess;

[CreateAssetMenu(fileName = "NewChessPiece", menuName = "Chess/Chess Piece")]
public class ChessPieceData : ScriptableObject
{
    public enum PieceColor { White, Black }
    public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
    public enum MovementType { Horizontal, Vertical, Diagonal, LShape, SingleSpace, PawnSpecific }

    //actual variables
    public Sprite pieceSprite;
    public PieceColor pieceColor;
    public PieceType pieceType;
    public float pieceValue; //for scoring, based on Turing values:
    /*
    - Pawn = 1
    - Knight = 3
    - Bishop = 3.5
    - Rook = 5
    - Queen = 10
    */
    public List<MovementType> movements = new List<MovementType>();
    public PieceModifiers modifier;
    public string PieceName => $"{pieceColor} {pieceType}";
}

public class RuntimeChessPieceData
{
    public ChessPieceData.PieceColor pieceColor;
    public ChessPieceData.PieceType pieceType;
    public Sprite pieceSprite;
    public List<ChessPieceData.MovementType> movements;
    public float pieceValue;

    public int moveAmount; //mainly for pawn, but might need for castling
}