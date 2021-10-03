using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraInteractions : MonoBehaviour
{
    public bool lockCursor = true; 

    public LayerMask interactableMask;

    private InteractableItem cachedInteractable;
    public GameObject InteractText; 

    void Update()
    {
        // Ensure the cursor is always locked when set
        if (lockCursor && GameManager.Instance.AllowInput)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 6f, interactableMask))
        {
            cachedInteractable = hit.transform.GetComponent<InteractableItem>();
        }
        else
        {
            cachedInteractable = null; 
        }

        InteractText.SetActive(cachedInteractable != null);

     
    }

    public void TryInteract()
    {
            if (cachedInteractable != null)
                cachedInteractable.OnUse();
    }
}
