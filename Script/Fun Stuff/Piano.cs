using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour
{
    [SerializeField] private AudioClip sound;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        
    }

    private void OnMouseDown()
    {
        audioSource.PlayOneShot(sound);
    }
    public void PlayNoteWithController()
    {
        audioSource.PlayOneShot(sound);
    }
}
