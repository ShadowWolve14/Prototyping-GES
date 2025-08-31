using UnityEngine;

public class playerManager : MonoBehaviour
{
    InpputManager inpputmanager;
    PlayerLocomotion playerLocomotion;

    private void Awake()
    {
        inpputmanager = GetComponent<InpputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        inpputmanager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
}
