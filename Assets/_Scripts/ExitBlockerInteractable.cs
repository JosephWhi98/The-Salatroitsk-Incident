using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBlockerInteractable : InteractableItem
{
    public override void OnUse()
    {
        if (GameManager.Instance.samplesCollected < 6)
            SubtitlesManager.Instance.ShowSubtitle("I cant leave yet.", 4f);
        else
        {
            SubtitlesManager.Instance.ShowSubtitle("There's the car!.", 4f);
            GameManager.Instance.Escape();
        }
    }
}
