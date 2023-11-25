using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [SerializeField] private AudioSource _musicSource, _effectsSource;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeMusic(AudioClip clip) 
    {
        _musicSource.Stop();
        _musicSource.clip = clip;
        StartCoroutine(ChangeMusicDelay());
    }
    private IEnumerator ChangeMusicDelay()
    {
        yield return new WaitForSeconds(0.5f);
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip) 
    {
        _effectsSource.PlayOneShot(clip);
    }

    public void ChangeMasterVolume(float value)
    {
        AudioListener.volume = value;
    }
    public void ChangeMusicVolume(float value)
    {
        _musicSource.volume = value;
    }    
    public void ChangeEffectVolume(float value)
    {
        _effectsSource.volume = value;
    }        
    public void StopMusic()
    {
        _musicSource.Stop();
    }
    public void ToggleEffects()
    {
        _effectsSource.mute = !_effectsSource.mute;
    }
    public void ToggleMusic()
    {
        _musicSource.mute = !_musicSource.mute;
    }
}
