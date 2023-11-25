using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/*public static class ButtonExtension
{
    public static void AddEventListener<T>(this Button button, T param, Action<T> OnClick)
    {
        button.onClick.AddListener(delegate() {
            OnClick(param);
        });
    }
}*/
public class ManageScenes : MonoBehaviour
{

    [SerializeField] private GameObject LoadingScreen;
    [SerializeField] private Image LoadingBarFill;

    /*void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if(scene.enabled)
            {
                string stage = RegexThisPath(scene.path);

                if (stage != "HubWorld" && stage != "MainMenu")
                {
                    g = Instantiate(buttonTemplate, transform);
                    g.transform.GetChild(0).GetComponent<TMP_Text>().text = stage;
                    //g.GetComponent<Button>().AddEventListener(stage, ChooseStage);
                }
            }
        }

        Destroy(buttonTemplate);
    }*/
    // Exit the game
    public void Exit()
    {
        Application.Quit();   // The Game

        //UnityEditor.EditorApplication.isPlaying = false;    // In Editor
    }

    // Return to Hubworld
    public void BackToHubworld()
    {
        SceneManager.LoadScene("Hubworld");
        Time.timeScale = 1f;
    }
    // To Barneposten
    public void ToBarneposten()
    {
        StartCoroutine(LoadSceneAsync("Barneposten"));
        //SceneManager.LoadScene("Barneposten");
        //Time.timeScale = 1f;
    }
    // To Main Menu
    public void ToMainMenu()
    {
        StartCoroutine(LoadSceneAsync("MainMenu"));
        //SceneManager.LoadScene("Barneposten");
        //Time.timeScale = 1f;
    }

    // Choose the stage
    public void ChooseStage(string stage)
    {
        SceneManager.LoadScene(stage);
        Time.timeScale = 1f;
    }
    IEnumerator LoadSceneAsync(string sceneId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);
        LoadingScreen.SetActive(true);

        while (!operation.isDone)
        {
            Debug.Log(operation.progress);
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            LoadingBarFill.fillAmount = progressValue;
            yield return null;
        }
    }
    private string RegexThisPath(string path)
    {
        string pattern = @"(.*\/)(.+/|.[^.]*)";
        Match match = Regex.Match(path, pattern);
        string result = match.Groups[2].ToString();

        return result;
    }
}
