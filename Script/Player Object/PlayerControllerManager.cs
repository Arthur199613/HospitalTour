using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PlayerControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject keyboardPlayer;
    [SerializeField] private GameObject vrPlayer;

    private void Start()
    {
        Debug.Log(XRSettings.loadedDeviceName);
        if (XRSettings.loadedDeviceName == "MockHMD Display")
        {
            // No XR device detected, use keyboard player
            keyboardPlayer.SetActive(true);
            vrPlayer.SetActive(false);
        }
        else
        {
            // XR device detected, use VR player
            keyboardPlayer.SetActive(false);
            vrPlayer.SetActive(true);
        }
    }
}
