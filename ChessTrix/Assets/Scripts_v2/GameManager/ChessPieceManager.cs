using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChessPieceManager : MonoBehaviour
{
    private ChessPieceData[] chessPieces; //grabs from folder

    public List<RuntimeChessPieceData> activeChessPieces = new List<RuntimeChessPieceData>();

    public SpawnPiece spawnPiece { get; private set; }
    public MoveAllPieces moveAllPieces { get; private set; }

    private void Awake()
    {
        chessPieces = Resources.LoadAll<ChessPieceData>("ChessPieces");

        spawnPiece = new SpawnPiece()
        {
            chessPieces = chessPieces,
            activeChessPieces = activeChessPieces
        };

        moveAllPieces = new MoveAllPieces() { };
    }

    private void Start()
    {
        spawnPiece.DebugTest();
    }
}
