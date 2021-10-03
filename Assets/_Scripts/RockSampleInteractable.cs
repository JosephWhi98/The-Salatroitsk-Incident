using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSampleInteractable : InteractableItem
{

    public override void OnUse()
    {
        GameManager.Instance.CollectedRockSample();
        SubtitlesManager.Instance.ShowSubtitle(interactSubtitle + " " + GameManager.Instance.samplesCollected + " of 6" , 4f);

        gameObject.SetActive(false);
    }
}
