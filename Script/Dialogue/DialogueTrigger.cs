using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    private bool challenge2Begin = false;
    private bool challenge4Begin = false;

    private bool isClicked;
    [SerializeField] private InputActionProperty showButton;
    private static DialogueTrigger instance;

    [Header("Visual Cue")]
    [SerializeField] private GameObject NPC;
    [SerializeField] private GameObject InteractWith;
    [SerializeField] private GameObject VRInteractWith;
    [SerializeField] private GameObject AndroidInteractWith;

    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;
    [SerializeField] private TextAsset defaultInkJSON;

    public bool playerInRange;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        playerInRange = false;
        // GameManager
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }
    public static DialogueTrigger GetInstance()
    {
        return instance;
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (state == GameState.Challenge2)
        {
            challenge2Begin = true;
        }
        if (state == GameState.Challenge4)
        {
            challenge4Begin = true;
        }
    }

    private void Update()
    {
        if (playerInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            InteractWith.SetActive(true);
            VRInteractWith.SetActive(true);
            AndroidInteractWith.SetActive(true);
            if (Input.GetKeyDown(KeyCode.E) || showButton.action.WasPressedThisFrame() || isClicked == true)
            {
                StartDialogue();
            }
        }
    }

    public void ButtonClicked()
    {   if (playerInRange)
            isClicked = true;
    }

    private void StartDialogue()
    {
        if (challenge2Begin == true && NPC.name == "Paul" || challenge4Begin == true && NPC.name == "Roald")
        {
            InteractWith.SetActive(false);
            VRInteractWith.SetActive(false);
            AndroidInteractWith.SetActive(false);
            DialogueManager.GetInstance().EnterDialogueMode(inkJSON);
            isClicked = false;
        }
        else
        {
            InteractWith.SetActive(false);
            AndroidInteractWith.SetActive(false);
            DialogueManager.GetInstance().EnterDialogueMode(defaultInkJSON);
            isClicked = false;
        }
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
