using UnityEngine;
using UnityEngine.SceneManagement;
using Chess;
using TMPro;

public class RoundManager : MonoBehaviour
{
    public bool gameOver = false;
    private int round = 0; //starts at zero because it will automatically add one

    [SerializeField] private TextMeshProUGUI maxRoundText;
    [SerializeField] private TextMeshProUGUI currentRoundText;
    [SerializeField] private GameObject gameOverScreen;

    private void Start()
    {
        gameOver = false;
        UniversalFunctions.PutNumberOnScreen(ref StaticVariables.MAX_TURNS_AMOUNT, ref maxRoundText);
    }

    public void AddRound()
    {
        round++;

        UpdateRoundText();
        CheckRound();
    }

    public void ResetRounds()
    {
        round = 0;
    }

    public int GetRound()
    {
        return round;
    }

    private void CheckRound()
    {
        if (StaticVariables.MAX_TURNS_AMOUNT <= round)
        {
            GameOver();
        }
    }

    private void UpdateRoundText()
    {
        UniversalFunctions.PutNumberOnScreen(ref round, ref currentRoundText);
    }

    public void GameOver()
    {
        gameOver = true;
        gameOverScreen.SetActive(true);
    }

    public void QuitGame()
    {
        UniversalFunctions.QuitGame();
    }

    public void RestartGame()
    {
        UniversalFunctions.RestartGame();
    }
}
