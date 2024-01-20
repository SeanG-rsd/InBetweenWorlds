using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class IconHandler : MonoBehaviour
{
    private rect earth;
    private rect moon;

    public struct rect
    {
        public Vector2 min;
        public Vector2 max;
        public Vector2 pivot;
        public Vector2 position;
    }

    [SerializeField] private RectTransform cargoIcon;
    [SerializeField] private TMP_Text cargoText;
    [SerializeField] private RectTransform deathWallIcon;
    [SerializeField] private TMP_Text deathWallText;

    private bool bothActive;

    [SerializeField] private GameObject cargo;
    [SerializeField] private GameObject deathWall;

    [SerializeField] private GameObject player;
    [SerializeField] private Tilemap tilemap;

    [SerializeField] private int buffer;

    private int grav = 1;

    private void Awake()
    {
        Debug.Log(Screen.width);
        buffer = (int)(Screen.width / 118);

        earth.min = new Vector2(0, 0.5f);
        earth.max = new Vector2(0, 0.5f);
        earth.pivot = new Vector2(0, 0.5f);
        earth.position = new Vector2(0, 0);

        moon.min = new Vector2(1, 0.5f);
        moon.max = new Vector2(1, 0.5f);
        moon.pivot = new Vector2(1, 0.5f);
        moon.position = new Vector2(-00, 0);

        Player.OnSwapGravity += HandleGravitySwap;
    }

    private void OnDestroy()
    {
        Player.OnSwapGravity -= HandleGravitySwap;
    }

    private void Update()
    {
        int playerX = tilemap.WorldToCell(player.transform.position).x;
        int cargoX = tilemap.WorldToCell(cargo.transform.position).x;
        int deathWallX = tilemap.WorldToCell(deathWall.transform.position).x;

        if (Mathf.Abs(playerX - cargoX) > buffer)
        {
            if (!cargoIcon.gameObject.activeSelf)
            {
                SwapSides((playerX - cargoX) * grav, cargoIcon, playerX - cargoX > 0 && grav > 0);
            }

            cargoIcon.gameObject.SetActive(true);
            cargoText.text = Mathf.Abs(playerX - cargoX).ToString() + " m";
        }
        else
        {
            cargoIcon.gameObject.SetActive(false);
        }

        if (playerX - deathWallX > buffer)
        {
            if (!deathWallIcon.gameObject.activeSelf)
            {
                SwapSides((playerX - deathWallX) * grav, deathWallIcon, playerX - cargoX > 0 && grav > 0);
            }

            deathWallIcon.gameObject.SetActive(true);
            deathWallText.text = Mathf.Abs(playerX - deathWallX).ToString() + " m";
        }
        else
        {
            deathWallIcon.gameObject.SetActive(false);
        }

        if (deathWallIcon.gameObject.activeSelf && cargoIcon.gameObject.activeSelf && !bothActive)
        {
            cargoIcon.transform.localPosition += new Vector3(0, 30, 0);
            deathWallIcon.transform.localPosition -= new Vector3(0, 30, 0);
            bothActive = true;
        }
        else if (bothActive && !(deathWallIcon.gameObject.activeSelf && cargoIcon.gameObject.activeSelf))
        {
            bothActive = false;
            cargoIcon.transform.localPosition -= new Vector3(0, 30, 0);
            deathWallIcon.transform.localPosition += new Vector3(0, 30, 0);

        }
    }

    private void HandleGravitySwap(int newGrav)
    {
        bool sameGrav = false;
        if (grav > 0 && newGrav > 0)
        {
            sameGrav = true;
        }

        grav = newGrav;
        SwapSides(newGrav, cargoIcon, sameGrav);
        SwapSides(newGrav, deathWallIcon, sameGrav);   
    }

    private void SwapSides(int side, RectTransform icon, bool sameGrav)
    {
        rect newRect = side > 0 ? earth : moon;
        icon.anchorMin = newRect.min;
        icon.anchorMax = newRect.max;
        icon.pivot = newRect.pivot;
        if (!sameGrav)
        {
            icon.transform.localPosition += new Vector3(-35 * side > 0 ? -1 : 1, 0, 0); 
            Debug.Log($"same grav for {icon.gameObject.name}");
        }
    }
}
