using Chess;
using NUnit.Framework;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public RuntimeChessPieceData pieceData;
    public GameObject pieceGameObject;
    public Vector2Int tilePosition;

    [HideInInspector] public Color defaultColor;

    [HideInInspector] public bool selected;
    public bool iced;

    private void Start()
    {
        defaultColor = GetComponent<Renderer>().material.color;
    }

    public void HighlightAsValidMove(Color highlightColor)
    {
        GetComponent<Renderer>().material.color = highlightColor;
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

    public void Selected(Color selectedColor)
    {
        selected = true;
        GetComponent<Renderer>().material.color = selectedColor;
    }

    public void UnSelected()
    {
        selected = false;
        GetComponent<Renderer>().material.color = defaultColor;
    }
}
