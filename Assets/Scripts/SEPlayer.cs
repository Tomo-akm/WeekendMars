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
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public AudioSource audioSource; //このスクリプトをアタッチするオブジェクト自身(SoundManager)

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
}