using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Chess;

[System.Serializable]
public struct GenerateBoardConfig
{
    public Vector2Int squareAmount;
    public Vector2Int tileSize;
    public BoardData boardData;
    public GameObject tilePrefab;
    public Transform debugSquare;
    public Transform boardParent;
}

public class GenerateBoard
{
    public static void GenereateBoardTiles(GenerateBoardConfig config, ref List<TileInfo> tiles)
    {
        if (config.debugSquare != null)
            config.debugSquare.gameObject.SetActive(false);

        Vector2 boardWorldSize = config.debugSquare.localScale;
        Vector2 tileSize = new Vector2(
            boardWorldSize.x / config.squareAmount.x,
            boardWorldSize.y / config.squareAmount.y
        );

        if (InputManager.input != null && InputManager.input.trackMousePos != null)
        {
            InputManager.input.trackMousePos.tileSize = tileSize;
            InputManager.input.trackMousePos.boardSize = config.squareAmount;

            Vector2 centeredOffset = (Vector2)config.boardParent.position -
                                     new Vector2(boardWorldSize.x, boardWorldSize.y) / 2f;
            InputManager.input.trackMousePos.offset = centeredOffset;
        }

        Vector2 boardBottomLeft = (Vector2)config.boardParent.position -
                                   new Vector2(boardWorldSize.x, boardWorldSize.y) / 2f;

        for (int row = 0; row < config.squareAmount.y; row++)
        {
            for (int col = 0; col < config.squareAmount.x; col++)
            {
                Vector3 position = new Vector3(
                    boardBottomLeft.x + col * tileSize.x + tileSize.x / 2f,
                    boardBottomLeft.y + row * tileSize.y + tileSize.y / 2f,
                    0f
                );

                GameObject tileObject = UnityEngine.Object.Instantiate(
                    config.tilePrefab,
                    position,
                    Quaternion.identity,
                    config.boardParent
                );

                tileObject.transform.localScale = new Vector3(tileSize.x, tileSize.y, 1f);
                tileObject.name = $"Tile_{col}_{row}";

                SpriteRenderer renderer = tileObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = (col + row) % 2 == 0 ? config.boardData.black.normal : config.boardData.white.normal;
                }

                TileInfo tileInfo = tileObject.GetComponent<TileInfo>();
                if (tileInfo != null)
                {
                    tiles.Add(tileInfo);
                    tileInfo.tilePosition = new Vector2Int(col, row);
                    tileInfo.defaultColor = (col + row) % 2 == 0 ? config.boardData.black.normal : config.boardData.white.normal;
                }
            }
        }
    }
}
