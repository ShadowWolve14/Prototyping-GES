using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    InpputManager inpputManager;

    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody Bee;

    public float movementSpeed = 7;
    public float rotationSpeed = 15;

    private void Awake()
    {
        inpputManager = GetComponent<InpputManager>();
        Bee = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        HandleMovement();
        HandleRotation();
    }
    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inpputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inpputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        Bee.linearVelocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;
        float verticalInput = inpputManager.verticalInput;
        float horizontalInput = inpputManager.horizontalInput;
        if (verticalInput != 0 || horizontalInput != 0)
        {
            targetDirection = cameraObject.forward * inpputManager.verticalInput;
            targetDirection = targetDirection + cameraObject.right * inpputManager.horizontalInput;
            targetDirection.Normalize();
            targetDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion modelRotation = Quaternion.Euler(0, 90, 0);
            targetRotation = targetRotation * modelRotation;
            Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            transform.rotation = playerRotation;
        }
        
    }
}
