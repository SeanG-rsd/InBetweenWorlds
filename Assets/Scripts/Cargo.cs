using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cargo : MonoBehaviour
{
    public static Action OnGameOver = delegate { };

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.tag == "DeathWall")
        {
            Debug.Log("cargo loss");
            OnGameOver?.Invoke();
        }
    }
}
