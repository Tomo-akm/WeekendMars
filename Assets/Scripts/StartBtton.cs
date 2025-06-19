using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
     [Header("Scene Names")]
     [SerializeField] private string gameSceneName = "map1";

    void Start()
    {
        // ボタンが押されたときに StartGame を呼び出す
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // map1Sceanに遷移
        SceneManager.LoadScene(gameSceneName);
    }
}
