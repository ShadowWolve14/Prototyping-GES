using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InpputManager : MonoBehaviour
{
    InputSystem_Actions inputSystem_Actions;

    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;

    private void OnEnable()
    {
        if (inputSystem_Actions == null)
        {
            inputSystem_Actions = new InputSystem_Actions();

            inputSystem_Actions.Player.Move.performed += inputSystem_Actions => movementInput = inputSystem_Actions.ReadValue<Vector2>();

            inputSystem_Actions.Player.Move.canceled += inputSystem_Actions => movementInput = Vector2.zero;

        }

        inputSystem_Actions.Enable();

    }

    private void OnDisable()
    {
        inputSystem_Actions.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        //HandleActionInput
    }
    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }
}
