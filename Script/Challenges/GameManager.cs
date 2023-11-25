using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameState state;
    public static event Action<GameState> OnGameStateChanged;

    [SerializeField] private InputActionProperty showButton;

    [Header("Journal elements")]
    [SerializeField] private TextMeshProUGUI ch_name;
    [SerializeField] private TextMeshProUGUI ch_description;
    [SerializeField] private GameObject challenge_notComplete;
    [SerializeField] private TextMeshProUGUI VRch_name;
    [SerializeField] private TextMeshProUGUI VRch_description;
    [SerializeField] private GameObject VRchallenge_notComplete;
    private bool challenge_complete;

    [Header("INK Files")]
    [SerializeField] private TextAsset intro;
    [SerializeField] private TextAsset ch1;
    [SerializeField] private TextAsset ch2;
    [SerializeField] private TextAsset ch3;
    [SerializeField] private TextAsset ch4;
    [SerializeField] private TextAsset ch5;
    [SerializeField] private TextAsset outro;

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Dialogue UI VR")]
    [SerializeField] private GameObject VRdialoguePanel;
    [SerializeField] private GameObject VRcontinueIcon;
    [SerializeField] private TextMeshProUGUI VRdialogueText;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSo defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSo[] audioInfos;
    private DialogueAudioInfoSo currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSo> audioInfoDictionary;
    private AudioSource audioSource;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set;}
    private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;
    private const string AUDIO_TAG = "audio";
    private const string CH1_TAG = "ch1";

    private void Awake()
    {
        instance = this;

        //Game elements for the journal 
        challenge_complete = false;
        challenge_notComplete.SetActive(true);
        VRchallenge_notComplete.SetActive(true);

        // Journal Description: "Hovering over the first challenge
        ch_name.text = "Teddybjørnene på Barneposten";
        ch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Barneposten. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";
        VRch_name.text = "Teddybjørnene på Barneposten";
        VRch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Barneposten. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";

        audioSource = this.gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
        InitializeAudioInfoDictionary();
    }
    private void Start()
    {
        UpdateGameState(GameState.Introduction);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
        Debug.Log(state);

        switch (newState)
        {
            case GameState.Introduction:
                // Plays the introduction
                StartCoroutine(EnterDialogueMode(intro));     
                break;
            case GameState.Challenge1:
                // Plays the intro for challenge 1
                StartCoroutine(EnterDialogueMode(ch1));
                break;
            case GameState.Challenge2:
                // Plays the intro for challenge 2
                StartCoroutine(EnterDialogueMode(ch2));
                break;
            case GameState.Challenge3:
                // Plays the intro for challenge 3
                StartCoroutine(EnterDialogueMode(ch3));
                break;
            case GameState.Challenge4:
                // Plays the intro for challenge 4
                StartCoroutine(EnterDialogueMode(ch4));
                break;
            case GameState.Challenge5:
                // Plays the intro for challenge 5
                StartCoroutine(EnterDialogueMode(ch5));
                break;
            case GameState.Finished:
                // Plays the outro
                StartCoroutine(EnterDialogueMode(outro));  
                challenge_notComplete.SetActive(false);
                VRchallenge_notComplete.SetActive(false);
                challenge_complete = true;                 
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }
    public void ChooseGameState(int i)
    {
        if (challenge_complete == true)
        {
            switch (i)
            {
                case 1:
                    UpdateGameState(GameState.Challenge1);
                    break;
                case 2:
                    UpdateGameState(GameState.Challenge2);
                    break;
                case 3:
                    UpdateGameState(GameState.Challenge3);
                    break;
                case 4:
                    UpdateGameState(GameState.Challenge4);
                    break;
                case 5:
                    UpdateGameState(GameState.Challenge5);
                    break;  
                default:
                    throw new ArgumentOutOfRangeException();
            }   
        }
    }
    public void ChallengeDescription(int i)
    {
        switch (i)
        {
            case 1:
                ch_name.text = "Teddybjørnene på Barneposten";
                ch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Barneposten. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";
                VRch_name.text = "Teddybjørnene på Barneposten";
                VRch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Barneposten. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";
                break;
            case 2:
                ch_name.text = "Dialog med Paul";
                ch_description.text = "Paul er leder på Barneposten. I denne oppgaven skal du ha en dialog med Paul og stille han noen spørsmål. Paul kan du finne ved Resepsjonen.";
                VRch_name.text = "Dialog med Paul";
                VRch_description.text = "Paul er leder på Barneposten. I denne oppgaven skal du ha en dialog med Paul og stille han noen spørsmål. Paul kan du finne ved Resepsjonen.";
                break;
            case 3:
                ch_name.text = "Hva er Kontroll Kommisjonen?";
                ch_description.text = "I denne utfordringen skal du få informasjon om Kontroll Kommisjonen. Ved resepsjonen er det en plakat man kan samhandle med. Gjør det og du vil få informasjonen.";
                VRch_name.text = "Hva er Kontroll Kommisjonen?";
                VRch_description.text = "I denne utfordringen skal du få informasjon om Kontroll Kommisjonen. Ved resepsjonen er det en plakat man kan samhandle med. Gjør det og du vil få informasjonen.";
                break;
            case 4:
                ch_name.text = "Dialog med Roald";
                ch_description.text = "Roald er leder på Sykehusskolen og har lyst til å ta en prat med deg. Prat med han og still han noen spørsmål om sykehusskolen.";
                VRch_name.text = "Dialog med Roald";
                VRch_description.text = "Roald er leder på Sykehusskolen og har lyst til å ta en prat med deg. Prat med han og still han noen spørsmål om sykehusskolen.";
                break;
            case 5:
                ch_name.text = "Teddybjørnene på Sykehusskolen";
                ch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Sykehusskolen. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";
                VRch_name.text = "Teddybjørnene på Sykehusskolen";
                VRch_description.text = "I denne utfordringen skal du finne teddybjørnene som er plassert på Sykehusskolen. Hver gang du finner en vil du få informasjon om rommet du fant teddybjørnen i.";
                break;  
            default:
                throw new ArgumentOutOfRangeException();
        }   
    }
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (XRSettings.loadedDeviceName != "OpenXR Display")
            dialoguePanel.SetActive(state == GameState.Introduction);
        else
            VRdialoguePanel.SetActive(state == GameState.Introduction);

    }
    private void InitializeAudioInfoDictionary()
    {
        audioInfoDictionary = new Dictionary<string, DialogueAudioInfoSo>();
        audioInfoDictionary.Add(defaultAudioInfo.id, defaultAudioInfo);
        foreach (DialogueAudioInfoSo audioInfo in audioInfos)
        {
            audioInfoDictionary.Add(audioInfo.id, audioInfo);
        }
    }
    private void SetCurrentAudioInfo(string id)
    {
        DialogueAudioInfoSo audioInfo = null;
        audioInfoDictionary.TryGetValue(id, out audioInfo);
        if (audioInfo != null)
        {
            this.currentAudioInfo = audioInfo;
        }
        else
        {
            Debug.LogWarning("Failed to find audio info for id: " + id);
        }
    }
    private void Update()
    {
        // return right away if dialogue isn't playing
        if (!dialogueIsPlaying)
        {
            return;
        }

        // handle continuing to the next line in the dialogue when submit is pressed
        if ((Input.GetKeyDown(KeyCode.Space) || showButton.action.WasPressedThisFrame() || Input.touchCount > 0)
            && canContinueToNextLine)
        {
            ContinueStory();
        }
    }
    public IEnumerator EnterDialogueMode(TextAsset inkJSON)
    {
        yield return new WaitForSeconds(1.5f);

        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        if (XRSettings.loadedDeviceName != "OpenXR Display")
            dialoguePanel.SetActive(true);
        else 
            VRdialoguePanel.SetActive(true);
        ContinueStory();
    }
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueIsPlaying = false;
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            dialoguePanel.SetActive(false);
            dialogueText.text = "";
        } else
        {
            VRdialoguePanel.SetActive(false);
            VRdialogueText.text = "";
        }

        // disable mouse
        Cursor.lockState = CursorLockMode.Locked;

        // go back to default audio
        SetCurrentAudioInfo(defaultAudioInfo.id);
    }
    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            string nextLine = currentStory.Continue();
            // Handle tags
            HandleTags(currentStory.currentTags);
            // set text for the current dialogue line
            displayLineCoroutine = StartCoroutine(DisplayLine(nextLine));
        }
        else 
        {
            StartCoroutine(ExitDialogueMode());
        }
    }
    private IEnumerator DisplayLine(string line)
    {
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            // set the text to the full line, but set the visible characters to 0
            dialogueText.text = line;
            dialogueText.maxVisibleCharacters = 0;
            // hide items while text is typing
            continueIcon.SetActive(false);

            canContinueToNextLine = false;
            bool isAddingRichTextTag = false;

            // Handle dialogue audio
            PlayDialogueSound();

            // display each letter one at a time
            foreach (char letter in line.ToCharArray())
            {
                // if the submit button is pressed, finish the line
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dialogueText.maxVisibleCharacters = line.Length;
                    break;
                }

                // check for rich text tag, if found, add it without waiting
                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    //dialogueText.text += letter;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                // if not rich text, add the next letter and wait a small time
                else
                {
                    dialogueText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            // actions to take after the entire line has finished displaying
            continueIcon.SetActive(true);
            canContinueToNextLine = true;
        } else
        {
            // set the text to the full line, but set the visible characters to 0
            VRdialogueText.text = line;
            VRdialogueText.maxVisibleCharacters = 0;
            // hide items while text is typing
            VRcontinueIcon.SetActive(false);

            canContinueToNextLine = false;
            bool isAddingRichTextTag = false;

            // Handle dialogue audio
            PlayDialogueSound();

            // display each letter one at a time
            foreach (char letter in line.ToCharArray())
            {
                // if the submit button is pressed, finish the line
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    VRdialogueText.maxVisibleCharacters = line.Length;
                    break;
                }

                // check for rich text tag, if found, add it without waiting
                if (letter == '<' || isAddingRichTextTag)
                {
                    isAddingRichTextTag = true;
                    //VRdialogueText.text += letter;
                    if (letter == '>')
                    {
                        isAddingRichTextTag = false;
                    }
                }
                // if not rich text, add the next letter and wait a small time
                else
                {
                    VRdialogueText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(typingSpeed);
                }
            }

            // actions to take after the entire line has finished displaying
            VRcontinueIcon.SetActive(true);
            canContinueToNextLine = true;
        }

    }
    private void PlayDialogueSound()
    {
        // set variables for the below based on our config
        AudioClip[] audioClip = currentAudioInfo.soundClip;

        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        AudioClip soundClip = audioClip[0];
        
        // Play Sound
        audioSource.PlayOneShot(soundClip);
        SetCurrentAudioInfo(defaultAudioInfo.id);

    }
    private void HandleTags(List<string> currentTags)
    {
        // Loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(":");
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be appropriatly parsed:" + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            switch (tagKey)
            {
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    break;
                case CH1_TAG:
                    UpdateGameState(GameState.Challenge1);
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }
}

public enum GameState
{
    Introduction,
    Challenge1, // Teddy bear challenge on Barneposten floor
    Challenge2, // Talking with the secction leader and ask questions
    Challenge3, // Look up your rights on Control Commision's border
    Challenge4, // Talking with the hospital school leader
    Challenge5, // Teddy bear challenge on Hospital School floor
    Finished

}