using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("---Player---")]
    [SerializeField] private TMP_Text playerScoreText;
    private int playerScore = 0;
    [SerializeField] private GameObject cargo;
    private Vector3 cargoStartPos;
    [SerializeField] private GameObject player;
    private Player playerComp;
    
    private bool isPlaying;

    [Header("---Map---")]
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Transform objectParent;
    [SerializeField] private DeathWall wall;
    [SerializeField] private MapGenerator mapGenerator;

    [Header("---Menu---")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private Sprite[] mainMenuBackgroundImages;
    [SerializeField] private Image mainMenuBackground;
    [SerializeField] private float timePerFrame;
    [SerializeField] private GameObject titleObjectHolder;
    private bool playAnimation = false;
    private int start;
    private int end;
    private int current;
    private int direction;
    private float frameTimer;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private Image youLostPausedImage;
    [SerializeField] private Sprite[] youLostPausedSprites;
    [SerializeField] private GameObject scoresObject;
    private bool isPaused;

    [Header("---Shop/Settings---")]
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject noAdsScreen;

    public static Action OnStartGame = delegate { };
    public static Action OnGenerateMap = delegate { };
    public static Action<bool> OnPause = delegate { };
    public static Action OnResumeGame = delegate { };

    private void Awake()
    {
        isPlaying = false;
        mainMenu.SetActive(true);
        gameOverMenu.SetActive(false);
        RewardedAd.OnLeaveContinueScreen += HandlePlayerDeath;
        cargoStartPos = cargo.transform.position;
        playerComp = player.GetComponent<Player>();
        
    }

    private void OnDestroy()
    {
        RewardedAd.OnLeaveContinueScreen -= HandlePlayerDeath;
    }

    private void Update()
    {
        if (isPlaying)
        {
            if (tilemap.WorldToCell(cargo.transform.position).x > playerScore)
            {
                playerScore = tilemap.WorldToCell(cargo.transform.position).x;
            }
            playerScoreText.text = playerScore.ToString();
        }

        if (playAnimation)
        {
            if (frameTimer >= timePerFrame)
            {
                current += direction;
                if (current > end || current < start)
                {
                    mainMenu.SetActive(direction > 0 ? false : true);
                    playAnimation = false;
                    titleObjectHolder.SetActive(true);
                    gameOverMenu.SetActive(false);
                    if (direction > 0) { StartGame(); }
                }
                else
                {
                    frameTimer = 0;
                    mainMenuBackground.sprite = mainMenuBackgroundImages[current];
                }
            }
            else
            {
                frameTimer += Time.deltaTime;
            }
        }
    }

    private void HandlePlayerDeath()
    {
        gameOverMenu.SetActive(true);
        gameOverScoreText.text = playerScore.ToString();

        if (PlayerPrefs.HasKey("Score"))
        {
            if (playerScore > PlayerPrefs.GetInt("Score"))
            {
                PlayerPrefs.SetInt("Score", playerScore);
            }
        }
        else
        {
            PlayerPrefs.SetInt("Score", playerScore);
            
        }

        int highscore = PlayerPrefs.GetInt("Score");
        highScoreText.text = highscore.ToString();

        isPlaying = false;
        playerScore = 0;
        playerScoreText.text = playerScore.ToString();
    }

    private void StartGame()
    {
        isPlaying = true;
        wall.Initialize();
        OnStartGame?.Invoke();
        gameOverMenu.SetActive(false);
        settingsScreen.SetActive(false);
        shopScreen.SetActive(false);
        cargo.transform.position = cargoStartPos;
    }

    public void OnClickRestart()
    {
        StartGame();
    }

    public void OnClickMainMenu()
    {
        mainMenu.SetActive(true);
        if (isPaused)
        {
            OnClickPause();
        }
        PlayAnimation(-1);
    }

    public void OnClickPlay()
    {
        OnGenerateMap?.Invoke();
        PlayAnimation(1);
    }

    public void PlayAnimation(int direction)
    {
        titleObjectHolder.SetActive(false);

        this.direction = direction;
        start = 0;
        end = mainMenuBackgroundImages.Length - 1;
        playAnimation = true;
        current = direction > 0 ? start : end;
        frameTimer = 0;
        
    }

    public void OnClickSettingsMenu()
    {
        settingsScreen.SetActive(true);
    }

    public void OnClickShopMenu()
    {
        shopScreen.SetActive(true);
    }

    public void OnClickNoAdsScreen()
    {
        noAdsScreen.SetActive(true);
    }

    public void OnClickExit()
    {
        shopScreen.SetActive(false);
        settingsScreen.SetActive(false);
        noAdsScreen.SetActive(false);
    }

    public void OnClickPause()
    {
        isPaused = !isPaused;
        OnPause?.Invoke(isPaused);
        youLostPausedImage.sprite = youLostPausedSprites[isPaused ? 1 : 0];
        gameOverMenu.SetActive(isPaused);
        restartButton.SetActive(!isPaused);
        continueButton.SetActive(isPaused);
        scoresObject.SetActive(!isPaused);
        
        if (!isPaused)
        {
            shopScreen.SetActive(false);
            settingsScreen.SetActive(false);
        }
    }
}
