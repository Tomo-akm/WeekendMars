using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("ポーズ設定")]
    public Button pauseButton;
    public GameObject pauseMenu;
    
    private bool isPaused = false;
    
    void Start()
    {
        // ポーズボタンのクリックイベントを設定
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        // 開始時はポーズメニューを非表示
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }
    

    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    void PauseGame()
    {
        Time.timeScale = 0f; // ゲーム時間を停止
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        Debug.Log("ゲームをポーズしました");
    }
    
    void ResumeGame()
    {
        Time.timeScale = 1f; // ゲーム時間を再開
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        Debug.Log("ゲームを再開しました");
    }
    
    // 外部から呼び出し可能なメソッド
    public void Resume()
    {
        if (isPaused)
        {
            TogglePause();
        }
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}