using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private Sprite[] pageSprites;
    [SerializeField] private GameObject[] indicators;

    private int currentPage;

    [SerializeField] private Image page;
    [SerializeField] private GameObject screen;

    public void ToggleScreen()
    {
        screen.SetActive(!screen.activeSelf);
        currentPage = 0;
        UpdatePage();
    }

    public void PageLeft()
    {
        currentPage = Mathf.Clamp(currentPage - 1, 0, pageSprites.Length - 1);
        UpdatePage();
    }

    public void PageRight()
    {
        currentPage = Mathf.Clamp(currentPage + 1, 0, pageSprites.Length - 1);
        UpdatePage();
    }

    private void UpdatePage()
    {
        page.sprite = pageSprites[currentPage];
        foreach (GameObject g in indicators)
        {
            g.SetActive(false);
        }
        indicators[currentPage].SetActive(true);
    }
}
