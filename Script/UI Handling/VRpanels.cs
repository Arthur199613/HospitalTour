using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;

public class VRpanels : MonoBehaviour
{
    [SerializeField] private InputActionProperty TB_Button;

    [Header("Panels")]
    [SerializeField] GameObject TB_Barneposten_Panel;
    [SerializeField] GameObject TB_HospitalSchool_Panel;

    private string _state = "";

    private void Awake()
    {
        // GameManager
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }

    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        if (state == GameState.Challenge1)
            _state = "Barneposten";
        if (state == GameState.Challenge5)
            _state = "HospitalSchool";
    }

    private void Update()
    {
        // Check if a dialogue is playing, game is paused
        if (DialogueManager.GetInstance().dialogueIsPlaying || VRGameMenuManager.GetInstance().GameIsPaused)
            return;
        

        if (TB_Button.action.WasPressedThisFrame())
        {
            Debug.Log("Click");
            OpenTeddyBearChallenge();
        }
    }

    //Get a list of all scenes
    private void OpenTeddyBearChallenge()
    {
        // .activeSelf
        if (_state == "Barneposten")
            TB_Barneposten_Panel.SetActive(!TB_Barneposten_Panel.activeSelf);
        if (_state == "HospitalSchool")
            TB_HospitalSchool_Panel.SetActive(!TB_HospitalSchool_Panel.activeSelf);

    }
    
}
