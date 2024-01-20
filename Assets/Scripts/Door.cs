using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject opposite;

    public void SetOpposite(GameObject opposite)
    {
        this.opposite = opposite;
    }

    public void DestroyPair()
    {
        Destroy(opposite);
        Destroy(this.gameObject);
    }
}
