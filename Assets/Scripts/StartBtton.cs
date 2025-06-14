using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    void Start()
    {
        // ボタンが押されたときに StartGame を呼び出す
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // 指定のSceanに遷移
        SceneManager.LoadScene("map1-25-06-12-komi");
    }
}
