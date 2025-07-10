// VolumeUISetup.cs

using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider volumeSlider;

    void Start()
    {
        // SEPlayerが存在しなければ何もしない
        if (SEPlayer.instance == null)
        {
            Debug.LogWarning("SEPlayer.instance not found.");
            return;
        }

        // --- スライダーの初期値を設定 ---
        // SEPlayerから現在の音量を取得してスライダーに反映
        volumeSlider.value = SEPlayer.instance.GetVolume();

        // --- スライダーのイベントに関数を登録 ---
        // スライダーが動かされたら、SEPlayerの音量設定メソッドを呼び出すようにする
        volumeSlider.onValueChanged.AddListener(SEPlayer.instance.SetVolume);
    }
}