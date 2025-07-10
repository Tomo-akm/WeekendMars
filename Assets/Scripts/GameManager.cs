using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    // シングルトンパターン実装
    public static GameManager instance;

    // ゲーム設定
    [Header("Game Settings")]
    [SerializeField] private float gameTime = 10f; // ゲーム時間（秒）
    private float currentTime; // 現在の残り時間
    private bool isGameOver = false; // ゲーム終了フラグ
    private bool isGameClear = false; // ゲームクリアフラグ

    // お金システム
    [Header("Money System")]
    [SerializeField] private int playerMoney = 50; // プレイヤーの所持金
    [SerializeField] private TextMeshProUGUI moneyText; // お金表示用テキスト

    // UI要素
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI timeText; // 時間表示テキスト
    [SerializeField] private GameObject gameOverPanel; // ゲームオーバーパネル
    [SerializeField] private GameObject gameClearPanel; // ゲームクリアパネル

    // シーン名
    [Header("Scene Names")]
    [SerializeField] private string gameOverSceneName = "GameOverScene";
    [SerializeField] private string gameClearSceneName = "GameClearScene";
    [SerializeField] private float sceneTransitionDelay = 1.0f; // シーン遷移までの待機時間

    private void Awake()
    {
        // シングルトンパターン
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 初期化
        InitializeGame();
    }

    private void InitializeGame()
    {
        // 時間の初期化
        currentTime = gameTime;
        isGameOver = false;
        isGameClear = false;

        // パネルを非表示に
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (gameClearPanel != null)
            gameClearPanel.SetActive(false);

        // 時間表示の更新
        UpdateTimeDisplay();

        //　お金表示の更新
        UpdateMoneyDisplay();

        // 基本BGMの再生
        SEPlayer.instance.PlaydefaultBGM();
    }

    private void Update()
    {
        // ゲームが終了していなければ時間を更新
        if (!isGameOver && !isGameClear)
        {
            UpdateGameTime();
        }
    }

    private void UpdateGameTime()
    {
        // 残り時間をカウントダウン
        currentTime -= Time.deltaTime;

        // 時間表示の更新
        UpdateTimeDisplay();

        // 時間切れチェック
        if (currentTime <= 0)
        {
            currentTime = 0;
            GameClear(); // 時間経過でゲームクリア
        }
    }

    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            // 分と秒に変換
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // 00:00 形式で表示
            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // ゲームオーバー処理
    public void GameOver()
    {
        if (!isGameOver && !isGameClear)
        {
            isGameOver = true;
            Debug.Log("Game Over");

            // タイマーUIを非表示にする
            if (timeText != null)
                timeText.gameObject.SetActive(false);

            // ゲームオーバーパネルを表示
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            // シーン遷移
            StartCoroutine(LoadSceneAfterDelay(gameOverSceneName, sceneTransitionDelay));
            SEPlayer.instance.StopBGM();
            SEPlayer.instance.PlaygameOverBGM();
        }
    }

    // ゲームクリア処理
    public void GameClear()
    {
        if (!isGameOver && !isGameClear)
        {
            isGameClear = true;
            Debug.Log("Game Clear");

            // タイマーUIを非表示にする
            if (timeText != null)
                timeText.gameObject.SetActive(false);

            // ゲームクリアパネルを表示
            if (gameClearPanel != null)
                gameClearPanel.SetActive(true);

            // ゲームクリア効果音を再生
            SEPlayer.instance.StopBGM();
            SEPlayer.instance.PlaygameClearSE_BGM();

            // シーン遷移
            StartCoroutine(LoadSceneAfterDelay(gameClearSceneName, sceneTransitionDelay));
        }
    }

    // 遅延付きシーン遷移
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }

    // 公開メソッド：残り時間の取得
    public float GetRemainingTime()
    {
        return currentTime;
    }

    // 公開メソッド：ゲーム時間の設定
    public void SetGameTime(float newTime)
    {
        gameTime = newTime;
        if (!isGameOver && !isGameClear)
        {
            currentTime = gameTime;
        }
    }

    // デバッグ用：時間を追加する
    public void AddTime(float timeToAdd)
    {
        if (!isGameOver && !isGameClear)
        {
            currentTime += timeToAdd;
            UpdateTimeDisplay();
        }
    }

    // お金の表示を更新
    private void UpdateMoneyDisplay()
    {
        if (moneyText != null)
        {
            moneyText.text = playerMoney.ToString() + " G";
        }
    }

    // 敵を倒してお金を獲得する
    public void AddMoney(int amount, Vector3 enemyPosition = default)
    {
        if (!isGameOver && !isGameClear)
        {
            // お金を追加
            playerMoney += amount;
            
            // 表示を更新
            UpdateMoneyDisplay();
            
            Debug.Log(amount + "Gを獲得しました！ 現在の所持金: " + playerMoney + "G");
        }
    }
    // お金を消費する
    public bool SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            UpdateMoneyDisplay();
            return true; // 支払い成功
        }
        
        return false; // お金が足りない
    }
    
    // 現在の所持金を取得
    public int GetCurrentMoney()
    {
        return playerMoney;
    }
}