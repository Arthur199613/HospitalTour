using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    private static GameMenuManager instance;

    [SerializeField] private InputActionProperty showButton;

    [Header("Panels")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject option;
    [SerializeField] private GameObject stages;
    [SerializeField] private GameObject journal;
    [SerializeField] private Image image;

    [Header("Other stuff")]
    public bool GameIsPaused = false;

    private void Awake()
    {
        if (instance != null)
            {
                Debug.LogWarning("Found more than one Dialogue Manager in the scene");
            }
            instance = this;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public static GameMenuManager GetInstance()
    {
        return instance;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || showButton.action.WasPressedThisFrame())
        {
            if (GameIsPaused)
                Resume();
            else
                Pause();
        }   
    }

    public void OpenPauseMeny()
    {
        Cursor.lockState = CursorLockMode.Confined;
        image.fillAmount = 0f;
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
        Cursor.lockState = CursorLockMode.Locked;
        image.fillAmount = 1f;
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(false);
        GameIsPaused = false;
    }

    private void Resume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        image.fillAmount = 1f;
        mainMenu.SetActive(false);
        stages.SetActive(false);
        option.SetActive(false);
        journal.SetActive(false);
        GameIsPaused = false;
    }
    private void Pause()
    {
        Cursor.lockState = CursorLockMode.Confined;
        image.fillAmount = 0f;
        mainMenu.SetActive(true);
        GameIsPaused = true;
    }
}
