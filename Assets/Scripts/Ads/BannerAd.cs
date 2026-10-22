using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class BannerAd : MonoBehaviour
{
    [SerializeField] string _AndroidAdUnitId = "BP_Banner_Android";
    string _adUnitId;

    [SerializeField] Button _bannerButton;
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
        _bannerButton.interactable = true;
    }

    void OnBannerError(string message)
    {
        Debug.Log($"Banner error: {message}");
        LoadBanner();
    }

    public void showBannerAd()
    {
        if (isBannerVisible)
            HideBannerAd();
        else
        {
            BannerOptions bannerOptions = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };
            Advertisement.Banner.Show(_adUnitId, bannerOptions);
        }
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

    public void SetButton(Button button)
    {
        if (button == null)
            return;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(showBannerAd);
        _bannerButton = button;
        _bannerButton.interactable = false;
    }
}
