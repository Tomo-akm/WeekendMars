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
        // map1Sceanに遷移
        SceneManager.LoadScene("map1");
    }
}
