using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManagerScript : MonoBehaviour
{
    [Header("End Game Sounds")]
    public AudioSource audioSource;
    public AudioClip winSound;
    public AudioClip loseSound;

    [Header("Game Settings")]
    public int maxLives = 3;

    [Header("UI")]
    public GameObject endGamePanel;
    public GameObject[] lifeImages;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text finalTimeText;
    public GameObject[] starImages;

    [Header("Star Time Limits")]
    public float threeStarTime = 120f;
    public float twoStarTime = 240f;
    public float oneStarTime = 360f;

    private int currentLives;
    private int placedCars = 0;
    private int totalCars;

    private float elapsedTime = 0f;

    private bool gameEnded = false;

    private bool endingGame = false;

    void Start()
    {
        currentLives = maxLives;

        GameObjectsScript gameObjectsScript =
            FindFirstObjectByType<GameObjectsScript>();

        totalCars = gameObjectsScript.cars.Length;

        endGamePanel.SetActive(false);

        UpdateLivesUI();
        UpdateTimerUI();
    }


    void Update()
    {
        // TEST KEY - instantly win the game
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ForceWin();
        }

        if (gameEnded)
            return;

        elapsedTime += Time.deltaTime;

        UpdateTimerUI();
    }

    void ForceWin()
    {
        if (gameEnded)
            return;

        Debug.Log("TEST: Force Win");

        placedCars = totalCars;

        WinGame();
    }


    // =========================
    // LIVES
    // =========================

    public void LoseLife()
    {
        if (gameEnded || endingGame)
            return;

        currentLives--;

        UpdateLivesUI();

        Debug.Log("Life lost! Remaining lives: " + currentLives);

        if (currentLives <= 0)
        {
            endingGame = true;

            StartCoroutine(DelayedLoseGame());
        }
    }
    IEnumerator DelayedLoseGame()
    {
        // Give the bomb/disappear animation time to finish
        yield return new WaitForSeconds(1f);

        LoseGame();
    }


    void UpdateLivesUI()
    {
        for (int i = 0; i < lifeImages.Length; i++)
        {
            lifeImages[i].SetActive(i < currentLives);
        }
    }


    // =========================
    // CAR PLACEMENT
    // =========================

    public void CorrectCarPlaced()
    {
        if (gameEnded)
            return;

        placedCars++;

        Debug.Log(
            "Correct cars: " +
            placedCars +
            "/" +
            totalCars
        );

        if (placedCars >= totalCars)
        {
            WinGame();
        }
    }


    // =========================
    // TIMER
    // =========================

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int hours = Mathf.FloorToInt(elapsedTime / 3600f);
            int minutes = Mathf.FloorToInt((elapsedTime % 3600f) / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            timerText.text =
                string.Format(
                    "{0:00}:{1:00}:{2:00}",
                    hours,
                    minutes,
                    seconds
                );
        }
    }


    // =========================
    // WIN
    // =========================

    void WinGame()
    {
        gameEnded = true;

        if (audioSource != null && winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }

        Time.timeScale = 0f;

        resultText.text = "UZVARA!";

        finalTimeText.text =
            "Laiks: " +
            FormatTime(elapsedTime);

        UpdateStars();

        endGamePanel.SetActive(true);
    }


    // =========================
    // LOSE
    // =========================

    void HideAllStars()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(false);
        }
    }

    void LoseGame()
    {
        gameEnded = true;

        if (audioSource != null && loseSound != null)
        {
            audioSource.PlayOneShot(loseSound);
        }

        Time.timeScale = 0f;

        resultText.text = "SPĒLE BEIGUSIES";

        finalTimeText.text =
            "Laiks: " +
            FormatTime(elapsedTime);

        HideAllStars();

        endGamePanel.SetActive(true);
    }


    // =========================
    // STARS
    // =========================

    int GetStarCount()
    {
        if (elapsedTime <= threeStarTime)
        {
            return 3;
        }

        if (elapsedTime <= twoStarTime)
        {
            return 2;
        }

        if (elapsedTime <= oneStarTime)
        {
            return 1;
        }

        return 0;
    }

    void UpdateStars()
    {
        int stars = GetStarCount();

        for (int i = 0; i < starImages.Length; i++)
        {
            starImages[i].SetActive(i < stars);
        }
    }


    // =========================
    // TIME FORMAT
    // =========================

    string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600f);
        int minutes = Mathf.FloorToInt((time % 3600f) / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        return string.Format(
            "{0:00}:{1:00}:{2:00}",
            hours,
            minutes,
            seconds
        );
    }


    // =========================
    // BUTTONS
    // =========================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }


    public void ReturnToMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("StartMenu");
    }
}