using UnityEngine;

public class walk : MonoBehaviour
{
    Vector3 movement;
    [SerializeField] float movementspeed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        transform.Translate(movement * Time.deltaTime * movementspeed, Space.World);
    }
}
