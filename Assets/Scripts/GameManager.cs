using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel; // <-- เพิ่มช่องใส่ UI หน้า Pause
    public Text timerText;

    private float startTime;
    private bool gameEnded;
    private bool isPaused; // <-- ตัวแปรเช็คว่าเกมหยุดอยู่หรือไม่

    void Start()
    {
        startTime = Time.time;
        Time.timeScale = 1f; // เมคชัวร์ว่าเริ่มเกมมาเวลาเดินปกติ

        // ปิด UI ทั้งหมดตอนเริ่มเกม
        if (winPanel) winPanel.SetActive(false);
        if (losePanel) losePanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
    }

    void Update()
    {
        // ถ้าเกมจบแล้ว (ชนะ/แพ้) ไม่ต้องรับค่าปุ่ม ESC
        if (gameEnded) return;

        // ── เช็คการกดปุ่ม ESC ────────────────────────────
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame(); // ถ้าหยุดอยู่ ให้เล่นต่อ
            }
            else
            {
                PauseGame();  // ถ้าเล่นอยู่ ให้หยุด
            }
        }

        // ถ้าเกมหยุดอยู่ ไม่ต้องอัปเดตเวลาบนหน้าจอ
        if (isPaused) return;

        float elapsed = Time.time - startTime;
        if (timerText) timerText.text = "Time: " + elapsed.ToString("F1") + " s";
    }

    // ── ระบบ Pause / Resume ─────────────────────────────────────────
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // หยุดเวลาในเกม (ฟิสิกส์ทุกอย่างจะหยุด)
        if (pausePanel) pausePanel.SetActive(true); // เปิดหน้า UI Pause
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // ให้เวลาเดินต่อ
        if (pausePanel) pausePanel.SetActive(false); // ปิดหน้า UI Pause
    }

    // ── ระบบจบเกม ───────────────────────────────────────────────────
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

    // ── ปุ่ม UI สำหรับหน้าต่างต่างๆ ─────────────────────────────────────
    public void RestartGame()
    {
        Time.timeScale = 1f; // ต้องคืนค่าเวลาเป็น 1 เสมอก่อนโหลดฉากใหม่
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // โหลดฉากปัจจุบันใหม่ (ไม่ต้องกลัวพิมพ์ชื่อผิด)
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