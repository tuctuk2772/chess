using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Chess;

public class GenerateBoard : MonoBehaviour
{
    public Vector2Int tileAmount = (new Vector2Int(8, 8));
    public Vector2 tileSize = new Vector2(1f, 1f);
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Transform boardParent;
    private Transform debugSquare;

    [Space(10)]
    PieceManager pieceManager;
    TileManager tileManager;
    TrackMousePos trackMousePos;
    public BoardData boardData;

    private void Awake()
    {
        pieceManager = GetComponent<PieceManager>();
        if (pieceManager == null) Debug.LogError("Piece Manager not found!");

        tileManager = GetComponent<TileManager>();
        if (tileManager == null) Debug.LogError("Tile Manager not found!");

        trackMousePos = GetComponent<TrackMousePos>();
        if (trackMousePos == null) Debug.LogError("Track Mouse Pos not found!");

        debugSquare = transform.Find("DebugSize");
        if (debugSquare == null) Debug.LogError("DebugSquare not found!");
        debugSquare.gameObject.SetActive(false);

        GenerateBoardTiles();
    }

    void GenerateBoardTiles()
    {
        Vector2 offset = (Vector2)transform.localPosition;

        trackMousePos.offset = offset;
        trackMousePos.tileSize = tileSize;

        for (int row = 0; row < tileAmount.y; row++)
        {
            for (int col = 0; col < tileAmount.x; col++)
            {
                Vector3 position = new Vector3(offset.x + col * tileSize.x, offset.y + row * tileSize.y, 0);
                GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.identity, boardParent);
                tileObject.transform.localScale = new Vector3(tileSize.x, tileSize.y, 1f);
                tileObject.name = $"Tile_{col}_{row}";

                SpriteRenderer renderer = tileObject.GetComponent<SpriteRenderer>();
                renderer.color = (col + row) % 2 == 0 ? boardData.black.normal : boardData.white.normal;

                TileInfo tileInfo = tileObject.GetComponent<TileInfo>();
                tileManager.tiles.Add(tileInfo);

                tileInfo.tilePosition = new Vector2Int(col, row);
                tileInfo.defaultColor = (col + row) % 2 == 0 ? boardData.black.normal : boardData.white.normal;
            }
        }

        tileManager.selectedColor = boardData.selected;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            if (debugSquare == null)
            {
                Transform found = transform.Find("DebugSize");
                if (found != null) debugSquare = found;
            }
        }

        if (debugSquare != null)
        {
            debugSquare.transform.localScale = new Vector3(tileAmount.x * tileSize.x, tileAmount.y * tileSize.y, 1f);
            debugSquare.transform.localPosition = new Vector3((tileAmount.x * tileSize.x) / 2f - 0.5f, (tileAmount.y * tileSize.y) / 2f - 0.5f, 0f);
        }
    }
#endif
}
