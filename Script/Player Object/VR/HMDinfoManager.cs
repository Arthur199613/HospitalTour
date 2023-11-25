using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HMDinfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Is Device Active: " + XRSettings.isDeviceActive);
        Debug.Log("Device Name is: " + XRSettings.loadedDeviceName);

        if(!XRSettings.isDeviceActive)
        {
            Debug.Log("No Headset plugged");
        }
        else if (XRSettings.isDeviceActive && (XRSettings.loadedDeviceName == "MockHMD Display"
            || XRSettings.loadedDeviceName == "Mock HMD"))
        {
            Debug.Log("Using Mock HMD");
        }
        else
        {
            Debug.Log("We have a headset: " + XRSettings.loadedDeviceName);
        }
    }
}
