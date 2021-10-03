using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableItem : MonoBehaviour
{

    public string interactSubtitle; 

    public virtual void OnUse()
    {
        SubtitlesManager.Instance.ShowSubtitle(interactSubtitle, 4f);
    }
}
