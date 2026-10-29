using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

/// <summary>
/// Rewarded ads. Whoever shows the ad passes a callback that gets true only
/// when Unity Ads reports COMPLETED, and gives its own reward.
/// </summary>
public class RewardedAds : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    string _adUnitId;

    [SerializeField] Button _rewardedAdButton;

    public bool IsReady { get; private set; }

    private Action<bool> onAdFinished;
    private float timeScaleBeforeAd = 1f;

    private void Awake()
    {
        _adUnitId = _androidAdUnitId;
    }

    public void LoadAd()
    {
        if (!Advertisement.isInitialized)
        {
            Debug.Log("Tried to load rewarded ad before Unity Ads was initialized.");
            return;
        }

        Debug.Log("Loading Rewarded ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);

    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log("Rewarded ad loaded: " + placementId);

        if (placementId.Equals(_adUnitId))
        {
            IsReady = true;
            if (_rewardedAdButton != null)
                _rewardedAdButton.interactable = true;
        }
    }

    /// <summary>CityScene's button: the reward destroys every flying object.</summary>
    public void ShowAd()
    {
        ShowForReward(completed =>
        {
            FlyingObjectManager flyingObjectManager = FindFirstObjectByType<FlyingObjectManager>();
            if (completed && flyingObjectManager != null)
                flyingObjectManager.DestroyAllFlyingObjects();
        });
    }

    /// <summary>Tower of Hanoi hint / undo.</summary>
    public bool ShowForHanoiReward(Action<bool> onFinished)
    {
        return ShowForReward(onFinished);
    }

    public bool ShowForReward(Action<bool> onFinished)
    {
        if (!IsReady)
            return false;

        IsReady = false;
        onAdFinished = onFinished;
        timeScaleBeforeAd = Time.timeScale;

        if (_rewardedAdButton != null)
            _rewardedAdButton.interactable = false;

        if (AdManager.Instance != null)
            AdManager.Instance.bannerAd.HideBannerAd();

        Advertisement.Show(_adUnitId, this);
        return true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading rewarded ad {placementId} : {error.ToString()} - {message}");
        StartCoroutine(WaitAndLoadAd(10f));
    }

    private IEnumerator WaitAndLoadAd(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadAd();
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing rewarded ad: {placementId} : {error.ToString()} - {message}");
        Finish(false);
        StartCoroutine(WaitAndLoadAd(10f));
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        Time.timeScale = 0f;
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        Debug.Log("User clicked on rewarded ad: " + placementId);
    }

    public void OnUnityAdsShowComplete(string placementId,
    UnityAdsShowCompletionState showCompletionState)
    {
        bool completed = placementId.Equals(_adUnitId) &&
            showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED);

        if (completed)
            Debug.Log("Rewarded ad completed!");

        Finish(completed);
        StartCoroutine(WaitAndLoadAd(completed ? 15f : 1f));
    }

    private void Finish(bool completed)
    {
        Time.timeScale = timeScaleBeforeAd;

        Action<bool> callback = onAdFinished;
        onAdFinished = null;
        callback?.Invoke(completed);

        if (AdManager.Instance != null)
            AdManager.Instance.bannerAd.ShowBanner();
    }

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(ShowAd);
        _rewardedAdButton = button;
        _rewardedAdButton.interactable = IsReady;
    }
}
