using UnityEngine;
using Chess;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(BoardManager))]
[RequireComponent(typeof(InputManager))]
public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public GameStates gameState = GameStates.Board;

    [HideInInspector] public BoardManager boardManager;
    [HideInInspector] public InputManager inputManager;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        boardManager = GetComponent<BoardManager>();
        inputManager = GetComponent<InputManager>();
    }

    private void Start()
    {
        switch (gameState)
        {
            case GameStates.Board:
                boardManager.CreateBoard();
                break;
            default:
                Debug.LogError("No State!");
                break;
        }
    }

}
