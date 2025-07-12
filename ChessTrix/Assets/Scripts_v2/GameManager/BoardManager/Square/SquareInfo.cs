using Chess;
using NUnit.Framework;
using UnityEngine;

public class SquareInfo : MonoBehaviour
{
    public RuntimeChessPieceData pieceData = null;
    public GameObject pieceGameObject = null;
    public Vector2Int squarePosition;

    private SpriteRenderer squareRenderer;

    [HideInInspector] public SpriteRenderer dot;
    [HideInInspector] public SpriteRenderer circle;

    private void Awake()
    {
        UniversalFunctions.CheckComponent(ref squareRenderer, gameObject);
        dot = transform.GetChild(0).GetComponent<SpriteRenderer>();
        circle = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    public void SetPiece(RuntimeChessPieceData piece, GameObject pieceObject)
    {
        pieceData = piece;
        pieceGameObject = pieceObject;
    }

    public void ClearPiece()
    {
        pieceData = null;
        pieceGameObject = null;
    }
}
