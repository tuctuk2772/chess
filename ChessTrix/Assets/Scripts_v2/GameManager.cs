using UnityEngine;
using Chess;
using System.Runtime.CompilerServices;
using DG.Tweening;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(ChessPieceManager))]
public class GameManager : MonoBehaviour
{
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

    private void Start()
    {
        switch (gameState)
        {
            case GameStates.Board:
                float boardSpawnTime = boardManager.CreateBoard();

                DOVirtual.DelayedCall(boardSpawnTime, () =>
                {
                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                    UniversalFunctions.GetSquare(0, 4, boardManager.squares),
                    ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Pawn);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                    UniversalFunctions.GetSquare(6, 4, boardManager.squares),
                    ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Pawn);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(4, 5, boardManager.squares),
                        ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Knight);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(4, 7, boardManager.squares),
                        ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Queen);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(5, 7, boardManager.squares),
                        ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Bishop);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(3, 7, boardManager.squares),
                        ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Bishop);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(3, 3, boardManager.squares),
                        ChessPieceData.PieceColor.White, ChessPieceData.PieceType.Bishop);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(1, 3, boardManager.squares),
                        ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Bishop);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(5, 3, boardManager.squares),
                        ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Bishop);

                    chessPieceManager.spawnPiece.SpawnSpecificPiece(
                        UniversalFunctions.GetSquare(7, 3, boardManager.squares),
                        ChessPieceData.PieceColor.Black, ChessPieceData.PieceType.Bishop);
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
