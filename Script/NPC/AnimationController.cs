using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    // Reference to the Animation component
    private Animation animation;

    void Start()
    {
        // Get the Animation component
        animation = GetComponent<Animation>();

        if (animation.GetClip("Idle_P") != null)
        {

            // Set the wrap mode of the animation clip to Loop
            animation["Idle_P"].wrapMode = WrapMode.Loop;

            // Play the animation clip
            animation.Play("Idle_P");
        }
        else if (animation.GetClip("Idle_R") != null)
        {
            animation["Idle_R"].wrapMode = WrapMode.Loop;
            animation.Play("Idle_R");
        }
    }
}
