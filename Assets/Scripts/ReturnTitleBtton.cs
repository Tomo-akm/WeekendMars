using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturntitleButton : MonoBehaviour
{
    void Start()
    {
        // ボタンが押されたときに StartGame を呼び出す
        GetComponent<Button>().onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // GameStartSceanに遷移
        SceneManager.LoadScene("GameStartScene");
    }
}
