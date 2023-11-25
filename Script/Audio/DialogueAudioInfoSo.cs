using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueAudioInfo", menuName = "ScriptableObjects/DialogueAudioInfoSo", order = 1)]
public class DialogueAudioInfoSo : ScriptableObject
{
    public string id;
    public AudioClip[] soundClip;
    [Range(1,5)]
    public int frequencyLevel = 1;
    public bool stopAudioSource;
    [Range(-3,3)]
    public float minPitch = 0.5f;
    [Range(-3,3)]
    public float maxPitch = 3f;
}
