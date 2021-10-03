using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class GameManager : Singleton<GameManager>
{
    public bool playing = true; 

    public float totalExposureTime; 
    public int samplesCollected;
    public AudioSource pickupSource;
    public AudioClip pickupClip;
    public AudioClip finalPickupClip;

    public AudioClip gameWinClip;

    public Transform startPosition;
    public TextMeshProUGUI objectiveText; 
    public bool AllowInput { get { return !paused && playing; } }
    public bool paused;

    public GameObject player;
    public GameObject pause;

    public GameObject playerHands; 

    public CronenbergMonster monster;

    public GameObject playerScareMonster;
    public MainMenu menu;

    public AudioClip gameOverClip;

    public float timeInRange = 0f;

    public CanvasGroup endTitleGroup;

    public TextMeshProUGUI tutorialText;
    public bool gigerCounterTutorialCompleted; 

    public IEnumerator Start()
    {
        monster.gameObject.SetActive(false);
        playing = false;
        yield return new WaitForSeconds(1f);
        ScreenFader.Instance.Fade(0, 2f);
        yield return new WaitForSeconds(2f);
        playing = true;
    }

    public void Pause()
    {
        if (playing)
        {
            if (!paused)
            {
                paused = true;
                Time.timeScale = 0f;
            }
            else
            {
                paused = false;
                Time.timeScale = 1f;
            }

            player.SetActive(!paused);
            pause.SetActive(paused);
        }
    }

    public bool triggerGameOver;
    public bool GameOver; 

    public void Update()
    {
        if (!gigerCounterTutorialCompleted)
        {
            tutorialText.text = "HOLD 'RMB' TO BRING UP GIGER COUNTER";
        }
        else
        {
            tutorialText.text = "";
        }

        if (triggerGameOver)
        {
            TriggerGameOver();
            triggerGameOver = false;
        }

        timeInRange = Mathf.Clamp(timeInRange, 0, 5f);

        if (playing && timeInRange >= 3f && monster.lastShotTime < Time.time)
        {
            TriggerGameOver();
        }


        if (playing)
        {
            totalExposureTime += Time.deltaTime;
            if (samplesCollected < 6f)
            {
                objectiveText.text = "Objective: Collect rock samples";
            }
            else
            {
                objectiveText.text = "Objective: Escape";
            }
        }
        else
        {
            objectiveText.text = "";
        }
    }

    public void Escape()
    {
        playing = false ;

        monster.gameObject.SetActive(false);

        StartCoroutine(EscapeRoutine());
    }

    public IEnumerator EscapeRoutine()
    {
        yield return new WaitForSeconds(1f);

        ScreenFader.Instance.Fade(1f, 2f);
        pickupSource.PlayOneShot(gameWinClip);

        yield return new WaitForSeconds(7.5f);
        endTitleGroup.alpha = 1f;
        yield return new WaitForSeconds(3f);

        float t = 0;
        float endTime = Time.time + 2f;

        while (endTime > Time.time)
        {
            t += Time.deltaTime;

            endTitleGroup.alpha = Mathf.Lerp(1, 0, t /2f);

            yield return null; 
        }

        endTitleGroup.alpha = 0f; 

        menu.ExitToMenu();
    }

    public void TriggerGameOver()
    {
        GameOver = true;
        playing = false;
        PlayerController.Instance.ResetCamera();
        StartCoroutine(TriggerGameOverRoutine());
    }

    public IEnumerator TriggerGameOverRoutine()
    {
        ScreenFader.Instance.Fade(1f, 1f);
        pickupSource.PlayOneShot(gameOverClip);
        yield return new WaitForSeconds(1f);
        monster.gameObject.SetActive(false);
        playerHands.SetActive(false);
        ScreenFader.Instance.Fade(0f, 1f);

        playerScareMonster.SetActive(true);

        yield return new WaitForSeconds(2f);

        menu.ExitToMenu();
    }

    public void CollectedRockSample()
    {
        if (samplesCollected == 0)
        {
            monster.gameObject.SetActive(true);
            monster.Init();
        }

        samplesCollected++;

        if (samplesCollected < 6f)
            pickupSource.PlayOneShot(pickupClip);
        else
        {
            pickupSource.PlayOneShot(finalPickupClip);
            StartCoroutine(FinaleSubtitleRoutine()) ;
        }
    }

    public IEnumerator FinaleSubtitleRoutine()
    {
        yield return new WaitForSeconds(4f);

        SubtitlesManager.Instance.ShowSubtitle("That's all of them, time to get out of here.", 4f);
    }
}
