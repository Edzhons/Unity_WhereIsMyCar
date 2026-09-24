using UnityEngine;

public class MenuAudioScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonSound;

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonSound);
    }
}