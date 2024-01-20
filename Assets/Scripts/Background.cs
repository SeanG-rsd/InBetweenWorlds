using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private float startPos;
    private float startSize;

    private float lastFramePos;
    private void Start()
    {
        startPos = player.transform.position.x;
        startSize = spriteRenderer.size.x;
        transform.position = new Vector3(startPos, transform.position.y, transform.position.z);
        lastFramePos = startPos;
    }
    void Update()
    {
        float newPos = player.transform.position.x;

        spriteRenderer.size += new Vector2(newPos - lastFramePos, 0f);
        transform.Translate((newPos - lastFramePos) / 2, 0, 0);

        lastFramePos = newPos;
    }
}
