using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events; 

public class MenuButtons : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    bool hovering;

    public UnityEvent clickEvent;
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnClick()
    {
        clickEvent.Invoke();
        Debug.Log(gameObject.name);
        audioSource.clip = clickClip;
        audioSource.Play();
    }


    public void MouseEnter()
    {
        hovering = true; 
        text.fontSize = 0.05f;
        audioSource.clip = hoverClip;
        audioSource.Play();
    }


    public void MouseExit()
    {
        Debug.Log("Exit");
        hovering = false;
        text.fontSize = 0.04f;
    }
}
