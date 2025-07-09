using Chess;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("GenerateBoard")]
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform boardParent;
    [SerializeField] private Transform debugSquare;
    [HideInInspector] public BoardData boardData;

    static readonly Vector2Int squareAmount = new Vector2Int(8, 8);
    static readonly Vector2Int squareSize = new Vector2Int(1, 1);

    [Header("Helper Scripts")]
    public GetValidSquares getValidSquares { get; private set; }

    [Space(10)]
    [Header("Variables")]
    public SquareInfo selectedSquare;
    public SquareInfo hoveredSquare;
    [HideInInspector] public List<SquareInfo> squares;
    public List<SquareInfo> validSquares;

    private void Awake()
    {
        getValidSquares = new GetValidSquares();
    }

    public float CreateBoard()
    {
        GenerateBoardConfig config = new GenerateBoardConfig
        {
            squareAmount = squareAmount,
            tileSize = squareSize,
            boardData = boardData,
            tilePrefab = tilePrefab,
            debugSquare = debugSquare,
            boardParent = boardParent
        };

        return GenerateBoard.GenerateBoardSquares(config, ref squares);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (debugSquare == null || boardParent == null)
            return;

        Renderer debugRenderer = null;
        UniversalFunctions.CheckComponent(ref debugRenderer, debugSquare.gameObject);

        Vector3 boardSize = new Vector3(
            squareAmount.x * squareSize.x,
            squareAmount.y * squareSize.y,
            1f
        );

        debugSquare.localScale = boardSize;
        debugSquare.localPosition = Vector3.zero;

        debugRenderer.sharedMaterial.color = boardData.black.normal;
    }

#endif
}
