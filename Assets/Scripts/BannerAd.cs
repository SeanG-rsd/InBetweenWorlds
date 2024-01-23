using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class BannerAd : MonoBehaviour
{

    [SerializeField] BannerPosition _bannerPosition = BannerPosition.BOTTOM_CENTER;

    //[SerializeField] string _androidAdUnitId = "Banner_Android";
    [SerializeField] string _iOSAdUnitId = "Banner_iOS";
    string _adUnitId = null; // This will remain null for unsupported platforms.

    bool hasBeenLoaded;

    private void Awake()
    {
        GameManager.OnPause += ToggleBanner;
        GameManager.OnStartGame += HandleAction;
        RewardedAd.OnRevivePlayer += HandleAction;
        Player.OnDeath += HandlePlayerDeath;
    }

    private void OnDestroy()
    {
        GameManager.OnPause -= ToggleBanner;
        RewardedAd.OnRevivePlayer -= HandleAction;
        GameManager.OnStartGame += HandleAction;
        Player.OnDeath -= HandlePlayerDeath;
    }

    void Start()
    {
        // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#elif UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#endif

        // Disable the button until an ad is ready to show:
        //_showBannerButton.interactable = false;
        //_hideBannerButton.interactable = false;

        // Set the banner position:
        Advertisement.Banner.SetPosition(_bannerPosition);

        // Configure the Load Banner button to call the LoadBanner() method when clicked:
        //_loadBannerButton.onClick.AddListener(LoadBanner);
        //_loadBannerButton.interactable = true;

        LoadBanner();
    }

    // Implement a method to call when the Load Banner button is clicked:
    public void LoadBanner()
    {
        // Set up options to notify the SDK of load events:
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        // Load the Ad Unit with banner content:
        Advertisement.Banner.Load(_adUnitId, options);
    }

    // Implement code to execute when the loadCallback event triggers:
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded");

        // Configure the Show Banner button to call the ShowBannerAd() method when clicked:
        //_showBannerButton.onClick.AddListener(ShowBannerAd);
        // Configure the Hide Banner button to call the HideBannerAd() method when clicked:
        //_hideBannerButton.onClick.AddListener(HideBannerAd);

        // Enable both buttons:
        //_showBannerButton.interactable = true;
        //_hideBannerButton.interactable = true;
    }

    // Implement code to execute when the load errorCallback event triggers:
    void OnBannerError(string message)
    {
        Debug.Log($"Banner Error: {message}");
        // Optionally execute additional code, such as attempting to load another ad.
    }

    private void HandleAction()
    {
        ShowBannerAd(false);
    }

    private void HandlePlayerDeath()
    {
        ShowBannerAd(true);
    }

    private void ToggleBanner(bool isPaused)
    {
        ShowBannerAd(isPaused);
    }

    // Implement a method to call when the Show Banner button is clicked:
    void ShowBannerAd(bool isPaused)
    {
        // Set up options to notify the SDK of show events:
        BannerOptions options = new BannerOptions
        {
            
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        // Show the loaded Banner Ad Unit:
        if (isPaused)
        {
            Advertisement.Banner.Hide();
        }
        else
        {
            Debug.Log("Banner loaded");
            Advertisement.Banner.Show(_adUnitId, options);
        }
    }

    void OnBannerShown() { }
    void OnBannerHidden() { }
}
