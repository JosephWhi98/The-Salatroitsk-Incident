using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public void QuitGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    public void PlayGame()
    {
            StartCoroutine(PlayGameRoutine());
    }

    public void ExitToMenu()
    {
            StartCoroutine(ExitToMenuRoutine());
    }

    public IEnumerator ExitToMenuRoutine()
    {
        //uiManager.ScreenFade(Color.black, 1.5f);
        AudioManager.Instance.SnapAudioClose();
        GameManager.Instance.Pause();
        ScreenFader.Instance.Fade(1, 1.5f);
        yield return new WaitForSecondsRealtime(1.5f);


        SceneManager.LoadScene("Menu");
    }

    public void Resume()
    {
        GameManager.Instance.Pause();
    }


    public IEnumerator PlayGameRoutine()
    {
        AudioManager.Instance.SnapAudioClose();
        ScreenFader.Instance.Fade(1, 1.5f);
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene("Main");
    }
}
