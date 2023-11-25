using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{

    [SerializeField] private Slider _slider;
    [SerializeField] private Slider _slider_music;
    [SerializeField] private Slider _slider_effect;

    // Start is called before the first frame update
    void Start()
    {
        //Master
        SoundManager.Instance.ChangeMasterVolume(_slider.value);
        _slider.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMasterVolume(val));

        //Music
        SoundManager.Instance.ChangeMusicVolume(_slider_music.value);
        _slider_music.onValueChanged.AddListener(val => SoundManager.Instance.ChangeMusicVolume(val));

        //Sound effect
        SoundManager.Instance.ChangeEffectVolume(_slider_effect.value);
        _slider_effect.onValueChanged.AddListener(val => SoundManager.Instance.ChangeEffectVolume(val));
    }
}
