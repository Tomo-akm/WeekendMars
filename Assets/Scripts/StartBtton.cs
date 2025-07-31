using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StartButton : MonoBehaviour
{
     [Header("Scene Names")]
     [SerializeField] private string gameSceneName = "map1";

    [Header("PlayerName")]
    [SerializeField] private TMP_InputField nameInputField; 

     // ゲーム全体で共有されるプレイヤー名
    public static string CurrentPlayerName { get; private set; }

    void Start()
    {
        // タイトルBGMを再生
        SEPlayer.instance.PlaytitleBGM();
        // ボタンが押されたときに StartGame を呼び出す
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
      {
        // InputFieldから名前を取得して保存
        if (nameInputField != null)
        {
            CurrentPlayerName = nameInputField.text;
        }

        // もし名前が入力されていなかったら、仮の名前を入れる
        if (string.IsNullOrEmpty(CurrentPlayerName))
        {
            CurrentPlayerName = "Guest";
        }

        Debug.Log("プレイヤー名: " + CurrentPlayerName + " でゲームを開始します。");

        // ゲームシーンに遷移
        SceneManager.LoadScene(gameSceneName);
    }
}
