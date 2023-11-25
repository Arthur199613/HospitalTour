using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class Challenge1Manager : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    [Header("Teddy Bears Barneposten")]
    [SerializeField] private GameObject PanelForBarneposten;
    [SerializeField] private GameObject teddyBears_Barneposten;
    [SerializeField] private List<GameObject> Challenge1_Barneposten;
    private int challengeCountBarneposten;

    [Header("Teddy Bears Hospital School")]
    [SerializeField] private GameObject PanelForHospitalSchool;
    [SerializeField] private GameObject teddyBears_HospitalSchool;
    [SerializeField] private List<GameObject> Challenge5_HospitalSchool;    
    private int challengeCountHospitalSchool;

    [Header("VR Panel")]
    [SerializeField] private GameObject VRPanelForBarneposten;
    [SerializeField] private GameObject VRPanelForHospitalSchool;

    //IMPORTANT
    //This is the Teddy bear challenge for both Barneposten and Hospital School
    public static Challenge1Manager instance;

    private void Awake()
    {
        // GameManager
        GameManager.OnGameStateChanged += GameManagerOnOnGameStateChanged;
        
        instance = this;
    }
    private void OnDestroy()
    {
        GameManager.OnGameStateChanged -= GameManagerOnOnGameStateChanged;
    }
    private void GameManagerOnOnGameStateChanged(GameState state)
    {
        PanelForBarneposten.SetActive(state == GameState.Challenge1);
        teddyBears_Barneposten.SetActive(state == GameState.Challenge1);
        if (state == GameState.Challenge1)
        {
            challengeCountBarneposten = Challenge1_Barneposten.Count;
            foreach(GameObject teddy in Challenge1_Barneposten)
            {
                teddy.SetActive(true);
            }
        }

        PanelForHospitalSchool.SetActive(state == GameState.Challenge5);
        teddyBears_HospitalSchool.SetActive(state == GameState.Challenge5);
        if (state == GameState.Challenge5)
        {
            challengeCountHospitalSchool = Challenge5_HospitalSchool.Count;
            foreach(GameObject teddy in Challenge5_HospitalSchool)
            {
                teddy.SetActive(true);
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // How many teddy bears there are on each floor
        challengeCountBarneposten = -1;
        challengeCountHospitalSchool = -1;
        
        GameObject buttonTemplate = PanelForBarneposten.transform.GetChild(1).gameObject;
        GameObject g;
        GameObject buttonTemplateVR = VRPanelForBarneposten.transform.GetChild(1).gameObject;
        GameObject gVR;
        int i = 0;
        float iVR = 0.0f;

        foreach(GameObject teddy in Challenge1_Barneposten)
        {
            ChallengeObject script = teddy.GetComponent<ChallengeObject>();
            
            // Non VR
            g = Instantiate(buttonTemplate, PanelForBarneposten.transform);
            g.transform.GetChild(0).GetComponent<TMP_Text>().text = teddy.name;
            script.questItem = g.GetComponent<Image>();

            g.transform.position = new Vector3(g.transform.position.x, g.transform.position.y-i, g.transform.position.z);

            // VR
            gVR = Instantiate(buttonTemplateVR, VRPanelForBarneposten.transform);
            gVR.transform.GetChild(0).GetComponent<TMP_Text>().text = teddy.name;
            script.questItemVR = gVR.GetComponent<Image>();

            gVR.transform.position = new Vector3(gVR.transform.position.x, gVR.transform.position.y-iVR, gVR.transform.position.z);

            i -= -110;
            iVR -= -0.02f;
        }
        Destroy(buttonTemplate);
        Destroy(buttonTemplateVR);

        GameObject buttonTemplate2 = PanelForHospitalSchool.transform.GetChild(1).gameObject;
        GameObject h;
        GameObject buttonTemplate2VR = VRPanelForHospitalSchool.transform.GetChild(1).gameObject;
        GameObject hVR;
        int y = 0;
        float yVR = 0.0f;

        foreach(GameObject teddy2 in Challenge5_HospitalSchool)
        {
            ChallengeObject script = teddy2.GetComponent<ChallengeObject>();
            
            // Non VR
            h = Instantiate(buttonTemplate2, PanelForHospitalSchool.transform);
            h.transform.GetChild(0).GetComponent<TMP_Text>().text = teddy2.name;
            script.questItem = h.GetComponent<Image>();

            h.transform.position = new Vector3(h.transform.position.x, h.transform.position.y-y, h.transform.position.z);

            // VR
            hVR = Instantiate(buttonTemplate2VR, VRPanelForHospitalSchool.transform);
            hVR.transform.GetChild(0).GetComponent<TMP_Text>().text = teddy2.name;
            script.questItemVR = hVR.GetComponent<Image>();

            hVR.transform.position = new Vector3(hVR.transform.position.x, hVR.transform.position.y-yVR, hVR.transform.position.z);

            y -= -110;
            yVR -= -0.02f;
        }
        Destroy(buttonTemplate2);
        Destroy(buttonTemplate2VR);
    }

    public void FinishQuest(GameObject teddy) 
    {
        challengeCountBarneposten--;
        challengeCountHospitalSchool--;
        ChallengeObject script = teddy.GetComponent<ChallengeObject>();
        SoundManager.Instance.PlaySound(clip);
        StartCoroutine(GameManager.instance.EnterDialogueMode(script.info));
        StartCoroutine(CheckIfFinished());
    }

    private IEnumerator CheckIfFinished()
    {
        yield return new WaitForSeconds(8.0f);
        if (challengeCountBarneposten == 0)
        {
            GameManager.instance.UpdateGameState(GameState.Challenge2);
        }
        if (challengeCountHospitalSchool == 0)
        {
            GameManager.instance.UpdateGameState(GameState.Finished);
        }
    }
}
