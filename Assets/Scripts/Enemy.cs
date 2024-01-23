using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour
{
    private int travelingDirection = 1;
    [SerializeField] private float speed;
    [SerializeField] private float maxDistanceInADirection;
    private float currentDistanceInADirection;

    private bool isPaused = false;
    Tilemap map;

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        map = FindFirstObjectByType<Tilemap>();
        GameManager.OnPause += HandlePause;
        RewardedAd.OnRevivePlayer += HandleRevivePlayer;
        Player.OnDeath += HandleDeath;
        GameManager.OnStartGame += HandleStartGame;
    }

    private void OnDestroy()
    {
        GameManager.OnPause -= HandlePause;
        RewardedAd.OnRevivePlayer -= HandleRevivePlayer;
        Player.OnDeath -= HandleDeath;
        GameManager.OnStartGame -= HandleStartGame;
    }

    private void Update()
    {
        if (!isPaused)
        {
            if (currentDistanceInADirection > maxDistanceInADirection || !IsPlatformBelowFront())
            {
                travelingDirection *= -1;
                currentDistanceInADirection = 0;
            }

            animator.SetFloat("Speed", currentDistanceInADirection / speed);
            spriteRenderer.flipX = travelingDirection > 0 ? false : true;

            transform.Translate(travelingDirection * speed * Time.deltaTime, 0, 0);
            currentDistanceInADirection += speed * Time.deltaTime;
        }
    }

    private void HandlePause(bool pauseState)
    {
        isPaused = pauseState;
    }

    private void HandleRevivePlayer()
    {
        isPaused = false;
    }

    private void HandleDeath()
    {
        isPaused = true;
    }

    private void HandleStartGame()
    {
        isPaused = false;
    }

    private bool IsPlatformBelowFront()
    {
        Vector3Int enemyPos = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        return map.GetTile(enemyPos) != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "DeathWall")
        {
            Destroy(gameObject);
        }
    }
}
