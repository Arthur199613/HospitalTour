using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject MainMenu;
    [SerializeField] private GameObject Settings;

    // Start
    private void Start()
    {
        MainMenu.SetActive(true);
        Settings.SetActive(false);
    }

    public void OpenSettings()
    {
        MainMenu.SetActive(false);
        Settings.SetActive(true);
    }
    public void CloseSettings()
    {
        MainMenu.SetActive(true);
        Settings.SetActive(false);
    }
}
