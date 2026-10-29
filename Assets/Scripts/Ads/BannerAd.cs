using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

/// <summary>The banner is always on; it only hides while a full-screen ad plays.</summary>
public class BannerAd : MonoBehaviour
{
    [SerializeField] string _AndroidAdUnitId = "BP_Banner_Android";
    string _adUnitId;

    public bool isBannerLoaded = false;
    public bool isBannerVisible = false;

    [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    void Awake()
    {
        _adUnitId = _AndroidAdUnitId;
        Advertisement.Banner.SetPosition(bannerPosition);
    }

    public void LoadBanner()
    {
        if(!Advertisement.isInitialized)
        {
            Debug.Log("Tried to load banner ad before Unity ads initialized");
            return;
        }

        BannerLoadOptions bannerOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(_adUnitId, bannerOptions);
    }

    void OnBannerLoaded()
    {
        Debug.Log("Banner ad loaded!");
        isBannerLoaded = true;
        ShowBanner();
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner error: {message}");
        StartCoroutine(LoadAgainLater(10f));
    }

    IEnumerator LoadAgainLater(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadBanner();
    }

    public void ShowBanner()
    {
        if (!isBannerLoaded || isBannerVisible)
            return;

        BannerOptions bannerOptions = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };
        Advertisement.Banner.Show(_adUnitId, bannerOptions);
    }

    void OnBannerClicked()
    {
        Debug.Log("User clicked on banner ad");
    }

    void OnBannerHidden()
    {
        Debug.Log("Banner ad is hidden! " + _adUnitId);
        isBannerVisible = false;
    }

    void OnBannerShown()
    {
        Debug.Log("Banner ad is visible! " + _adUnitId);
        isBannerVisible = true;
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide();
        isBannerVisible = false;
    }
}
