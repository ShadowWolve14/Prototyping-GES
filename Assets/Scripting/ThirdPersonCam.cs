using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Transform player;
    public Transform PlayerObj;
    public Rigidbody rb;

    public float rotationspeed;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        //Rotate Orientation
        Vector3 viewDir = player.position - new Vector3(transform.position.x, transform.position.y, transform.position.z);
        orientation.forward = viewDir.normalized;

        //rotate player Object
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 InputDir = -orientation.forward * verticalInput + -orientation.right * horizontalInput;

        if (InputDir != Vector3.zero)
        {
            PlayerObj.forward = Vector3.Slerp(PlayerObj.forward, InputDir.normalized, Time.deltaTime*rotationspeed);
        }
    }
}
