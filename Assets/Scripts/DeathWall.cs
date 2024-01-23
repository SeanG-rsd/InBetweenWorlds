using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathWall : MonoBehaviour
{
    [SerializeField] private float speed;

    [SerializeField] private SpriteRenderer front;
    [SerializeField] private SpriteRenderer back;
    [SerializeField] private float animateSpeed;
    [SerializeField] private Vector2 maxMinHeight;

    [SerializeField] private Vector3Int startPos;
    private float max;
    private int direction = 1;

    public int currentRoomNumber;

    private bool isPaused = false;

    [SerializeField] private int playerReviveBuffer;
    // Update is called once per frame

    private void Awake()
    {
        currentRoomNumber = 0;
        Player.OnCompletedRoom += HandleRoomCompletion;
        GameManager.OnPause += HandlePause;
        RewardedAd.OnRevivePlayer += HandleRevivePlayer;
        Player.OnDeath += HandleDeath;
        GameManager.OnStartGame += HandleStartGame;
        max = maxMinHeight.y;
    }

    private void OnDestroy()
    {
        Player.OnCompletedRoom -= HandleRoomCompletion;
        GameManager.OnPause -= HandlePause;
        RewardedAd.OnRevivePlayer -= HandleRevivePlayer;
        Player.OnDeath -= HandleDeath;
        GameManager.OnStartGame -= HandleStartGame;
    }

    
    private void Update()
    {
        if (!isPaused)
        {
            transform.Translate(speed * Time.deltaTime, 0, 0);

            front.size += new Vector2(0, direction * animateSpeed * Time.deltaTime);
            back.size += new Vector2(0, direction * animateSpeed * Time.deltaTime);

            if (front.size.y >= max && direction == 1)
            {
                direction *= -1;
                max = Random.Range(maxMinHeight.y - 3, maxMinHeight.y + 3);
            }
            else if (front.size.y <= maxMinHeight.x && direction == -1)
            {
                direction *= -1;
            }
        }
    }

    private void HandleRoomCompletion()
    {
        currentRoomNumber++;
        speed = Mathf.Log(currentRoomNumber + 1, 4);
    }

    private void HandleRevivePlayer()
    {
        transform.Translate(-playerReviveBuffer, 0, 0);
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

    private void HandlePause(bool pauseState)
    {
        isPaused = pauseState;
    }

    public void Initialize()
    {
        currentRoomNumber = 0;
        speed = 0;
        transform.position = startPos;
    }
}
