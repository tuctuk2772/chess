using UnityEngine;
using Chess;
using System.Runtime.CompilerServices;
using DG.Tweening;
using System.Transactions;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(ChessPieceManager))]
public class GameManager : MonoBehaviour
{
    [Tooltip("Only input variables where x == y, or x/y is zero")] public Vector2Int debug;
    public bool clicked;
    public GameObject piece;

    public static GameManager instance { get; private set; }

    public GameStates gameState = GameStates.Board;

    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] public ChessPieceManager chessPieceManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        UniversalFunctions.CheckComponent(ref boardManager, gameObject);
        UniversalFunctions.CheckComponent(ref inputManager, gameObject);
        UniversalFunctions.CheckComponent(ref chessPieceManager, gameObject);

        DOTween.ClearCachedTweens();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            clicked = true;
        }

        bool spawnDone = DOTween.TotalTweensById("spawn") == 0;

        //this is dogshit code, but DOTween makes no sense and it's temporary
        if (spawnDone && clicked && DOTween.TotalTweensById("moveSquares") == 0)
        {
            chessPieceManager.moveAllPieces.MoveAllPiecesDirection(piece, debug,
                () =>
                {
                    clicked = false;
                });
        }
    }

    private void Start()
    {
        switch (gameState)
        {
            case GameStates.Board:
                float boardSpawnTime = boardManager.CreateBoard();

                DOVirtual.DelayedCall(boardSpawnTime, () =>
                {
                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                    UniversalFunctions.GetSquare(7, 7),
                    ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Pawn);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                    UniversalFunctions.GetSquare(6, 7),
                    ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Rook);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                    UniversalFunctions.GetSquare(6, 2),
                    ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Queen);
                });
                break;

            case GameStates.Title:
                Debug.Log("Title");
                break;

            case GameStates.Upgrade:
                Debug.Log("Upgrade");
                break;

            case GameStates.GameOver:
                Debug.Log("Game Over");
                break;

            default:
                Debug.LogError("No State!");
                break;
        }
    }
}
