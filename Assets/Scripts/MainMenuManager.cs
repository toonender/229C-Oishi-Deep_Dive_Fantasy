using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // ปุ่ม Play — ใส่ชื่อ Scene จริงของคุณตรง "GameScene"
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // ปุ่ม Credit
    public void OpenCredit()
    {
        SceneManager.LoadScene("end credit");
    }

    // ปุ่ม Quit
    public void QuitGame()
    {
        Application.Quit();

        // บรรทัดนี้ใช้ตอน test ใน Editor เท่านั้น
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}