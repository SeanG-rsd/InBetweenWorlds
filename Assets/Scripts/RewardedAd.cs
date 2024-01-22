
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using TMPro;
using System;

public class RewardedAd : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    //[SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    [SerializeField] string _andriodAdUnitId = "Rewarded_Andriod";
    string _adUnitId = null; // This will remain null for unsupported platforms

    [SerializeField] private TMP_Text timerText;
    [SerializeField] private float timer;
    private float time;

    [SerializeField] private GameObject continueScreen;

    [SerializeField] private Button playAdButton;

    public static Action OnLeaveContinueScreen = delegate { };
    public static Action OnRevivePlayer = delegate { };

    [SerializeField] private int maxAdsPerRun;
    private int currentRunCount;

    private void Awake()
    {
        playAdButton.interactable = false;
        continueScreen.SetActive(false);
        // Get the Ad Unit ID for the current platform:
        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
                    ? _iOSAdUnitId
                    : _andriodAdUnitId;

        //Disable the button until the ad is ready to show:
        Player.OnDeath += HandlePlayerDeath;
        time = timer;
        LoadAd();
    }

    private void OnDestroy()
    {
        Player.OnDeath -= HandlePlayerDeath;
    }

    private void Update()
    {
        if (continueScreen.activeSelf)
        {
            time -= Time.deltaTime;
            timerText.text = ((int)time).ToString();
        }

        if (time < 0)
        {
            Skip();
        }
    }

    private void HandlePlayerDeath()
    {
        continueScreen.SetActive(true);
    }

    public void Skip()
    {
        currentRunCount = 0;
        continueScreen.SetActive(false);
        time = timer;
        OnLeaveContinueScreen?.Invoke();
    }
    // Load content to the Ad Unit:
    public void LoadAd()
    {
        // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
        //Debug.Log("Loading Ad: " + _adUnitId);
        Advertisement.Load(_adUnitId, this);
    }

    // If the ad successfully loads, add a listener to the button and enable it:
    public void OnUnityAdsAdLoaded(string adUnitId)
    {
        Debug.Log("Ad Loaded: " + adUnitId);

        if (adUnitId.Equals(_adUnitId))
        {
            if (currentRunCount <= maxAdsPerRun)
            {
                playAdButton.interactable = true;
            }
            // Configure the button to call the ShowAd() method when clicked:
            //_showAdButton.onClick.AddListener(ShowAd);
            // Enable the button for users to click:
        }
    }

    // Implement a method to execute when the user clicks the button:
    public void ShowAd()
    {
        // Disable the button:
        // Then show the ad:
        currentRunCount++;
        playAdButton.interactable = false;

        Advertisement.Show(_adUnitId, this);
    }

    // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
    public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            continueScreen.SetActive(false);
            time = timer;
            OnRevivePlayer?.Invoke();
            LoadAd();
            // Grant a reward.
        }
    }

    // Implement Load and Show Listener error callbacks:
    public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
        // Use the error details to determine whether to try to load another ad.
    }

    public void OnUnityAdsShowStart(string adUnitId) { }
    public void OnUnityAdsShowClick(string adUnitId) { }
}