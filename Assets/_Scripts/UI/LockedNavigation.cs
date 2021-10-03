using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedNavigation : MonoBehaviour
{
    public MenuButtons[] buttons;
    public int currentSelected;
    public ControlsManager controlsManager;
    public bool active;

    public void OnEnable()
    {
        OnControllerInput(controlsManager.currentScheme == "Gamepad");
        
    }

    public void OnDisable()
    {
        active = false;
    }

    public void OnControllerInput(bool input)
    {
        Debug.Log("Locked nav enabled");
        if (input && !active)
            buttons[currentSelected].MouseEnter();
        else if(!input && active)
            buttons[currentSelected].MouseExit();

        active = input;
    }

    public void SelectButton()
    {
        if (active)
            buttons[currentSelected].OnClick();
    }

    float nextInput = 0f; 

    public void Input(float input)
    {
        if (!active)
            return;

        if (Mathf.Abs(input) > 0f && Time.unscaledTime > nextInput)
        {
            nextInput = Time.unscaledTime + 0.2f;

            buttons[currentSelected].MouseExit();

            if (input > 0)
                currentSelected -= 1;
            else if(input < 0)
                currentSelected += 1;

            if (currentSelected > 1)
                currentSelected = 0;
            else if (currentSelected < 0)
                currentSelected = 1;

            buttons[currentSelected].MouseEnter();


        }
    }

}
