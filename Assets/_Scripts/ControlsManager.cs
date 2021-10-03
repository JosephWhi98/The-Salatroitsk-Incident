using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{

	Controls controls;

	public GigerCounter gigerCounter;
	public CameraInteractions cameraInteractions;
	public Gun gun;

	public LockedNavigation lockedNav;

	public InputDevice currentInputDevice { get; private set; }
	public string currentScheme { get; private set; }


	public void Awake()
	{
		controls = new Controls();

		controls.Main.Aim.performed += AimPressed;
		controls.Main.Interact.performed += InteractPressed;
		controls.Main.Running.performed += HandleRunning;
		controls.Main.Pause.performed += Pause;
		controls.Main.Fire.performed += FirePressed;
		controls.Main.Reload.performed += Reload;
		controls.Main.UIControllerSelect.performed += UISelect;

		InputSystem.onActionChange += OnActionChange;

		//controls.Main.Pause.performed += PausePressed;

		controls.Enable();
	}

	private  void OnActionChange(object obj, InputActionChange change)
	{
		if (change == InputActionChange.ActionPerformed)
		{
			InputDevice lastDevice = ((InputAction)obj).activeControl.device;

			if (lastDevice != currentInputDevice || currentInputDevice == null)
			{
				if (lastDevice.ToString().Contains("Mouse") || lastDevice.ToString().Contains("Keyboard"))
				{
					if (currentScheme != "Keyboard")
						lockedNav.OnControllerInput(false);

					currentScheme = "Keyboard";
				}
				else
				{
					if (currentScheme != "Gamepad")
						lockedNav.OnControllerInput(true);

					currentScheme = "Gamepad";
				}
			}
			//if (lastDevice != currentInputDevice || currentInputDevice == null)
			//{
			//	currentInputDevice = lastDevice;
			//	InputControlScheme[] schemes = controls.controlSchemes.ToArray();
			//	InputControlScheme scheme = (InputControlScheme)InputControlScheme.FindControlSchemeForDevice<InputControlScheme[]>(currentInputDevice, schemes);
			//	if (currentScheme != scheme && scheme != null)
			//	{
			//		currentScheme = scheme;
			//	}
			//}
		}
	}

	public void OnDestroy()
	{
		controls.Main.Aim.performed-= AimPressed;
		controls.Main.Interact.performed-= InteractPressed;
		controls.Main.Running.performed -= HandleRunning;
		controls.Main.Pause.performed -= Pause;
		controls.Main.Fire.performed -= FirePressed;
		controls.Main.Reload.performed -= Reload;
		controls.Main.UIControllerSelect.performed -= UISelect;

		InputSystem.onActionChange -= OnActionChange;
	}

	public void Update()
	{ 


		if (GameManager.Instance)
		{
			if (GameManager.Instance.AllowInput)
			{
				PlayerController.Instance.moveInput = controls.Main.Move.ReadValue<Vector2>();
				PlayerController.Instance.lookInput = controls.Main.Look.ReadValue<Vector2>() * 0.5f * 0.1f;
			}
			else
			{
				PlayerController.Instance.moveInput = Vector2.zero;
				PlayerController.Instance.lookInput = Vector2.zero;
			}
		}

		lockedNav.Input(controls.Main.UIController.ReadValue<float>());

		//Debug.Log(currentScheme.name);
	}

	public void UISelect(InputAction.CallbackContext ctx)
	{
		if (lockedNav.gameObject.activeInHierarchy)
		{
			lockedNav.SelectButton();
		}
	}

	public void AimPressed(InputAction.CallbackContext ctx)
	{
		if (GameManager.Instance && GameManager.Instance.AllowInput)
		{
			gigerCounter.aiming = ctx.ReadValueAsButton();
		}
	}

	public void InteractPressed(InputAction.CallbackContext ctx)
	{
		if (GameManager.Instance && GameManager.Instance.AllowInput)
		{
			cameraInteractions.TryInteract();
		}
	}

    public void Pause(InputAction.CallbackContext ctx)
	{
		if (GameManager.Instance)
		{
			GameManager.Instance.Pause();
			}
    }

    public void FirePressed(InputAction.CallbackContext ctx)
	{
		 if (GameManager.Instance && GameManager.Instance.AllowInput)
		{
			if (!gigerCounter.aiming)
				gun.Fire();
		}
	}

	public void Reload(InputAction.CallbackContext ctx)
	{
		if (GameManager.Instance && GameManager.Instance.AllowInput)
		{
			if (!gigerCounter.aiming)
				gun.Reload();
		}
	}

	public void HandleRunning(InputAction.CallbackContext ctx)
	{
			if (GameManager.Instance && GameManager.Instance.AllowInput)
			{
				PlayerController.Instance.isRunning = ctx.ReadValueAsButton();
			}
	}

}
