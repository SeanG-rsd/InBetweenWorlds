using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyCountText;
    [SerializeField] private Player player;

    [SerializeField] private TMP_Text speedCostText;
    [SerializeField] private TMP_Text airCostText;

    private int airUpgadesBought;
    private int speedUpgradesBought;

    [SerializeField] private int[] UPGRADE_COSTS;

    [Header("---Visual---")]
    [SerializeField] private Transform airUpgradeBar;
    [SerializeField] private Transform speedUpgradeBar;
    [SerializeField] private GameObject airUpgradePrefab;
    [SerializeField] private GameObject speedUpgradePrefab;

    [Header("---Buffs---")]
    [SerializeField] private float[] speedBuffs;
    [SerializeField] private float[] airBuffs;

    private string AIR_UPGRADES = "AIR_UPGRADES";
    private string SPEED_UPGRADES = "SPEED_UPGRADES";

    public static Action<float> OnUpdatePlayerSpeed;
    public static Action<float> OnUpdatePlayerAir;

    private void Start()
    {
        airUpgadesBought = PlayerPrefs.GetInt(AIR_UPGRADES);
        speedUpgradesBought = PlayerPrefs.GetInt(SPEED_UPGRADES);

        UpdatePlayerStats();

        UpdateBars();
    }

    private void UpdatePlayerStats()
    {
        OnUpdatePlayerAir?.Invoke(airBuffs[airUpgadesBought]);
        OnUpdatePlayerSpeed?.Invoke(speedBuffs[speedUpgradesBought]);
    }

    private void Update()
    {
        moneyCountText.text = player.GetCoinCount().ToString();
    }

    private void UpdateBars()
    {
        while (airUpgadesBought > airUpgradeBar.childCount)
        {
            Instantiate(airUpgradePrefab, airUpgradeBar);
        }

        while (speedUpgradesBought > speedUpgradeBar.childCount)
        {
            Instantiate(speedUpgradePrefab, speedUpgradeBar);
        }

        airCostText.text = UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought].ToString();
        speedCostText.text = UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought].ToString();
        UpdatePlayerStats();
    }

    public void UpgradeAirClick()
    {
        if (airUpgadesBought < UPGRADE_COSTS.Length / 2)
        {
            if (player.GetCoinCount() >= UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought])
            {
                player.UpdateCoinCount(-UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought]);
                airUpgadesBought++;
                PlayerPrefs.SetInt(AIR_UPGRADES, airUpgadesBought);
                UpdateBars();
            }
        }
    }

    public void UpdateSpeedClick()
    {
        if (speedUpgradesBought < UPGRADE_COSTS.Length / 2)
        {
            if (player.GetCoinCount() >= UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought])
            {
                player.UpdateCoinCount(-UPGRADE_COSTS[airUpgadesBought + speedUpgradesBought]);
                speedUpgradesBought++;
                PlayerPrefs.SetInt(SPEED_UPGRADES, speedUpgradesBought);
                UpdateBars();
            }
        }
    }
}
