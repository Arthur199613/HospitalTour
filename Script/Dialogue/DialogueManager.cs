using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;

    [SerializeField] private InputActionProperty showButton;
    [SerializeField] Image image;

    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.04f;

    [Header("Load Gloabals JSON")]
    [SerializeField] private TextAsset loadGlobalsJSON;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject continueIcon;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI displayNameText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choiceText;

    [Header("Dialogue UI VR")]
    [SerializeField] private GameObject VRdialoguePanel;
    [SerializeField] private GameObject VRcontinueIcon;
    [SerializeField] private TextMeshProUGUI VRdialogueText;
    [SerializeField] private TextMeshProUGUI VRdisplayNameText;

    [Header("Choices UI in VR")]
    [SerializeField] private GameObject[] VRchoices;
    private TextMeshProUGUI[] VRchoiceText;

    [Header("Audio")]
    [SerializeField] private DialogueAudioInfoSo defaultAudioInfo;
    [SerializeField] private DialogueAudioInfoSo[] audioInfos;
    //[SerializeField] private bool makePredictable;
    private DialogueAudioInfoSo currentAudioInfo;
    private Dictionary<string, DialogueAudioInfoSo> audioInfoDictionary;
    private AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip Barneposten_music;
    [SerializeField] private AudioClip HospitalSchool_music;
    [SerializeField] private AudioClip Dialogue_music;

    private Story currentStory;
    public bool dialogueIsPlaying { get; private set;}
    private bool canContinueToNextLine = false;
    private Coroutine displayLineCoroutine;

    private const string SPEAKER_TAG = "speaker";
    private const string PROTRAIT_TAG = "protrait";
    private const string LAYOUT_TAG = "layout";
    private const string AUDIO_TAG = "audio";
    private const string COMPLETE_TAG = "complete";
    private const string COMPLETE_TAG2 = "sykehus";

    private DialogueVariables dialogueVariables;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;
        TextAsset loadGlobalsJSON2 = loadGlobalsJSON;
        dialogueVariables = new DialogueVariables(loadGlobalsJSON2);

        audioSource = this.gameObject.AddComponent<AudioSource>();
        currentAudioInfo = defaultAudioInfo;
        // GameManager
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }
    public static DialogueManager GetInstance()
    {
        return instance;
    }
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (state == GameState.Challenge2)
        {
            TextAsset loadGlobalsJSON3 = loadGlobalsJSON;
            dialogueVariables = new DialogueVariables(loadGlobalsJSON3);
        }
        if (state == GameState.Challenge4)
        {
            TextAsset loadGlobalsJSON3 = loadGlobalsJSON;
            dialogueVariables = new DialogueVariables(loadGlobalsJSON3);
        }
    }
    private void Start()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        VRdialoguePanel.SetActive(false);

        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            // Get all of the choices text
            choiceText = new TextMeshProUGUI[choices.Length];
            int index = 0;
            foreach (GameObject choice in choices)
            {
                choiceText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
        } else {
            // Get all of the choices text
            VRchoiceText = new TextMeshProUGUI[VRchoices.Length];
            int index = 0;
            foreach (GameObject choice in VRchoices)
            {
                VRchoiceText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
                index++;
            }
        }

        InitializeAudioInfoDictionary();
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
            && currentStory.currentChoices.Count == 0
            && canContinueToNextLine)
        {
            ContinueStory();
        }
    }
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        //Change soundtrack to dialogue soundtrack
        SoundManager.Instance.ChangeMusic(Dialogue_music);

        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        { 
            dialoguePanel.SetActive(true);

            dialogueVariables.StartListening(currentStory);

            // Enable mouse
            Cursor.lockState = CursorLockMode.None;

            // Disable the cursor
            image.fillAmount = 0f;

            // Reset protrait and speaker
            displayNameText.text = "???";
        } else
        {
            VRdialoguePanel.SetActive(true);
            dialogueVariables.StartListening(currentStory);

            // Reset protrait and speaker
            VRdisplayNameText.text = "???";
        }

        ContinueStory();
    }
    private IEnumerator ExitDialogueMode()
    {
        yield return new WaitForSeconds(0.2f);

        dialogueVariables.StopListening(currentStory);

        dialogueIsPlaying = false;

        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            dialoguePanel.SetActive(false);
            dialogueText.text = "";

            // disable mouse
            Cursor.lockState = CursorLockMode.Locked;

            // Enables the cursor
            image.fillAmount = 1f;
        } else
        {
            VRdialoguePanel.SetActive(false);
            VRdialogueText.text = "";   
        }

        // Change back to normal soundtrack
        if (ElevatorManager.instance.floor == 1)
            SoundManager.Instance.ChangeMusic(Barneposten_music);
        else if (ElevatorManager.instance.floor == 2)
            SoundManager.Instance.ChangeMusic(HospitalSchool_music);

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
            HideChoices();

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
            // display choices, if any, for this dialogue line
            DisplayChoices();

            canContinueToNextLine = true;
        } else 
        {
            // set the text to the full line, but set the visible characters to 0
            VRdialogueText.text = line;
            VRdialogueText.maxVisibleCharacters = 0;
            // hide items while text is typing
            VRcontinueIcon.SetActive(false);
            HideChoices();

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
                    //dialogueText.text += letter;
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
            // display choices, if any, for this dialogue line
            DisplayChoices();

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
    private void HideChoices()
    {
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            foreach (GameObject choiceButton in choices)
            {
                choiceButton.SetActive(false);
            }
        } else {
            foreach (GameObject choiceButton in VRchoices)
            {
                choiceButton.SetActive(false);
            }
        }
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
                case SPEAKER_TAG:
                    if (XRSettings.loadedDeviceName != "OpenXR Display")
                        displayNameText.text = tagValue;
                    else
                        VRdisplayNameText.text = tagValue;
                    break;
                case PROTRAIT_TAG:
                    break;
                case LAYOUT_TAG:
                    break;
                case AUDIO_TAG:
                    SetCurrentAudioInfo(tagValue);
                    break;
                case COMPLETE_TAG:
                    CompletedTaskBarneposten();
                    break;
                case COMPLETE_TAG2:
                    CompletedTaskHospitalTour();
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }
    private void CompletedTaskBarneposten()
    {
        GameManager.instance.UpdateGameState(GameState.Challenge3);
    }
    private void CompletedTaskHospitalTour()
    {
        GameManager.instance.UpdateGameState(GameState.Challenge5);
    }
    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        // defensive check to make sure our UI can support of choices coming in
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            if (currentChoices.Count > choices.Length)
            {
                Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                    + currentChoices.Count);
            }
        } else {
            if (currentChoices.Count > VRchoices.Length)
            {
                Debug.LogError("More choices were given than the UI can support. Number of choices given: "
                    + currentChoices.Count);
            }
        }

        int index = 0;
        // enable and initialize the choices up to the amount of choices for this line of dialogue
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            foreach(Choice choice in currentChoices)
            {
                choices[index].gameObject.SetActive(true);
                choiceText[index].text = choice.text;
                index++;
            }
            // go through the remaining choices the UI supports and make sure they're hidden
            for (int i = index; i < choices.Length; i++)
            {
                choices[i].gameObject.SetActive(false);
            }

            //StartCoroutine(SelectFirstChoice());
        } else {
            foreach(Choice choice in currentChoices)
            {
                VRchoices[index].gameObject.SetActive(true);
                VRchoiceText[index].text = choice.text;
                index++;
            }
            // go through the remaining choices the UI supports and make sure they're hidden
            for (int i = index; i < VRchoices.Length; i++)
            {
                VRchoices[i].gameObject.SetActive(false);
            }

            //StartCoroutine(SelectFirstChoice());
        }
    }
    private IEnumerator SelectFirstChoice()
    {
        // Event system requires we clear it first, then wait
        // for at least one frame before we set current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        if (XRSettings.loadedDeviceName != "OpenXR Display")
        {
            EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
        } else {
            EventSystem.current.SetSelectedGameObject(VRchoices[0].gameObject);
        }
    }
    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            Input.GetKeyDown(KeyCode.Space);
            ContinueStory();
        }
    }
    public Ink.Runtime.Object GetVariableState(string variableName)
    {
        Ink.Runtime.Object variableValue = null;
        dialogueVariables.variables.TryGetValue(variableName, out variableValue);
        if (variableValue == null)
        {
            Debug.LogWarning("Ink variable was found to be null: " + variableName);
        }
        return variableValue;
    }
    public void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }
}
