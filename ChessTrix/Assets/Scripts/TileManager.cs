using Chess;
using System;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//todo - instead of despawning/respawning every piece, swap parents

public class TileManager : MonoBehaviour
{
    private ChessPieceData.PieceColor activeColor = ChessPieceData.PieceColor.White;

    [SerializeField] BoardData boardData;
    public List<TileInfo> tiles;

    TrackMousePos trackMousePos;
    PieceManager pieceManager;
    PauseManager pauseManager;
    ScoreManager scoreManager;
    GenerateBoard generateBoard;
    RoundManager roundManager;

    [HideInInspector] public Color selectedColor;
    private List<TileInfo> highlightedTiles = new List<TileInfo>();

    private bool tileSelected = false;
    private TileInfo previousTile = null;

    [SerializeField] private Canvas canvas;
    [SerializeField] private SpriteRenderer panel;

    [SerializeField] private TextMeshProUGUI clearText; //debug

    public Int32 dropOption;

    private void Awake()
    {
        UniversalFunctions.CheckComponent(ref trackMousePos, gameObject);
        UniversalFunctions.CheckComponent(ref pieceManager, gameObject);
        UniversalFunctions.CheckComponent(ref pauseManager, gameObject);
        UniversalFunctions.CheckComponent(ref scoreManager, gameObject);
        UniversalFunctions.CheckComponent(ref generateBoard, gameObject);
        UniversalFunctions.CheckComponent(ref roundManager, gameObject);
    }

    private void Start()
    {
        tileSelected = false;

        clearText.enabled = false;

        roundManager.ResetRounds();
        DropNewPiece();
        SwitchValidColor(activeColor);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !pauseManager.paused && !roundManager.gameOver)
        {
            SelectPiece();
        }

        //debug
        if (Input.GetKeyDown(KeyCode.A))
        {
            scoreManager.currentScore = scoreManager.neededScore;
        }
    }

    private void SwitchValidColor(ChessPieceData.PieceColor currentColor)
    {
        string[] names = Enum.GetNames(typeof(ChessPieceData.PieceColor));
        int colorCount = names.Length;
        int currentIndex = Array.IndexOf(names, currentColor.ToString());

        for (int i = 1; i <= colorCount; i++) //prevent infinite loops
        {
            int nextIndex = (currentIndex + i) % colorCount;
            ChessPieceData.PieceColor nextColor = (ChessPieceData.PieceColor)Enum.Parse(typeof(ChessPieceData.PieceColor), names[nextIndex]);

            //check to see if there are pieces of that color and that they have valid moves, if not skip
            if (pieceManager.HasActivePieceOfColor(nextColor))
            {
                activeColor = nextColor;
                pieceManager.UpdateActivePieceVisual(activeColor, tiles);
                ChangeUIColor();
                return;
            }
        }

        Debug.LogWarning("No active pieces of any color remain!");
    }

    private void ChangeUIColor()
    {
        Color panelColor = Color.gray;

        switch (activeColor)
        {
            case ChessPieceData.PieceColor.White:
                panelColor = generateBoard.boardData.white.normal;
                break;
            case ChessPieceData.PieceColor.Black:
                panelColor = UniversalFunctions.ToRGB(51, 51, 51);
                break;
            default: break;
        }

        panel.color = panelColor;
    }

    public void ClearRows()
    {
        bool atLeastOneRowCleared = false; //debug

        for (int y = 0; y < generateBoard.tileAmount.y; y++)
        {
            bool rowFull = true;
            ChessPieceData.PieceColor checkColor = ChessPieceData.PieceColor.White;

            for (int x = 0; x < generateBoard.tileAmount.x; x++)
            {
                TileInfo tile = GetTile(x + 1, y + 1);
                if (tile.pieceData == null)
                {
                    rowFull = false;
                    break;
                }

                if (x == 0) checkColor = tile.pieceData.pieceColor;

                if (tile.pieceData.pieceColor != checkColor)
                {
                    rowFull = false;
                    break;
                }
            }

            if (rowFull)
            {
                double addScore = 0;

                for (int x = 0; x < generateBoard.tileAmount.x; x++)
                {
                    TileInfo tile = GetTile(x + 1, y + 1);
                    float pieceValue = tile.pieceData.pieceValue;

                    addScore += pieceValue;

                    pieceManager.DeSpawnPiece(tile.transform);
                    tile.ClearPiece();
                }

                addScore = addScore * StaticVariables.HORIZONTAL_CLEARING_MULTIPLIER;
                scoreManager.SetScore(scoreManager.currentScore + addScore);
                clearText.enabled = true;
                MoveAllPiecesDown(null);

                atLeastOneRowCleared = true;
            }

            if (!atLeastOneRowCleared) clearText.enabled = false;
        }
    }

    public void ChangeDropOption(Int32 option)
    {
        dropOption = option;
    }

    private void DropNewPiece()
    {
        const int maxAttempts = 10;
        bool didSpawn = false;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            int randX = UnityEngine.Random.Range(1, 8);
            TileInfo tile = GetTile(randX, 8);

            if (tile.pieceData != null) continue;

            Debug.Log(roundManager.GetRound());
            switch (dropOption)
            {
                case 0:
                    switch (roundManager.GetRound())
                    {
                        case 0:
                            didSpawn = pieceManager.SpawnRandomPiece(tile.transform, new List<ChessPieceData.PieceColor> { ChessPieceData.PieceColor.White });
                            break;
                        case 1:
                            didSpawn = pieceManager.SpawnRandomPiece(tile.transform, new List<ChessPieceData.PieceColor> { ChessPieceData.PieceColor.Black });
                            break;
                        default:
                            didSpawn = pieceManager.SpawnRandomPiece(tile.transform);
                            break;
                    }
                    break;

                case 1:
                    ChessPieceData.PieceColor color = roundManager.GetRound() % 2 == 0
                        ? ChessPieceData.PieceColor.White
                        : ChessPieceData.PieceColor.Black;

                    didSpawn = pieceManager.SpawnRandomPiece(tile.transform, new List<ChessPieceData.PieceColor> { color });
                    break;
            }

            if (didSpawn)
            {
                roundManager.AddRound();
                return;
            }
        }

        Debug.LogWarning("Failed to spawn a piece after multiple attempts. Game may be stuck.");
    }

    public void MoveAllPiecesDown(TileInfo exceptionTile)
    {
        foreach (var tile in tiles)
        {
            if (tile.pieceData != null && tile != exceptionTile)
            {
                TileInfo belowTile = GetTile(tile.tilePosition.x + 1, tile.tilePosition.y);

                if (belowTile == null || belowTile.pieceData != null) continue;

                RuntimeChessPieceData movedPiece = pieceManager.SpawnPiece(belowTile.transform, tile.pieceData.pieceColor, tile.pieceData.pieceType, tile.pieceData.moveAmount);
                pieceManager.DeSpawnPiece(tile.transform);
                tile.ClearPiece();
                ResetAllSelectedTiles();
            }

            tile.UnSelected();
        }
    }

    void ResetAllSelectedTiles()
    {
        foreach (var tile in highlightedTiles)
        {
            tile.UnSelected();
        }

        highlightedTiles.Clear();
    }

    public void RemoveAllPieces()
    {
        foreach (var tile in tiles)
        {
            tile.UnSelected();
            pieceManager.DeSpawnPiece(tile.transform);
            tile.ClearPiece();
        }

        highlightedTiles.Clear();
    }

    private void SelectPiece()
    {
        TileInfo selectedTile = tiles.Find(tile => tile.tilePosition == trackMousePos.tilePosition);

        if (selectedTile == null) return;

        if (tileSelected && highlightedTiles.Contains(selectedTile))
        {
            MovePiece(selectedTile);
            return;
        }

        //makes it so you can only select active color and not blank spaces or any other color
        if (selectedTile.pieceData == null || selectedTile.pieceData.pieceColor != activeColor) return;

        ResetAllSelectedTiles();
        pieceManager.UpdateActivePieceVisual(activeColor, tiles);

        if (previousTile == selectedTile)
        {
            selectedTile.UnSelected();
            previousTile = null;
            tileSelected = false;
            return;
        }

        if (previousTile != null) previousTile.UnSelected();

        selectedTile.Selected(selectedColor);
        previousTile = selectedTile;
        tileSelected = true;

        if (selectedTile.pieceData == null) return;
        if (selectedTile.pieceData.pieceColor != activeColor) return;

        List<TileInfo> validMoves = pieceManager.GetValidMoves(selectedTile);
        foreach (var moveTile in validMoves)
        {
            moveTile.HighlightAsValidMove(boardData.validMove);
            highlightedTiles.Add(moveTile);
        }
    }

    private void MovePiece(TileInfo selectedTile)
    {
        if (selectedTile == null || previousTile.pieceData == null) return;

        RuntimeChessPieceData pieceData = previousTile.pieceData;

        int amountMoved = pieceData.moveAmount;

        pieceManager.DeSpawnPiece(previousTile.transform);
        previousTile.ClearPiece();

        if (selectedTile.pieceData != null)
        {
            float chessPieceDataScore = selectedTile.pieceData.pieceValue;
            pieceManager.DeSpawnPiece(selectedTile.transform);
            bool reachedThreshold = scoreManager.SetScore(scoreManager.currentScore + (chessPieceDataScore * StaticVariables.TAKEN_MULTIPLIER));
            selectedTile.ClearPiece();
            if (reachedThreshold)
            {
                DropNewPiece();
                SwitchValidColor(activeColor);
                return;
            }
        }

        // recreates piece
        RuntimeChessPieceData newPiece = pieceManager.SpawnPiece(selectedTile.transform, pieceData.pieceColor, pieceData.pieceType, amountMoved + 1);

        previousTile.UnSelected();
        ResetAllSelectedTiles();
        MoveAllPiecesDown(selectedTile);
        ClearRows();

        bool scoreThresholdReached = scoreManager.SetScore(scoreManager.currentScore);

        if (scoreThresholdReached)
        {
            pieceManager.SpawnPiece(selectedTile.transform, pieceData.pieceColor, pieceData.pieceType);
        }

        DropNewPiece();
        SwitchValidColor(activeColor);
    }

    public TileInfo GetTile(int x, int y)
    {
        TileInfo searchTile = tiles.Find(t => t.tilePosition == new Vector2Int(x - 1, y - 1));

        if (searchTile == null) return null;
        else return searchTile;
    }
};
