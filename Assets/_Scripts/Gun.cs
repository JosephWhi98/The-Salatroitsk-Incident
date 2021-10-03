using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Animator animator;
    public AudioSource source;
    public AudioClip fireClip;
    public AudioClip emptyClip;
    public AudioClip reloadClip; 

    bool canFire;

    public float bulletsInClip = 5f;
    private float maxBulletsInClip = 5f;

    public string emptyString = "Shit... I need to reload (R)";

    private bool reloading;

    public void Fire()
    {
        if (canFire && bulletsInClip > 0 && !reloading)
        {
            RaycastHit hit;


            if (Physics.Raycast(PlayerController.Instance.playerCamera.transform.position, PlayerController.Instance.playerCamera.transform.forward, out hit, 200))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Monster"))
                {
                    CronenbergMonster ai = hit.transform.GetComponent<CronenbergMonster>();

                    if (ai)
                    {
                        ai.Shot();
                    }
                }
            }

            bulletsInClip -= 1;

            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(fireClip);

            animator.SetTrigger("Fire");
            canFire = false;
        }
        else if(!reloading)
        {
            source.pitch = Random.Range(0.9f, 1.1f);
            source.PlayOneShot(emptyClip);

            if(SubtitlesManager.Instance.targetString != emptyString)
            SubtitlesManager.Instance.ShowSubtitle(emptyString, 4f);
        }

    }

    public void Reload()
    {
        if(!reloading)
        StartCoroutine(ReloadRoutine());
    }

    public IEnumerator ReloadRoutine()
    {
        reloading = true; 
         animator.SetTrigger("Reload");
        source.PlayOneShot(reloadClip);
        yield return new WaitForSeconds(3f);
        bulletsInClip = maxBulletsInClip;
        reloading = false; 
    }

    public void SetCanFire()
    {
        canFire = true;
    }
}
