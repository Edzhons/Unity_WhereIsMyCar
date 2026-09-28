using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenuScript : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip hornSound;

    public Animator blinkerAnimator;
    public Animator blinkerAnimator2;

    private bool emergencyLightsOn = true;

    public void StartGame()
    {
        StartCoroutine(StartGameWithSound());
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameWithSound());
    }

    public void PlayHornSound()
    {
        StartCoroutine(PlayHorn());
    }

    public void ToggleMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Pause();
        }
        else
        {
            musicSource.UnPause();
        }
    }


    IEnumerator StartGameWithSound()
    {
        // Play button sound
        audioSource.PlayOneShot(buttonSound);

        // Wait for the sound to finish
        yield return new WaitForSeconds(buttonSound.length);

        // Load the game scene
        SceneManager.LoadScene("CityScene");
    }


    IEnumerator QuitGameWithSound()
    {
        // Play button sound
        audioSource.PlayOneShot(buttonSound);

        // Wait for the sound to finish
        yield return new WaitForSeconds(buttonSound.length);

        // Quit the game
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    IEnumerator PlayHorn()
    {
        // Play horn sound
        audioSource.PlayOneShot(hornSound);

        // Wait for the sound to finish
        yield return new WaitForSeconds(hornSound.length);
    }

    public void ToggleEmergencyLights()
    {
        emergencyLightsOn = !emergencyLightsOn;

        if (emergencyLightsOn)
        {
            // Turn both blinkers back on
            blinkerAnimator.gameObject.SetActive(true);
            blinkerAnimator2.gameObject.SetActive(true);

            // Enable their animations
            blinkerAnimator.enabled = true;
            blinkerAnimator2.enabled = true;

            // Start both animations from the beginning
            blinkerAnimator.Play("Blinkers", 0, 0f);
            blinkerAnimator2.Play("Blinkers", 0, 0f);

            Debug.Log("Emergency lights ON");
        }
        else
        {
            // Completely disable both blinkers
            blinkerAnimator.gameObject.SetActive(false);
            blinkerAnimator2.gameObject.SetActive(false);

            Debug.Log("Emergency lights OFF");
        }
    }
}