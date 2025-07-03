using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("GenerateBoard")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform boardParent;
    [SerializeField] private Transform debugSquare;
    [SerializeField] private BoardData boardData;

    static readonly Vector2Int squareAmount = new Vector2Int(8, 8);
    static readonly Vector2Int squareSize = new Vector2Int(1, 1);

    [Space(10)]
    [Header("Variables")]
    public TileInfo selectedTile;
    [HideInInspector] public List<TileInfo> tiles;

    public void CreateBoard()
    {
        var config = new GenerateBoardConfig
        {
            squareAmount = squareAmount,
            tileSize = squareSize,
            boardData = boardData,
            tilePrefab = tilePrefab,
            debugSquare = debugSquare,
            boardParent = boardParent
        };

        GenerateBoard.GenereateBoardTiles(config, ref tiles);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (debugSquare == null || boardParent == null)
            return;

        Vector3 boardSize = new Vector3(
            squareAmount.x * squareSize.x,
            squareAmount.y * squareSize.y,
            1f
        );

        debugSquare.localScale = boardSize;
        debugSquare.localPosition = Vector3.zero;
    }

#endif
}
