using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swapper : MonoBehaviour
{
    [SerializeField] private GameObject opposite;
    [SerializeField] private AudioClip source;

    public GameObject GetDestinationPosition()
    {
        SoundManager.Instance.PlaySound(source);
        return opposite;
    }

    public void SetDestinationSwapper(GameObject opposite)
    {
        this.opposite = opposite;
    }
}
