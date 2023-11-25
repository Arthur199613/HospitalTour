using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class StopXRMovement : MonoBehaviour
{
    private Rigidbody xrRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        xrRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if a dialogue is playing, game is paused
        if (DialogueManager.GetInstance().dialogueIsPlaying || GameMenuManager.GetInstance().GameIsPaused)
            xrRigidbody.isKinematic = true;
    }
}
