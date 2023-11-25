using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleAudio : MonoBehaviour
{
    [SerializeField] private bool _toogleMusic, _toggleEffects;

    public void Toggle() 
    {
        if (_toggleEffects)
            SoundManager.Instance.ToggleEffects();

        if (_toogleMusic)
            SoundManager.Instance.ToggleMusic();
    }
}
