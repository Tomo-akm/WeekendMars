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

    [Header("オプション設定")]
    public Button optionsButton;
    public GameObject optionsPanel;
    public Button backButton;

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

        // オプションボタンのクリックイベントを設定
        if (optionsButton != null)
        {
            optionsButton.onClick.AddListener(OpenOptions);
        }

        //戻るボタンのクリックイベントを設定
        if (backButton != null)
        { 
            backButton.onClick.AddListener(CloseOptions);
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

        //開始時はオプションパネルを非表示に
        if (optionsPanel != null)
        {
            optionsPanel.SetActive(false);
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

        //ポーズボタンを無効化する 
        if (pauseButton != null)
        {
            pauseButton.interactable = false;
        }
        
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


        //ポーズボタンを再度有効化する
        if (pauseButton != null)
        {
            pauseButton.interactable = true;
        }

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
    
    //オプションパネルを開くメソッド
    void OpenOptions()
    {
        pauseMenu.SetActive(false);
        optionsPanel.SetActive(true);
    }

    //オプションパネルを閉じるメソッド
    void CloseOptions()
    {
        optionsPanel.SetActive(false);
        pauseMenu.SetActive(true);
    }
}