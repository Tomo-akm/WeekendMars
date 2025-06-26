using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    // GameManager.csを参考にシングルトンパターンにしてみた
    public static SEPlayer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーンを越えて保持
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource audioSource; //このスクリプトをアタッチするオブジェクト自身(SoundManager)

    // ここからBGM
    [SerializeField] private AudioSource bgmAudioSource;
    public void PlayBGM(AudioClip clip)
    {
        if (bgmAudioSource.clip == clip) return; // すでに同じ曲が流れていたら何もしない
        bgmAudioSource.clip = clip; // 再生するBGMをセット
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void StopBGM()
    {
        bgmAudioSource.Stop();
    }
    [SerializeField] private AudioClip titleBGM;
    public void PlaytitleBGM()
    {
        PlayBGM(titleBGM);
    }

    [SerializeField] private AudioClip defaultBGM;
    public void PlaydefaultBGM()
    {
        PlayBGM(defaultBGM);
    }

    [SerializeField] private AudioClip gameClearBGM;

    public void PlayGameClearBGM()
    {
        PlayBGM(gameClearBGM);
    }

    // ここから効果音

    // ボタンクリック音
    public AudioClip buttonClickSE;
    public void PlayButtonSE()
    {
        audioSource.PlayOneShot(buttonClickSE);
    }
    // 射撃音
    public AudioClip bulletSE;
    public void PlayBulletSE()
    {
        audioSource.PlayOneShot(bulletSE);
    }

    // 敵死亡音
    public AudioClip enemyDieSE;

    public void PlayEnemyDieSE()
    {
        audioSource.PlayOneShot(enemyDieSE);
    }

    // 敵着弾音
    public AudioClip damageSE;
    public void PlaydamageSE()
    {
        audioSource.PlayOneShot(damageSE);
    }

    // タワー設置音
    public AudioClip placeTowerSE;
    public void PlayplaceTowerSE()
    {
        audioSource.PlayOneShot(placeTowerSE);
    }

    // // タワーアップグレード音
    // public AudioClip towerUpgradeSE;
    // public void PlaytowerUpgradeSE()
    // {
    //     audioSource.PlayOneShot(towerUpgradeSE);
    // }

    // ゲームクリア効果音
    public AudioClip gameClearSE;
    public void PlaygameClearSE()
    {
        audioSource.PlayOneShot(gameClearSE);
    }
}