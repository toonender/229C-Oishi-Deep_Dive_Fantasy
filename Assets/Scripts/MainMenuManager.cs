using UnityEngine;
using UnityEngine.SceneManagement; // <-- สำคัญมาก ต้องมีเพื่อใช้คำสั่งเปลี่ยน Scene

public class MainMenuManager : MonoBehaviour
{
    // ── ฟังก์ชันสำหรับปุ่ม Play ────────────────────────
    public void PlayGame()
    {
        // คืนค่าเวลาเป็น 1 เสมอ ป้องกันบั๊กเวลาแวะมาจากตอนกด Pause
        Time.timeScale = 1f;

        // ใส่ชื่อฉากเกมของคุณให้ตรงเป๊ะ (ตัวพิมพ์เล็ก-ใหญ่มีผล)
        SceneManager.LoadScene("MainGame");
    }

    // ── ฟังก์ชันสำหรับปุ่ม Credit ──────────────────────
    public void GoToCredit()
    {
        SceneManager.LoadScene("CreditScene");
    }

    // ── ฟังก์ชันสำหรับปุ่ม Quit ────────────────────────
    public void QuitGame()
    {
        // คำสั่งนี้จะปิดเกมเมื่อ Build เป็น .exe หรือ .apk แล้ว
        Application.Quit();

        // บรรทัดนี้มีไว้ให้เราเห็นใน Unity Editor ว่าปุ่มทำงานแล้ว
        Debug.Log("Quit Game!");
    }
}