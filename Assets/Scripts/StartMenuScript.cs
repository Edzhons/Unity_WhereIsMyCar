using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenuScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip buttonSound;

    public void StartGame()
    {
        StartCoroutine(StartGameWithSound());
    }

    public void QuitGame()
    {
        StartCoroutine(QuitGameWithSound());
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
}