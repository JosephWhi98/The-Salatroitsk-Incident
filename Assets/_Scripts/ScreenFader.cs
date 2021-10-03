using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFader : Singleton<ScreenFader>
{

    public CanvasGroup screenfadeGroup;
    public UnityEngine.UI.Image fadeImage; 
    private Coroutine fadeRoutine;

    public bool fadeOnStart = false; 

    public IEnumerator  Start()
    {
        screenfadeGroup.alpha = 1;

        yield return new WaitForSeconds(1f);

        if (fadeOnStart)
        {
            Fade(0, 2f);
        }
    }

    public void Update()
    {
        fadeImage.raycastTarget = (screenfadeGroup.alpha > 0);
    }

    public void Fade(float target, float time)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeRoutine(target, time));
    }

    public IEnumerator FadeRoutine(float target, float time)
    {
        float start = screenfadeGroup.alpha;

        float endTime = Time.unscaledTime * time;
        float t = 0;

        while (Time.unscaledTime < endTime)
        {
            t += Time.unscaledDeltaTime;
            screenfadeGroup.alpha = Mathf.Lerp(start, target, t / time);
            yield return null;
        }

        screenfadeGroup.alpha = target;
    }

}
