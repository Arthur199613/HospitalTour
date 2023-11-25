using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class VRGameMenuManager : MonoBehaviour
{
    private static VRGameMenuManager instance;

    [SerializeField] private InputActionProperty showButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject stages;
    [SerializeField] private GameObject journal;

    [Header("Other stuff")]
    public bool GameIsPaused = false;

    private void Awake()
    {
        if (instance != null)
            {
                Debug.LogWarning("Found more than one Dialogue Manager in the scene");
            }
            instance = this;
    }
    public static VRGameMenuManager GetInstance()
    {
        return instance;
    }
    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }   
    }

    public void OpenPauseMeny()
    {
        mainMenu.SetActive(true);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(false);
        GameIsPaused = true;
    }
    public void OpenOptions()
    {
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(true);
        journal.SetActive(false);
    }
    public void OpenListOfStages()
    {
        mainMenu.SetActive(false);
        stages.SetActive(true);
        option.SetActive(false);
        journal.SetActive(false);
    }
    public void OpenJournal()
    {
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(true);
    }
    public void CloseMenu()
    {
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(false);
        GameIsPaused = false;
    }

    private void Resume()
    {
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(false);
        GameIsPaused = false;
    }
    private void Pause()
    {
        mainMenu.SetActive(true);
        GameIsPaused = true;
    }
}