using UnityEngine;

public class SEPlayer : MonoBehaviour
{
    public AudioClip buttonClickSE;
    public AudioClip bulletSE;
    public AudioSource audioSource;

    public void PlayButtonSE()
    {
        audioSource.PlayOneShot(buttonClickSE);
    }
    public void PlayBulletSE()
    {
        audioSource.PlayOneShot(bulletSE);
    }
}