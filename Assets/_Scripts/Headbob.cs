using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Headbob : MonoBehaviour
{

    public Vector3 restPosition; //local position where your camera would rest when it's not bobbing.
    public float transitionSpeed = 20f; //smooths out the transition from moving to not moving.
    public float bobSpeed = 4.8f; //how quickly the player's head bobs.
    public float bobAmount = 0.05f; //how dramatic the bob is. Increasing this in conjunction with bobSpeed gives a nice effect for sprinting.

    float timer = Mathf.PI / 2; //initialized as this value because this is where sin = 1. So, this will make the camera always start at the crest of the sin wave, simulating someone picking up their foot and starting to walk--you experience a bob upwards when you start walking as your foot pushes off the ground, the left and right bobs come as you walk.
    Vector3 camPos;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip footstepClip;
    bool played = false;

    [SerializeField] PlayerController playerController;

    void Awake()
    {
        camPos = transform.localPosition;
        restPosition = transform.localPosition;

        StartCoroutine(FootstepRoutine());
    }


    void Update()
    {
        if (playerController.isRunning)
        {
            bobSpeed = 7f;
            bobAmount = 0.3f;
        }
        else
        {
            bobSpeed = 4.8f;
            bobAmount = 0.2f;
        }
            //if (controller.controller.isGrounded)
            //{
            if (PlayerController.Instance.moveInput.x != 0 || PlayerController.Instance.moveInput.y != 0) //moving
            {
                timer += bobSpeed * Time.deltaTime;

                //use the timer value to set the position
                Vector3 newPosition = new Vector3(Mathf.Cos(timer) * bobAmount, restPosition.y + Mathf.Abs((Mathf.Sin(timer) * bobAmount)), restPosition.z); //abs val of y for a parabolic path
                camPos = newPosition;
                transform.localPosition = camPos;
 
            }
            else
            {
                timer = Mathf.PI / 2; //reinitialize

                Vector3 newPosition = new Vector3(Mathf.Lerp(camPos.x, restPosition.x, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.y, restPosition.y, transitionSpeed * Time.deltaTime), Mathf.Lerp(camPos.z, restPosition.z, transitionSpeed * Time.deltaTime)); //transition smoothly from walking to stopping.
                camPos = newPosition;
            }

            if (timer > Mathf.PI * 2) //completed a full cycle on the unit circle. Reset to 0 to avoid bloated values.
                timer = 0;

       // }
    }

    IEnumerator FootstepRoutine()
    {
        while (true)
        {
            if (playerController.canMove)
            {
                if (PlayerController.Instance.moveInput.x != 0 || PlayerController.Instance.moveInput.y != 0) //moving
                {
                    audioSource.volume = playerController.isRunning ?.8f : 0.6f;
                    audioSource.pitch = Random.Range(0.8f, 1.2f);
                    audioSource.PlayOneShot(footstepClip);

                    yield return new WaitForSeconds(.8f); //+ 0.5f);
                }
            }
            yield return null;
        }
    }
}
