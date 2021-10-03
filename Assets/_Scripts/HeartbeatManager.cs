using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing; 

public class HeartbeatManager : MonoBehaviour
{
    public float intensity; 
    public AudioSource heartBeatSource;
    public PostProcessVolume postVolume;
    Vignette vignette;

    public float minVignette;
    public float maxVignette;

    public Transform target;
    public float maxDistance;


    public void Update()
    {
        if (target.gameObject.activeInHierarchy && GameManager.Instance.playing)
        {
            intensity = 1 - Vector3.Distance(transform.position, target.position) / maxDistance;
            intensity = Mathf.Clamp01(intensity);

            heartBeatSource.volume = intensity;

            postVolume.profile.TryGetSettings(out vignette);

            if (intensity > 0.5f)
            {
                GameManager.Instance.timeInRange += Time.deltaTime;
            }
            else
            {
                GameManager.Instance.timeInRange -= Time.deltaTime;
            }

            if (vignette)
            {
                vignette.intensity.value = Mathf.Lerp(minVignette, maxVignette, intensity);
            }
        }
    }
}
