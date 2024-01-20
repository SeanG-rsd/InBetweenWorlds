using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera thisCam;

    private void Awake()
    {
        Player.OnSwapGravity += HandleGravitySwap;
    }

    private void OnDestroy()
    {
        Player.OnSwapGravity -= HandleGravitySwap;
    }

    private void HandleGravitySwap(int newGravity)
    {
        thisCam.m_Lens.Dutch = newGravity > 0 ? 0 : 180;
    }
}
