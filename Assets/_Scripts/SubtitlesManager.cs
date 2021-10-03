using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class SubtitlesManager : Singleton<SubtitlesManager>
{
    public AudioSource audiosource;
    public AudioClip subtitleClip;

    public string targetString;
    public string currentString;

    public TextMeshProUGUI textMesh;

    private Coroutine subtitleRoutine;


    public CanvasGroup screenfadeGroup;

    public Letterboxer.Letterboxer letterboxer;

    public GameObject staminaBarGameObejct; 
    public Image staminaFillbar; 


    public void Update()
    {
        textMesh.text = currentString;

        staminaBarGameObejct.SetActive(PlayerController.Instance.stamina < 1);
        staminaFillbar.fillAmount = PlayerController.Instance.stamina ;
    }

    public void ShowSubtitle(string target, float duration)
    {
        if (subtitleRoutine != null)
            StopCoroutine(subtitleRoutine);

        subtitleRoutine = StartCoroutine(SubtitleDisplayRoutine(target, duration)) ;
    }

    public IEnumerator SubtitleDisplayRoutine(string target, float duration)
    {
        targetString = target; 
        currentString = "";
        int i = 0;
        while (currentString.Length < targetString.Length)
        {
            currentString += targetString[i];
            i++;

            audiosource.PlayOneShot(subtitleClip);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(duration);

        currentString = "";

        yield return null;

        targetString = "";
    }

    public void OnDisable()
    {
        currentString = "";
        textMesh.text = currentString;

    }
}
