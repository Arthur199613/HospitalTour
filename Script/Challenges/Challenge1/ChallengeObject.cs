using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class ChallengeObject : MonoBehaviour
{
    public Image questItem;
    public Image questItemVR;
    public Color completedColor;
    public Color activeColor;

    [SerializeField] public TextAsset info;

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            questItem.color = activeColor;
            questItemVR.color = activeColor;
        }
    }

    public void FinishQuest(GameObject gameObject) 
    {
        questItem.color = completedColor;
        questItemVR.color = completedColor;
        Challenge1Manager.instance.FinishQuest(gameObject);
    }
    public void ChangeColor(Color color)
    {
        questItem.color = color;
        questItemVR.color = color;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player") {
            gameObject.SetActive(false);
            FinishQuest(gameObject);
        }
    }
}
