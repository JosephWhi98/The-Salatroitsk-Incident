using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class GigerCounter : MonoBehaviour
{
    [Range(0, 1)] public float intensity;

    public AudioSource lowSource;
    public AudioSource midSource;
    public AudioSource highSource;


    public Transform[] radioactivePoints;
    public float distanceRange;
    public float minimumDistance; 

    public Animator animator;

    public GameObject displayParent;
    public TextMeshPro display;

    private bool on;

    public AudioSource onOffSource;
    public AudioClip onClip;
    public AudioClip offClip;

    public bool aiming;

    public float GetClosestTarget()
    {
        float closestDistance = float.MaxValue;

        foreach (Transform t in radioactivePoints)
        {

            if(t.gameObject.activeInHierarchy)
                if (Vector3.Distance(transform.position, t.position) < closestDistance)
                    closestDistance = Vector3.Distance(transform.position, t.position);
        }

        return closestDistance; 
    }

    public void Update()
    {
        if (on)
        {
            float closestTarget = GetClosestTarget();


            intensity = 1 - closestTarget / distanceRange;
            intensity = Mathf.Clamp(intensity, 0.05f, 1);

            display.text = Mathf.Round(intensity * 1000) + " CPM";
        }
        else
        {
            intensity = 0f;
        }

        float highVolume = (Mathf.Clamp(intensity, 0.5f, 1f) - 0.5f) / 0.5f;
        highSource.volume = intensity > 0 ? highVolume : 0;

        float lowVolume = 1 - (Mathf.Clamp(intensity, 0f, 0.5f) / 0.5f);
        lowSource.volume = intensity > 0 ? lowVolume : 0;

        float midVolume = 1 - highVolume - lowVolume;
        midSource.volume = intensity > 0 ? midVolume : 0;

        displayParent.SetActive(on);

        animator.SetBool("RMB", aiming);
    }

    public void TurnOn()
    {
        if (!on)
            onOffSource.PlayOneShot(onClip);

        on = true;

        GameManager.Instance.gigerCounterTutorialCompleted = true; 
    }

    public void TurnOff()
    {
        if (on)
            onOffSource.PlayOneShot(offClip);

        on = false;
    }
}
