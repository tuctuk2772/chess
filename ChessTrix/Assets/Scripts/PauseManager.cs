/*using UnityEngine;
using UnityEngine.UI;
using Chess;

public class PauseManager : MonoBehaviour
{
    [HideInInspector] public bool paused = false;
    [SerializeField] GameObject pauseScreen;

    //private PasswordManager passwordManager;
    private RoundManager roundManager;

    private void Awake()
    {
        //UniversalFunctions.CheckComponent(ref passwordManager, gameObject);
        UniversalFunctions.CheckComponent(ref roundManager, gameObject);
    }

    private void Start()
    {
        Paused();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !roundManager.gameOver)
        {
            if (!paused) Paused();
            else Resume();
        }
    }

    public void Paused()
    {
        paused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Resume()
    {
        paused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void QuitGame()
    {
        ButtonFunctions.QuitGame();
    }
}
*/