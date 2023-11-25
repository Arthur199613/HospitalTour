using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControllCommissionManager : MonoBehaviour
{
    [Header("Visual Cue")]
    [SerializeField] private GameObject poster;
    [SerializeField] private GameObject InteractWith;
    [SerializeField] private GameObject VRInteractWith;
    [SerializeField] private GameObject AndroidInteractWith;

    [SerializeField] private AudioClip clip;
    [SerializeField] private InputActionProperty showButton;
    private bool ch3_active;
    public bool playerInRange;
    private bool isPlaying;

    [SerializeField] private Button button;
    private bool isClicked;

    // Start is called before the first frame update
    private void Awake()
    {
        // GameManager
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
        
        ch3_active = false;
        isPlaying = false;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (state == GameState.Challenge3)
            ch3_active = true;
        else
            ch3_active = false;
    }

    private void Update()
    {
        if (playerInRange)
        {
            InteractWith.SetActive(true);
            VRInteractWith.SetActive(true);
            AndroidInteractWith.SetActive(true);
            if ((Input.GetKeyDown(KeyCode.E) || showButton.action.WasPressedThisFrame() || isClicked == true) && isPlaying == false)
            {
                StartCoroutine(StartAudioClip());
            }
        }
    }

    public void ButtonClicked()
    {
        if (playerInRange && isPlaying == false)
            isClicked = true;
    }

    private IEnumerator StartAudioClip()
    {
        isClicked = false;
        isPlaying = true;
        SoundManager.Instance.PlaySound(clip);

        if (ch3_active == true)
        {
            yield return new WaitForSeconds(85f);
            GameManager.instance.UpdateGameState(GameState.Challenge4);
        }

        isPlaying = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            playerInRange = false;
            InteractWith.SetActive(false);
            VRInteractWith.SetActive(false);
            AndroidInteractWith.SetActive(false);
        }
    }
}
