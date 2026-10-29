using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InterstitialAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    string _adUnitId;
    public event Action OnInterstitialAdReady;
    public bool isReady = false;
    [SerializeField] Button _interstitialAdButton;

    // Watching the ad from CityScene's button slows the game down as a reward.
    private const string SLOW_MOTION_SCENE = "CityScene";
    private bool rewardSlowMotion = false;
    private bool isLoading = false;
    private float timeScaleBeforeAd = 1f;
    private Coroutine slowMotionRoutine;

    void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }

    private void Update()
    {
        if (_interstitialAdButton != null)
            _interstitialAdButton.interactable = isReady;
    }


    public void OnInterstitialAdButtonClicked()
    {
        Debug.Log("Interstitial Ad Button Clicked");
        ShowInterstitial();
    }

    public void ShowInterstitial()
    {
        if (isReady)
        {
            Debug.Log("Showing interstitial ad manually!");
            ShowAd(true);
        }
        else
        {
            Debug.LogWarning("Interstitial ad is not ready yet, loading again!");
            LoadAd();
        }
    }

    /// <param name="withReward">True when the player chose to watch it (slow motion in CityScene).</param>
    public void ShowAd(bool withReward)
    {
        if (!isReady)
        {
            Debug.LogWarning("Interstitial ad is not ready yet!");
            return;
        }

        rewardSlowMotion = withReward;
        timeScaleBeforeAd = Time.timeScale;

        if (AdManager.Instance != null)
            AdManager.Instance.bannerAd.HideBannerAd();

        Advertisement.Show(_adUnitId, this);
        isReady = false;
    }

    public void LoadAd()
    {
        if(!Advertisement.isInitialized)
        {
            Debug.LogWarning("Tried to load interstitial ad before Unity Ads was initialized!");
            return;
        }

        if (isReady || isLoading)
            return;

        Debug.Log("Loading interstitial ad...");
        isLoading = true;
        Advertisement.Load(_adUnitId, this);
    }


    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Interstitial ad loaded successfully!");
        isLoading = false;
        isReady = true;
        OnInterstitialAdReady?.Invoke();
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.LogError($"Failed to load interstitial ad: {error.ToString()} - {message}");
        isLoading = false;
        StartCoroutine(LoadAgainLater(10f));
    }

    private IEnumerator LoadAgainLater(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadAd();
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("Interstitial ad clicked!");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        Time.timeScale = timeScaleBeforeAd;

        bool inCity = SceneManager.GetActiveScene().name == SLOW_MOTION_SCENE;
        if (showCompletionState == UnityAdsShowCompletionState.COMPLETED && rewardSlowMotion && inCity)
        {
            Debug.Log("Interstitial ad completed successfully!");
            slowMotionRoutine = StartCoroutine(SlowDownTimeTemporarily(30f));
        }
        else if (showCompletionState != UnityAdsShowCompletionState.COMPLETED)
        {
            Debug.LogWarning($"Interstitial ad was not completed: {showCompletionState.ToString()}");
        }

        rewardSlowMotion = false;
        AfterAd();
    }

    private IEnumerator SlowDownTimeTemporarily(float duration)
    {
        Time.timeScale = 0.3f;
        Debug.Log("Time slowed down for " + duration + " seconds.");
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1f;
        slowMotionRoutine = null;
        Debug.Log("Time restored to normal.");
    }

    /// <summary>Called on every scene load, so slow motion stays in its scene.</summary>
    public void StopSlowMotion()
    {
        if (slowMotionRoutine == null)
            return;

        StopCoroutine(slowMotionRoutine);
        slowMotionRoutine = null;
        Time.timeScale = 1f;
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogError($"Failed to show interstitial ad: {error.ToString()} - {message}");
        Time.timeScale = timeScaleBeforeAd;
        rewardSlowMotion = false;
        AfterAd();
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Debug.Log("Interstitial ad started showing!");
        Time.timeScale = 0f;
    }

    private void AfterAd()
    {
        if (AdManager.Instance != null)
            AdManager.Instance.bannerAd.ShowBanner();
        LoadAd();
    }

   public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnInterstitialAdButtonClicked);
        _interstitialAdButton = button;
        _interstitialAdButton.interactable = isReady;
    }

}
