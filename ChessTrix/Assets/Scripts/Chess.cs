using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using UnityEngine.InputSystem.Controls;

namespace Chess
{
    public enum GameStates
    {
        Upgrade, Title, Board, GameOver
    };

    public class UniversalFunctions
    {
        //grabs the reference
        public static void CheckComponent<T>(ref T component, GameObject gameObject) where T : Component
        {
            component = gameObject.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{typeof(T).Name} not found on {gameObject.name}!");
            }
        }

        public static Color ToRGB(float r, float g, float b)
        {
            return new Color(r / 255f, g / 255f, b / 255f);
        }

        public static void PutNumberOnScreen<T>(ref T value, ref TextMeshProUGUI text)
        {
            if (value.GetType() == typeof(double) || value.GetType() == typeof(float)
                || value.GetType() == typeof(int) || value.GetType() == typeof(ulong)
                || value.GetType() == typeof(decimal))
            {
                text.text = string.Format("{0:N0}", value);
            }
        }

        public static SquareInfo GetSquare(int x, int y, List<SquareInfo> squares)
        {
            SquareInfo searchSquare = squares.Find(t => t.squarePosition == new Vector2Int(x, y));

            if (searchSquare == null) return null;
            else return searchSquare;
        }

        public static bool CheckIfSquareValidCapture(ref SquareInfo square)
        {
            ref BoardManager boardManager = ref GameManager.instance.boardManager;

            if (boardManager.validCaptureSquares != null && boardManager.validCaptureSquares.Contains(square))
            {
                return true;
            }

            if (boardManager.validEmptySquares != null && boardManager.validEmptySquares.Contains(square))
            {
                return false;
            }

            Debug.Log("something went wrong with CheckIfSquareValidCapture!");
            return false;
        }
    }

    public class ButtonFunctions
    {
        public static void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public class StaticVariables
    {
        //default scale of piece
        public static readonly Vector3 defaultScale = new Vector3(0.05f, 0.05f, 1f);

        //variables for needed scoring
        public static readonly double DEFAULT_NEEDED_SCORE = 1500; //default = 1500 (in case changed)

        //variables for scoring
        public static readonly float TAKEN_MULTIPLIER = 25f;
        public static readonly float HORIZONTAL_CLEARING_MULTIPLIER = 50f;
        public static readonly float VERTICAL_CLEARING_MULTIPLIER = 100f;
        public static readonly float CHECKMATE_MULTIPLIER = 500f;

        //variables for visuals
        public static readonly float PIECE_ALPHA_VALUE = 0.25f;

        //variables for rounds
        public static readonly int MAX_TURNS_AMOUNT = 64;
    }

    public class StaticChessInformation
    {
        public static readonly Dictionary<ChessPieceData.PieceType, float> typeWeights = new()
        {
            { ChessPieceData.PieceType.Pawn, 0.5f }, //1/2
            { ChessPieceData.PieceType.Rook, 0.125f }, //1/8
            { ChessPieceData.PieceType.Knight, 0.125f }, //1/8
            { ChessPieceData.PieceType.Bishop, 0.125f }, //1/8
            { ChessPieceData.PieceType.Queen, 0.0625f }, //1/16
            { ChessPieceData.PieceType.King, 0.0625f }, //1/16
        };
    }
}

