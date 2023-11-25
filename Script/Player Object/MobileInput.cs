using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileInput : MonoBehaviour
{
    public bool cameraMoveEnabled = true;
    
    void Update () {
        if (Input.touchCount > 0) {
            Touch touch = Input.GetTouch(0);
            if (touch.position.x > Screen.width / 2) {
                cameraMoveEnabled = true;
            } else {
                cameraMoveEnabled = false;
            }
        }
    }
}
