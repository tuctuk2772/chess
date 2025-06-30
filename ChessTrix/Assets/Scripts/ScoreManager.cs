using UnityEngine;
using System.Runtime.CompilerServices;
using TMPro;
using System;
using Chess;

public class ScoreManager : MonoBehaviour
{
    public double neededScore;
    public double currentScore;

    private RoundManager roundManager;
    private TileManager tileManager;
    private PieceManager pieceManager;

    [Header("References")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI neededScoreText;

    private void Awake()
    {
        UniversalFunctions.CheckComponent(ref roundManager, gameObject);
        UniversalFunctions.CheckComponent(ref tileManager, gameObject);
        UniversalFunctions.CheckComponent(ref pieceManager, gameObject);
    }

    private void Start()
    {
        SetScore(0f);
        neededScore = StaticVariables.DEFAULT_NEEDED_SCORE;
        SetNeededScore(neededScore);
    }

    private double lastDisplayedScore = -1;
    private double lastDisplayedNeededScore = -1;

    public bool SetScore(double score)
    {
        currentScore = score;

        if (Math.Abs(score - lastDisplayedScore) > 0.001)
        {
            UniversalFunctions.PutNumberOnScreen(ref score, ref scoreText);
            lastDisplayedScore = score;
        }

        return CheckScore();
    }

    private void SetNeededScore(double score)
    {
        score = Math.Round(score / 100) * 100;
        neededScore = score;

        if (Math.Abs(score - lastDisplayedNeededScore) > 0.001)
        {
            UniversalFunctions.PutNumberOnScreen(ref score, ref neededScoreText);
            lastDisplayedNeededScore = score;
        }
    }

    private bool CheckScore()
    {
        if (double.IsNaN(currentScore) || double.IsNaN(neededScore)) return false;

        if (currentScore >= neededScore && currentScore != 0)
        {
            tileManager.RemoveAllPieces();
            roundManager.ResetRounds();
            SetNeededScore(neededScore * 1.66);

            currentScore = 0;
            UniversalFunctions.PutNumberOnScreen(ref currentScore, ref scoreText);
            return true;
        }

        else return false;
    }
}
