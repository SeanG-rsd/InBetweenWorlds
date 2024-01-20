using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppPurchaseManager : MonoBehaviour
{
    [SerializeField] private GameObject noAdsButton;
    private static string PURCHASE_KEY = "BOUGHT_ADS";

    private void Start()
    {
        if (PlayerPrefs.GetInt(PURCHASE_KEY) == 1)
        {
            noAdsButton.SetActive(false);
        }


    }
}
