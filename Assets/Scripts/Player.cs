using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [Header("---Movement---")]
    [SerializeField] private float baseSpeed;
    private float speed;
    public bool gameHasStarted;
    private bool isPaused = false;
    private float horizontal;
    [SerializeField] private Vector3 startPos;

    [SerializeField] private Rigidbody2D rb;
    private bool isGrounded = false;
    [SerializeField] private GameObject cameraFollowObject;
    private MapGenerator.World currentWorld;
    private GameObject currentSwapperTouching = null;

    [SerializeField] private PlayerInput playerInput;

    [Header("---Item---")]
    [SerializeField] private GameObject cargo;
    private bool isTouchingItem;
    private GameObject itemTouching = null;
    private GameObject itemInHand = null;
    private bool hasItemInHand;
    [SerializeField] private GameObject itemFollowObject;
    private float itemFollowObjectOriginalX;


    public static Action<int> OnSwapGravity = delegate { };
    public static Action OnCompletedRoom = delegate { };
    public static Action OnDeath = delegate { };

    [Header("---Animation---")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("---Air---")]
    [SerializeField] private float baseMaxAir;
    private float maxAir;
    private float currentAir;
    [SerializeField] private float airDepletionRate;
    [SerializeField] private float airGainRate;

    [SerializeField] private RectTransform airMask;
    private float originalSize;

    [Header("---Coins---")]
    [SerializeField] private TMP_Text gameCoinCountText;
    private int coinCount;
    private string COIN_KEY = "COIN_KEY";

    [Header("---Sound---")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip killEnemySound;
    [SerializeField] private AudioClip hitByEnemySound;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip airLossSound;
    [SerializeField] private AudioClip coinPickupSound;
    // Start is called before the first frame update

    private void Awake()
    {
        GameManager.OnStartGame += HandleStartGame;
        GameManager.OnPause += HandlePause;
        Cargo.OnGameOver += Death;
        ShopManager.OnUpdatePlayerAir += HandleAirUpgrade;
        ShopManager.OnUpdatePlayerSpeed += HandleSpeedUpgrade;
    }

    private void OnDestroy()
    {
        GameManager.OnStartGame -= HandleStartGame;
        GameManager.OnPause -= HandlePause;
        Cargo.OnGameOver -= Death;
        ShopManager.OnUpdatePlayerAir -= HandleAirUpgrade;
        ShopManager.OnUpdatePlayerSpeed -= HandleSpeedUpgrade;
    }


    void Start()
    {
        currentWorld = MapGenerator.World.Earth;
        maxAir = baseMaxAir;
        speed = baseSpeed;
        currentAir = maxAir;
        itemFollowObjectOriginalX = itemFollowObject.transform.localPosition.x;
        gameHasStarted = false;
        originalSize = airMask.rect.width;

        if (!PlayerPrefs.HasKey(COIN_KEY))
        {
            PlayerPrefs.SetInt(COIN_KEY, 0);
        }
        else
        {
            coinCount = PlayerPrefs.GetInt(COIN_KEY);
        }

        UpdateCoins();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontal = playerInput.actions["Move"].ReadValue<Vector2>().x;

        animator.SetFloat("Speed", new Vector2(horizontal, 0).magnitude);

        if (gameHasStarted && !isPaused)
        {
            transform.Translate(horizontal * speed * Time.deltaTime, 0, 0);
            cameraFollowObject.transform.position = new Vector3(transform.position.x, 0, 0);

            if (horizontal > 0)
            {
                spriteRenderer.flipX = false;
                if (itemFollowObject.transform.localPosition.x != itemFollowObjectOriginalX)
                {
                    itemFollowObject.transform.localPosition = new Vector3(itemFollowObjectOriginalX, itemFollowObject.transform.localPosition.y, 0);
                }
            }
            else if (horizontal < 0)
            {
                spriteRenderer.flipX = true;
                if (itemFollowObject.transform.localPosition.x != -itemFollowObjectOriginalX)
                {
                    itemFollowObject.transform.localPosition = new Vector3(-itemFollowObjectOriginalX, itemFollowObject.transform.localPosition.y, 0);
                }
            }

            UpdateAir();

            UpdateCoins();

            Jump(false);

            CheckForInteraction(false);

            if (hasItemInHand)
            {
                itemInHand.transform.position = itemFollowObject.transform.position;
            }
        }
    }

    private void UpdateAir()
    {
        if (currentWorld == MapGenerator.World.Earth)
        {
            currentAir = Mathf.Clamp(currentAir + (Time.deltaTime * airGainRate), 0, maxAir);
            SoundManager.Instance.AirSound(false);
        }
        else if (currentWorld == MapGenerator.World.Moon)
        {
            currentAir = Mathf.Clamp(currentAir - (Time.deltaTime * airDepletionRate), 0, maxAir);
            SoundManager.Instance.AirSound(true);
        }

        float value = currentAir / maxAir;
        airMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, originalSize * value);

        if (currentAir == 0)
        {
            Death();
        }
    }

    private void UpdateCoins()
    {
        gameCoinCountText.text = coinCount.ToString();
        PlayerPrefs.SetInt(COIN_KEY, coinCount);
    }

    public void JumpPress()
    {
        Jump(true);
    }

    public void InteractionPress()
    {
        CheckForInteraction(true);
    }

    private void CheckForInteraction(bool press)
    {
        if ((Input.GetKeyDown(KeyCode.E) || press) && currentSwapperTouching != null)
        {
            GameObject destination = currentSwapperTouching.GetComponent<Swapper>().GetDestinationPosition();
            transform.position = destination.transform.position;
            rb.gravityScale = rb.gravityScale * -1;
            OnSwapGravity?.Invoke((int)rb.gravityScale);
            transform.Rotate(0, 0, 180);

            if (currentWorld == MapGenerator.World.Earth)
            {
                currentWorld = MapGenerator.World.Moon;
            }
            else if (currentWorld == MapGenerator.World.Moon)
            {
                currentWorld = MapGenerator.World.Earth;
            }

            if (hasItemInHand)
            {
                itemTouching.transform.Rotate(0, 0, 180);
            }
        }
        else if ((Input.GetKeyDown(KeyCode.E) || press) && (isTouchingItem || hasItemInHand))
        {
            SoundManager.Instance.PlaySound(pickupSound);
            hasItemInHand = !hasItemInHand;
            if (!hasItemInHand)
            {
                itemInHand = null;
                animator.SetBool("Item", false);
                animator.SetBool("ItemDrop", true);
            }
            else
            {
                itemInHand = itemTouching;
                animator.SetBool("ItemDrop", false);
                animator.SetBool("Item", true);
            }
        }
    }

    private void Jump(bool press)
    {
        if ((Input.GetKeyDown(KeyCode.Space) || press) && isGrounded)
        {
            SoundManager.Instance.PlaySound(jumpSound);
            rb.AddForce(new Vector2(0, 400 * rb.gravityScale));
            animator.SetBool("Ground", false);
            animator.SetBool("Jump", true);
        }
    }
    private void HandleStartGame()
    {
        gameHasStarted = true;
        transform.position = startPos;
    }

    private void HandlePause(bool pauseState)
    {
        isPaused = pauseState;
    }

    private void HandleAirUpgrade(float airUpgrade)
    {
        maxAir = baseMaxAir * airUpgrade;
    }

    private void HandleSpeedUpgrade(float speedUpgrade)
    {
        speed = baseSpeed * speedUpgrade;
    }

    private void Death()
    {
        Debug.Log("death");
        if (gameHasStarted)
        {
            if (rb.gravityScale == -1)
            {
                rb.gravityScale = 1;
                transform.Rotate(0, 0, 180);
                OnSwapGravity?.Invoke((int)rb.gravityScale);
                currentWorld = MapGenerator.World.Earth;
            }

            currentAir = maxAir;
            transform.position = startPos;
            SoundManager.Instance.PlaySound(deathSound);
            OnDeath?.Invoke();
            gameHasStarted = false;
            itemInHand = null;
            itemTouching = null;
            hasItemInHand = false;
            PlayerPrefs.SetInt(COIN_KEY, coinCount);
            SoundManager.Instance.AirSound(false);
        }
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    public void UpdateCoinCount(int value)
    {
        coinCount += value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Swapper")
        {
            currentSwapperTouching = collision.gameObject;
            //Debug.Log("swapper enter");
        }
        else if (collision.gameObject.tag == "Key")
        {
            isTouchingItem = true;
            itemTouching = collision.gameObject;
        }
        else if (collision.gameObject.tag == "Cargo")
        {
            isTouchingItem = true;
            itemTouching = collision.gameObject;
        }
        else if (collision.gameObject.tag.CompareTo("Coin") == 0)
        {
            coinCount++;
            SoundManager.Instance.PlaySound(coinPickupSound);
            Destroy(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Swapper")
        {
            currentSwapperTouching = null;
        }
        else if (collision.gameObject.tag == "Cargo")
        {
            isTouchingItem = false;
        }
        else if (collision.gameObject.tag == "Key")
        {
            isTouchingItem = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = true;
            animator.SetBool("Jump", false);
            animator.SetBool("Ground", true);
        }
        else if (collision.gameObject.tag == "Door" && itemInHand != null)
        {
            if (itemInHand.tag == "Key" && hasItemInHand)
            {
                SoundManager.Instance.PlaySound(doorOpenSound);
                OnCompletedRoom?.Invoke();
                collision.gameObject.GetComponent<Door>().DestroyPair();
                Destroy(itemInHand);
                itemInHand = null;
                hasItemInHand = false;
                animator.SetBool("Item", false);
                animator.SetBool("ItemDrop", true);
            }
        }
        else if (collision.gameObject.tag == "DeathWall")
        {
            Death();
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            //Debug.Log("hit enemy");
            for (int i = -2; i < 2; ++i)
            {
                Vector2 pos = transform.position;
                pos.x += (i * 0.1f);
                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, 2f, LayerMask.GetMask("Enemy"));

                if (hit.collider != null && hit.transform.tag == "Enemy")
                {
                    Destroy(hit.collider.gameObject);
                    SoundManager.Instance.PlaySound(killEnemySound);
                    return;
                }
            }

            SoundManager.Instance.PlaySound(hitByEnemySound);
            currentAir /= 2;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            isGrounded = false;
        }
    }
}
