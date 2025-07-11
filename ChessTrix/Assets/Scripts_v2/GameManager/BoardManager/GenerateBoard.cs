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
    public static float GenerateBoardSquares(GenerateBoardConfig config, ref List<SquareInfo> tiles)
    {
        if (config.debugSquare != null)
        {
            config.debugSquare.gameObject.SetActive(false);
        }

        Vector2 boardWorldSize = config.debugSquare.localScale;
        Vector2 tileSize = new Vector2(
            boardWorldSize.x / config.squareAmount.x,
            boardWorldSize.y / config.squareAmount.y
        );

        if (InputManager.input == null || InputManager.input.trackMousePos == null)
        {
            Debug.LogError("No input found!");
            return 0f;
        }

        InputManager.input.trackMousePos.tileSize = tileSize;
        InputManager.input.trackMousePos.boardSize = config.squareAmount;

        Vector2 centeredOffset = (Vector2)config.boardParent.position -
                                 new Vector2(boardWorldSize.x, boardWorldSize.y) / 2f;
        InputManager.input.trackMousePos.offset = centeredOffset;

        Vector2 boardBottomLeft = (Vector2)config.boardParent.position -
                                   new Vector2(boardWorldSize.x, boardWorldSize.y) / 2f;

        float delayStep = 0.01f;
        float animPostDelay = 0.15f;
        int index = 0;

        for (int row = 0; row < config.squareAmount.y; row++)
        {
            for (int col = 0; col < config.squareAmount.x; col++)
            {
                Vector3 position = new Vector3(
                    boardBottomLeft.x + col * tileSize.x + tileSize.x / 2f,
                    boardBottomLeft.y + row * tileSize.y + tileSize.y / 2f,
                    0f
                );

                GameObject squareObject = UnityEngine.Object.Instantiate(
                    config.tilePrefab,
                    position,
                    Quaternion.identity,
                    config.boardParent
                );

                squareObject.transform.localScale = Vector3.zero;
                squareObject.name = $"Tile_{col}_{row}";
                squareObject.isStatic = true;

                SpriteRenderer renderer = squareObject.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    Material material = renderer.material;
                    material.SetColor("_DefaultColor", (col + row) % 2 == 0 ? config.boardData.black.normal : config.boardData.white.normal);
                    material.SetColor("_NewColor", (col + row) % 2 == 0 ? config.boardData.white.normal : config.boardData.black.normal);
                }

                float delay = index * delayStep;
                Juice_ObjectSpawning.SpawnSquare(squareObject, new Vector3(tileSize.x, tileSize.y, 1f), delay);
                index++;

                SquareInfo squareInfo = squareObject.GetComponent<SquareInfo>();
                if (squareInfo != null)
                {
                    tiles.Add(squareInfo);
                    squareInfo.squarePosition = new Vector2Int(col, row);
                }
            }
        }

        return (index - 1) * delayStep + animPostDelay;
    }
}
