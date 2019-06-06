using System.Collections;
using UnityEngine;
using InControl;

/// <summary>
/// Uses InControl for touch input and device input
/// Handles multiple input types for the player's jump/dash
/// </summary>
[System.Serializable]
public class PlayerInput
{

    /// <summary>
    /// Our input device from InControl, handles most controller devices and touch controls
    /// </summary>
    private InputDevice inputDevice;

    /// <summary>
    /// The input events for jump, handles keyboard/touch/controller
    /// </summary>
    [Tooltip("Key/Button/Touch Inputs for jumping, make sure to setup touch controls with InControl and set it's target")]
    public PlayerInputEvents jumpInput;

    /// <summary>
    /// The input events for dash, handles keyboard/touch/controller
    /// </summary>
    [Tooltip("Key/Button/Touch Inputs for dashing, make sure to setup touch controls with InControl and set it's target")]
    public PlayerInputEvents dashInput;

    [HideInInspector]
    public bool disabled;

    public void GetInputs()
    {
        //Gets the current active device that the system is using
        inputDevice = InputManager.ActiveDevice;

        if (disabled)
            return;

        //Check if we're on touch controls or controller device
        if (inputDevice != InputDevice.Null && inputDevice != TouchManager.Device)
        {
            //Disable touch controls if we found a controller device
            TouchManager.ControlsEnabled = false;
        }

        //Fire all the Jump and Dash input events
        jumpInput.FireInputEvents(inputDevice);
        dashInput.FireInputEvents(inputDevice);
    }

    [System.Serializable]
    public class PlayerInputEvents
    {

        #region INPUTS
        [Tooltip("The keyboard key for this event")]
        [SerializeField] KeyCode key;

        [Tooltip("The touch screen input for this event, make sure to link it to controller button since they use the same input type")]
        [SerializeField] InputControlType touch;

        [Tooltip("The button to press on controller for this event, make sure to link it to touch since they use the same input type")]
        [SerializeField] InputControlType button;
        #endregion

        #region INPUT EVENTS
        public delegate void InputHandler();

        /// <summary>
        /// When input is pressed down, called once
        /// </summary>
        public event InputHandler OnInputDown;

        /// <summary>
        /// When input is being pressed, continuous calls until input is let go
        /// </summary>
        public event InputHandler OnInput;

        /// <summary>
        /// When input is released, called once
        /// </summary>
        public event InputHandler OnInputUp;
        #endregion

        /// <summary>
        /// Fires the input events for keyboard/touch/controller, takes in the current active device from InControl
        /// </summary>
        /// <param name="inputDevice">The current Active Device in the System</param>
        public void FireInputEvents(InputDevice inputDevice)
        {
            if (Input.GetKeyDown(key) || //Check for keyboard key pressed down
                inputDevice.GetControl(touch).WasPressed || //Check for touch control pressed down
                inputDevice.GetControl(button).WasPressed) //Check for Controller button pressed down
            {
                //if any of the input has been pressed down we fire the event
                if (OnInputDown != null)
                    OnInputDown();
            }

            if (Input.GetKey(key) || //Check for keyboard key press
                inputDevice.GetControl(touch).HasInput || //Check for touch control press
                inputDevice.GetControl(button).HasInput) //Check for touch control press
            {

                //if any of the input has been pressed down we fire the event
                if (OnInput != null)
                    OnInput();
            }

            if (Input.GetKeyUp(key) || //Check for keyboard key release
               inputDevice.GetControl(touch).WasReleased || //Check for touch control release
               inputDevice.GetControl(button).WasReleased) //Check for Controller button release
            {
                //if any of the input has been pressed down we fire the event
                if (OnInputUp != null)
                    OnInputUp();
            }
        }
    }
}
