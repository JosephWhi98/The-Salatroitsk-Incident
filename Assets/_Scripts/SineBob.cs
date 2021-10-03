using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineBob : MonoBehaviour
{
    [Range(0, 1f)]
    [SerializeField]
    float verticalSwayAmount = 0.5f;

    [Range(0, 1f)]
    [SerializeField]
    float horiztonalSwayAmount = 1f;

    [Range(0, 15f)]
    [SerializeField]
    float swaySpeed = 4f;

    Vector2 lastInput;


    // Update is called once per frame
    private void Update()
    {
        Vector3 v = new Vector3(-PlayerController.Instance.lookInput.x, -PlayerController.Instance.lookInput.y, 0);

        lastInput = Vector3.Lerp(lastInput, v, Time.deltaTime);

        float x = lastInput.x * 0.003f, y = lastInput.y * 0.005f;
        y += verticalSwayAmount * Mathf.Sin((swaySpeed * 2) * Time.time);
        x += horiztonalSwayAmount * Mathf.Sin(swaySpeed * Time.time);
        transform.localPosition = new Vector3(x, y, transform.localPosition.z);
    }
}
