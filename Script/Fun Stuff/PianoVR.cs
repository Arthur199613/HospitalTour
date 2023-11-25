using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PianoVR : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    public void PlayNote()
    {
        audioSource.PlayOneShot(sound);
    }
}
