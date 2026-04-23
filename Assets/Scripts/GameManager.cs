using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public Text timerText;

    private float startTime;
    private bool gameEnded;

    void Start()
    {
        startTime = Time.time;
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
    }

    void Update()
    {
        if (gameEnded) return;
        float elapsed = Time.time - startTime;
        if (timerText) timerText.text = "Time: " + elapsed.ToString("F1") + " s";
    }

    public void TriggerWin()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f; // pause
        if (winPanel) winPanel.SetActive(true);
    }

    public void TriggerGameOver()
    {
        if (gameEnded) return;
        gameEnded = true;
        Time.timeScale = 0f;
        if (losePanel) losePanel.SetActive(true);
    }

    // ── ปุ่ม UI ────────────────────────────────────────────────────
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }

    public void GoToCredit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CreditScene");
    }
}