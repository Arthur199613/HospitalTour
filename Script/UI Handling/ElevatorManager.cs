using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class ElevatorManager : MonoBehaviour
{
    public static ElevatorManager instance;
    public int floor = 1;

    [Header("Elevator")]
    [SerializeField] private GameObject elevator_camera;
    [SerializeField] private GameObject[] images;

    [Header("Music & Sound")]
    [SerializeField] private AudioClip Barneposten_clip;
    [SerializeField] private AudioClip HospitalSchool_clip;
    [SerializeField] private AudioClip Barneposten_music;
    [SerializeField] private AudioClip HospitalSchool_music;

    // if (XRSettings.loadedDeviceName == "MockHMD Display")
    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }
        instance = this;

        elevator_camera.SetActive(false);
        
    }
    public static ElevatorManager GetInstance()
    {
        return instance;
    }

    private void Transistion(AudioClip clip, AudioClip music)
    {
        GameObject player = GameObject.FindWithTag("Player");
        player.SetActive(false);

        elevator_camera.SetActive(true);
        
        // Choose a random image
        int index = Random.Range(0, images.Length);
        images[index].SetActive(true);

        // Toggle music and play sound clip
        SoundManager.Instance.StopMusic();
        SoundManager.Instance.PlaySound(clip);

        StartCoroutine(Transistion2(player, music, images[index]));
    }

    private IEnumerator Transistion2(GameObject player, AudioClip music, GameObject image)
    {
        yield return new WaitForSeconds(3f);
        SoundManager.Instance.ChangeMusic(music);
        elevator_camera.SetActive(false);
        image.SetActive(false);
        player.SetActive(true);
    }

    public void TeleportToFloor(int floorNumber)
    {
        // Find the game object for the desired floor
        GameObject floorObject = GameObject.Find("Floor" + floorNumber);

        // Check if the floor object was found
        if (floorObject != null)
        {
            // Get the position of the floor object
            Vector3 floorPosition = floorObject.transform.position;

            // Set the player's position to the position of the floor
            GameObject player = GameObject.FindWithTag("Player");
            // Reference to the character controller component
            CharacterController characterController = player.GetComponent<CharacterController>();

            // Disable character controller movement
            characterController.enabled = false;

            // Create a transistion before teleporting
            if (floorNumber == 1)
            {
                floor = 1;
                Transistion(Barneposten_clip, Barneposten_music);
            }
            if (floorNumber == 2)
            {
                floor = 2;
                Transistion(HospitalSchool_clip, HospitalSchool_music);
            }

            // Teleport the player
            player.transform.position = new Vector3(floorPosition.x, floorPosition.y, floorPosition.z);

            // Reset the character's velocity to zero after the teleport
            characterController.enabled = true;
            characterController.Move(Vector3.zero - characterController.velocity);
        }
    }
}
