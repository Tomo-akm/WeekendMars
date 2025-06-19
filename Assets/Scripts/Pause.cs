using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [Header("ポーズ設定")]
    public Button pauseButton;
    public GameObject pauseMenu;
    public Image grayOverlay;
    public Button resumeButton;
    public Button titleButton;

    [Header("Scene Names")]
    [SerializeField] private string TitleSceneName = "GameStartScene";
    
    private bool isPaused = false;
    
    void Start()
    {
        // ポーズボタンのクリックイベントを設定
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }
        
        // 再開ボタンのクリックイベントを設定
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(Resume);
        }
        
        // タイトルボタンのクリックイベントを設定
        if (titleButton != null)
        {
            titleButton.onClick.AddListener(GoToTitle);
        }
        
        // 開始時はポーズメニューを非表示
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        
        // 開始時はグレーオーバーレイを非表示
        if (grayOverlay != null)
        {
            grayOverlay.gameObject.SetActive(false);
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
        
        // グレーオーバーレイを表示
        if (grayOverlay != null)
        {
            grayOverlay.gameObject.SetActive(true);
        }
        
        // ポーズメニューを表示
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(true);
        }
        
        Debug.Log("ゲームをポーズしました");
    }
    
    void ResumeGame()
    {
        Time.timeScale = 1f; // ゲーム時間を再開
        
        // グレーオーバーレイを非表示
        if (grayOverlay != null)
        {
            grayOverlay.gameObject.SetActive(false);
        }
        
        // ポーズメニューを非表示
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
    
    public void GoToTitle()
    {
        Time.timeScale = 1f; // タイトルに戻る前に時間を正常に戻す
        SceneManager.LoadScene(TitleSceneName); // タイトルシーンの名前に変更してください
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}