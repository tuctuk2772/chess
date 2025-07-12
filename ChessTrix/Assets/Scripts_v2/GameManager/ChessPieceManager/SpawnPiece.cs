using Chess;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPiece
{
    public ChessPieceData[] chessPieces;
    public List<RuntimeChessPieceData> activeChessPieces;

    public RuntimeChessPieceData SpawnSpecificPiece(SquareInfo square, ChessPieceData.PieceColor color,
        ChessPieceData.PieceType type)
    {
        if (square == null)
        {
            Debug.LogError("Piece location not found!");
            return null;
        }

        if (activeChessPieces == null)
        {
            Debug.LogError("Active piece list not created for spawning!");
            return null;
        }

        ChessPieceData baseTemplate = Array.Find(chessPieces, p =>
            p.pieceColor == color && p.pieceType == type);

        if (baseTemplate == null)
        {
            Debug.LogError("Piece Type not Found!");
            return null;
        }

        if (square.pieceGameObject != null)
        {
            Debug.Log($"Piece already on tile, not spawning " +
                $"a {baseTemplate.pieceColor} {baseTemplate.pieceType} at {square.squarePosition}");
            return null;
        }

        var piece = new RuntimeChessPieceData
        {
            pieceColor = baseTemplate.pieceColor,
            pieceType = baseTemplate.pieceType,
            pieceSprite = baseTemplate.pieceSprite,
            movements = new List<ChessPieceData.MovementType>(baseTemplate.movements),
            pieceValue = baseTemplate.pieceValue,

            currentSquare = square,
            moveAmount = 0 //just in case
        };

        //create piece GameObject
        GameObject pieceObject = new GameObject($"{piece.pieceColor} {piece.pieceType}");
        pieceObject.transform.localScale = Vector3.zero;
        pieceObject.transform.SetParent(square.gameObject.transform);
        pieceObject.transform.localPosition = new Vector3(0, 0, -1f);
        piece.gameObject = pieceObject;

        //Add sprite
        SpriteRenderer pieceSprite = pieceObject.AddComponent<SpriteRenderer>();
        pieceSprite.sprite = piece.pieceSprite;
        pieceSprite.sortingOrder = 2;

        //Link to SquareInfo
        square.SetPiece(piece, pieceObject);

        //add to active chess pieces
        activeChessPieces.Add(piece);

        Juice_ObjectSpawning.SpawnPiece(ref pieceObject, StaticVariables.defaultScale);

        //Debug.Log($"{baseTemplate.pieceColor} {baseTemplate.pieceType} spawned at {square.squarePosition}");
        return piece;
    }

    public void DebugTest()
    {
        /*foreach (var piece in chessPieces)
        {
            Debug.Log(piece);
        }*/
    }
}
