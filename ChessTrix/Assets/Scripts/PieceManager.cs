using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Chess;

public class PieceManager : MonoBehaviour
{
    private Queue<ChessPieceData.PieceColor> colorHistory = new Queue<ChessPieceData.PieceColor>(2);

    [SerializeField] List<ChessPieceData> chessPieces = new List<ChessPieceData>();
    public List<GameObject> activeChessPieces = new List<GameObject>();
    private TileManager tileManager;
    private GenerateBoard generateBoard;

    private void Awake()
    {
        tileManager = GetComponent<TileManager>();
        if (tileManager == null) Debug.LogError("Tile Manager not found!");

        generateBoard = GetComponent<GenerateBoard>();
        if (generateBoard == null) Debug.LogError("Generate Board not found!");
    }

    public bool SpawnRandomPiece(Transform tile,
        List<ChessPieceData.PieceColor> allowedColors = null,
        List<ChessPieceData.PieceType> allowedTypes = null)
    {
        if (chessPieces == null || chessPieces.Count == 0 || tile == null) return false;

        List<ChessPieceData> filteredPieces = chessPieces.FindAll(piece =>
            (allowedColors == null || allowedColors.Count == 0 || allowedColors.Contains(piece.pieceColor)) &&
            (allowedTypes == null || allowedTypes.Count == 0 || allowedTypes.Contains(piece.pieceType))
        );

        if (filteredPieces.Count == 0) return false;

        //not completely random - check if there are too many of one color spawning
        if (colorHistory.Count == 3 && colorHistory.All(c => c == colorHistory.Peek()))
        {
            var enforcedColor = colorHistory.Peek() == ChessPieceData.PieceColor.White
                ? ChessPieceData.PieceColor.Black : ChessPieceData.PieceColor.White;

            filteredPieces = filteredPieces
                .Where(p => p.pieceColor == enforcedColor)
                .ToList();

            if (filteredPieces.Count == 0) return false;
        }

        //weighted towards certain pieces
        List<(ChessPieceData piece, float weight)> weighted = new();
        float totalWeight = 0f;

        //weird rounding error can mess up randomness
        filteredPieces = filteredPieces.OrderBy(_ => Random.value).ToList();

        foreach (var piece in filteredPieces)
        {
            float weight = StaticChessInformation.typeWeights.ContainsKey(piece.pieceType) ? StaticChessInformation.typeWeights[piece.pieceType] : 1f;
            weighted.Add((piece, weight));
            totalWeight += weight;
        }

        float rand = Random.Range(0f, totalWeight);
        float current = 0f;

        foreach (var (piece, weight) in weighted)
        {
            current += weight;
            if (rand <= current)
            {
                SpawnPiece(tile, piece.pieceColor, piece.pieceType);
                TrackRecentColor(piece.pieceColor);
                return true;
            }
        }

        return false;
    }

    private void TrackRecentColor(ChessPieceData.PieceColor color)
    {
        if (colorHistory.Count == 3)
            colorHistory.Dequeue();

        colorHistory.Enqueue(color);
    }

    public bool HasActivePieceOfColor(ChessPieceData.PieceColor color)
    {
        foreach (var pieceObj in activeChessPieces)
        {
            TileInfo tile = pieceObj.transform.parent?.GetComponent<TileInfo>();
            if (tile?.pieceData != null && tile.pieceData.pieceColor == color &&
                GetValidMoves(tile)?.Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateActivePieceVisual(ChessPieceData.PieceColor activeColor, List<TileInfo> tiles)
    {
        if (tiles.Count == 0 || tiles == null) return;

        foreach (var tile in tiles)
        {
            if (tile.pieceData == null) continue;

            ChessPieceData.PieceColor pieceColor = tile.pieceData.pieceColor;
            SetPieceVisibility(tile, pieceColor == activeColor);
        }
    }

    public void SetPieceVisibility(TileInfo tile, bool condition)
    {
        if (tile == null || tile.pieceData == null || tile.pieceGameObject == null) return;

        SpriteRenderer pieceRenderer = tile.pieceGameObject.GetComponent<SpriteRenderer>();
        if (pieceRenderer == null) return;
        Color color = pieceRenderer.color;

        ChessPieceData.PieceColor pieceColor = tile.pieceData.pieceColor;
        if (condition) color.a = 1;
        else color.a = StaticVariables.PIECE_ALPHA_VALUE;

        pieceRenderer.color = color;
    }

    public RuntimeChessPieceData SpawnPiece(
Transform tile,
ChessPieceData.PieceColor pieceColor,
ChessPieceData.PieceType pieceType,
int moveAmount = 0)
    {
        if (chessPieces == null || chessPieces.Count == 0 || tile == null)
        {
            Debug.LogWarning("SpawnPiece failed: invalid inputs.");
            return null;
        }

        ChessPieceData baseTemplate = chessPieces.Find(p =>
            p.pieceColor == pieceColor && p.pieceType == pieceType);

        if (baseTemplate == null)
        {
            Debug.LogWarning($"No template found for {pieceColor} {pieceType}.");
            return null;
        }

        TileInfo tileInfo = tile.GetComponent<TileInfo>();
        if (tileInfo == null)
        {
            Debug.LogError($"Tile {tile.name} missing TileInfo component!");
            return null;
        }

        // Guard against double-spawning on this tile
        if (tileInfo.pieceGameObject != null)
        {
            Debug.LogWarning($"SpawnPiece aborted: Tile {tileInfo.tilePosition} already has a piece.");
            return null;
        }

        // Defensive: destroy anything already lingering on the tile
        if (tile.childCount > 0)
        {
            foreach (Transform child in tile)
            {
                if (activeChessPieces.Contains(child.gameObject))
                {
                    activeChessPieces.Remove(child.gameObject);
                    Destroy(child.gameObject);
                    Debug.LogWarning($"Removed ghost piece from {tileInfo.tilePosition} before spawning new one.");
                }
            }
        }

        // Build runtime piece data
        var piece = new RuntimeChessPieceData
        {
            pieceColor = baseTemplate.pieceColor,
            pieceType = baseTemplate.pieceType,
            pieceSprite = baseTemplate.pieceSprite,
            movements = new List<ChessPieceData.MovementType>(baseTemplate.movements),
            pieceValue = baseTemplate.pieceValue,
            moveAmount = moveAmount,
        };

        // Create piece GameObject
        GameObject pieceObject = new GameObject($"{piece.pieceColor} {piece.pieceType}");
        pieceObject.transform.position = new Vector3(tile.position.x, tile.position.y, -1f);
        pieceObject.transform.localScale = new Vector3(
            generateBoard.tileSize.x * 0.05f,
            generateBoard.tileSize.y * 0.05f,
            1f);
        pieceObject.transform.SetParent(tile);

        // Add sprite
        SpriteRenderer sr = pieceObject.AddComponent<SpriteRenderer>();
        sr.sprite = piece.pieceSprite;

        // Link to TileInfo
        tileInfo.SetPiece(piece, pieceObject);
        activeChessPieces.Add(pieceObject);

        return piece;
    }

    public void DeSpawnPiece(Transform tile)
    {
        if (tile == null)
        {
            //Debug.LogWarning("Tried to despawn a piece from a null tile.");
            return;
        }

        TileInfo tileInfo = tile.GetComponent<TileInfo>();
        if (tileInfo == null)
        {
            //Debug.LogWarning($"Tile {tile.name} has no TileInfo component.");
            return;
        }

        if (tileInfo.pieceGameObject == null)
        {
            //Debug.LogWarning($"Tile {tile.name} has no pieceGameObject to despawn.");
            return;
        }

        GameObject pieceToDestroy = tileInfo.pieceGameObject;

        if (activeChessPieces.Contains(pieceToDestroy))
        {
            activeChessPieces.Remove(pieceToDestroy);
        }
        else
        {
            //Debug.LogWarning($"Piece {pieceToDestroy.name} was not in activeChessPieces list.");
        }

        Destroy(pieceToDestroy);

        // Null out the tile references
        tileInfo.ClearPiece();
    }

    public List<TileInfo> GetValidMoves(TileInfo originTile)
    {
        var validTiles = new List<TileInfo>();
        var piece = originTile.pieceData;

        foreach (var moveType in piece.movements)
        {
            if (moveType == ChessPieceData.MovementType.PawnSpecific)
            {
                // Forward one tile
                Vector2Int forward = originTile.tilePosition + new Vector2Int(0, -1);
                TileInfo forwardTile = tileManager.GetTile(forward.x + 1, forward.y + 1);
                if (forwardTile != null && forwardTile.pieceGameObject == null)
                {
                    validTiles.Add(forwardTile);
                }

                // Forward two tiles
                if (piece.moveAmount == 0)
                {
                    Vector2Int firstStep = originTile.tilePosition + new Vector2Int(0, -1);
                    Vector2Int doubleForward = originTile.tilePosition + new Vector2Int(0, -2);

                    TileInfo firstTile = tileManager.GetTile(firstStep.x + 1, firstStep.y + 1);
                    TileInfo doubleForwardTile = tileManager.GetTile(doubleForward.x + 1, doubleForward.y + 1);

                    if (firstTile != null && firstTile.pieceGameObject == null &&
                        doubleForwardTile != null && doubleForwardTile.pieceGameObject == null)
                    {
                        validTiles.Add(doubleForwardTile);
                    }
                }

                // Diagonal captures
                Vector2Int[] diagonals = {
                        originTile.tilePosition + new Vector2Int(-1, -1),
                        originTile.tilePosition + new Vector2Int(1, -1)
                    };

                foreach (var diag in diagonals)
                {
                    TileInfo diagTile = tileManager.GetTile(diag.x + 1, diag.y + 1);
                    if (diagTile != null && diagTile.pieceGameObject != null &&
                        diagTile.pieceData.pieceColor != piece.pieceColor)
                    {
                        SetPieceVisibility(diagTile, true);
                        validTiles.Add(diagTile);
                    }
                }

                break;
            }


            if (moveType == ChessPieceData.MovementType.LShape)
            {
                Vector2Int[] LShapes =
                {
                        originTile.tilePosition + new Vector2Int(1, 2),
                        originTile.tilePosition + new Vector2Int(1, -2),
                        originTile.tilePosition + new Vector2Int(-1, 2),
                        originTile.tilePosition + new Vector2Int(-1, -2),

                        originTile.tilePosition + new Vector2Int(2, 1),
                        originTile.tilePosition + new Vector2Int(2, -1),
                        originTile.tilePosition + new Vector2Int(-2, 1),
                        originTile.tilePosition + new Vector2Int(-2, -1),
                    };

                foreach (var LShape in LShapes)
                {
                    TileInfo LTile = tileManager.GetTile(LShape.x + 1, LShape.y + 1);
                    if (LTile == null) continue;

                    if (LTile.pieceGameObject == null || LTile.pieceData.pieceColor != piece.pieceColor)
                    {
                        SetPieceVisibility(LTile, true);
                        validTiles.Add(LTile);
                    }
                }
                break;
            }

            var directions = MovementDictionary.GetOffsets(moveType);
            foreach (var dir in directions)
            {
                Vector2Int current = originTile.tilePosition + dir;
                for (var i = 0; i < 8; i++)
                {
                    TileInfo target = tileManager.GetTile(current.x + 1, current.y + 1);
                    if (target == null) break;

                    if (target.pieceGameObject == null)
                    {
                        validTiles.Add(target);
                    }
                    else
                    {
                        if (target.pieceData.pieceColor != piece.pieceColor)
                        {
                            SetPieceVisibility(target, true);
                            validTiles.Add(target);
                        }
                        break;
                    }

                    if (moveType == ChessPieceData.MovementType.SingleSpace)
                    {
                        break;
                    }

                    current += dir;
                }
            }
        }

        return validTiles;
    }
}
