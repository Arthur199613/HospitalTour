using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorScript : MonoBehaviour
{
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
