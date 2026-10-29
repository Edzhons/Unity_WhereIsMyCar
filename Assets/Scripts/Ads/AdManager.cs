using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The AdsScriptHolder prefab is placed in every scene. The first one survives
/// scene loads; the copies in the scenes loaded later remove themselves.
/// Every scene load shows an interstitial (unless "no ads" was bought in the
/// shop) and the banner stays visible all the time.
/// </summary>
public class AdManager : MonoBehaviour
{
    public AdsInitializer adsInitializer;
    public InterstitialAd interstitialAd;
    [SerializeField] bool turnOffInterstitialAds = false;

    public RewardedAds rewardedAds;
    [SerializeField] bool turnOffRewardedAds = false;

    public BannerAd bannerAd;
    [SerializeField] bool turnOffBannerAds = false;

    public static AdManager Instance { get; private set; }

    // Set by a scene load, cleared once the interstitial for it was shown.
    private bool sceneAdWaiting = false;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (adsInitializer == null)
            adsInitializer = GetComponent<AdsInitializer>();

        adsInitializer.OnAdsInitialized += HandleAdsInitialized;
        interstitialAd.OnInterstitialAdReady += HandleInterstitialReady;
    }

    private void HandleAdsInitialized()
    {
        if (!turnOffInterstitialAds)
            interstitialAd.LoadAd();

        if (!turnOffRewardedAds)
            rewardedAds.LoadAd();

        if (!turnOffBannerAds)
            bannerAd.LoadBanner();
    }

    private void HandleInterstitialReady()
    {
        if (sceneAdWaiting)
            ShowSceneAd();
    }

    private void ShowSceneAd()
    {
        Debug.Log("Showing interstitial ad for the scene load!");
        sceneAdWaiting = false;
        interstitialAd.ShowAd(false);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // The copy in a newly loaded scene is about to be destroyed.
        if (Instance != this)
            return;

        // Slow motion from CityScene's interstitial button must not carry over.
        interstitialAd.StopSlowMotion();

        // Ad buttons only exist in CityScene.
        GameObject interstitialButton = GameObject.FindGameObjectWithTag("InterstitialAdButton");
        if (interstitialButton != null)
            interstitialAd.SetButton(interstitialButton.GetComponent<Button>());

        GameObject rewardedAdButton = GameObject.FindGameObjectWithTag("RewardedAdButton");
        if (rewardedAdButton != null)
            rewardedAds.SetButton(rewardedAdButton.GetComponent<Button>());

        if (!turnOffBannerAds)
            bannerAd.ShowBanner();

        if (turnOffInterstitialAds || StarBank.HasItem(StarBank.NO_ADS))
            return;

        sceneAdWaiting = true;
        if (interstitialAd.isReady)
            ShowSceneAd();
        else
            interstitialAd.LoadAd();
    }
}
