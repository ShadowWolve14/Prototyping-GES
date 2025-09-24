using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Security.Cryptography;


public class Projectile : MonoBehaviour
{
    //bullet

    public GameObject bullet;

    //bullet force
    public float shootForce;

    //Gun Stats
    public float timeBetweenShooting, spread, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold, isReloading;
    int bulletsleft, bulletShot;

    //bools
    bool shooting, readyToShoot, reloading;

    //references


    public Transform attackpoint;
    public Transform PlayerObj; // Referenz auf dein Spielerobjekt
    InputSystem_Actions inputSystem_Actions;

    //Graphics
    public GameObject muzzleFlash;
    public TextMeshProUGUI ammunitionDisplay;

    public bool allowInvoke = true;
    private void OnEnable()
    {
        inputSystem_Actions = new InputSystem_Actions();
    }
    private void Awake()
    {
        //make sure magazine is full
        bulletsleft = magazineSize;
        readyToShoot = true;
    }
    private void Update()
    {
        attackpoint.rotation = PlayerObj.rotation;
        MyInput();


        //set ammo Display, if it exist
        if(ammunitionDisplay != null)
        {
            ammunitionDisplay.SetText(bulletsleft/bulletsPerTap+ "/" + magazineSize/bulletsPerTap);
        }
    }
    private void MyInput()
    {
        //Check if allowed to hold down button and take corresponding input
        if (allowButtonHold)
        {
            //inputSystem_Actions.Player.Attack.performed += context => shooting = true;
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            //inputSystem_Actions.Player.Attack.started += context => shooting = true;
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        //reloading
        //inputSystem_Actions.Player.Jump.started += context => isReloading = true;
        if (Input.GetKey(KeyCode.R) && bulletsleft < magazineSize && !reloading)
        {
            Reload();
        }
        //reload automatically when trying to shoot without Ammo
        if(readyToShoot && shooting && !reloading && bulletsleft <= 0)
        {
            Reload();
        }
        //Shooting
        if (readyToShoot && shooting && !reloading && bulletsleft >0)
        {
            //Set bullet shot to 0
            bulletShot = 0;

            Shoot();
        }

    }
    private void Shoot()
    {
        readyToShoot = false;


        //direction of the bullet
        Vector3 directionWithoutSpread = transform.forward;

        //Calculate spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        float z = Random.Range(-spread, spread);

        //calculate direction with spread

        Vector3 direchtionWithSpread = directionWithoutSpread + new Vector3(x,y,z);

        //Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackpoint.position, Quaternion.identity);

        //rotate bullet to shoot direction
        currentBullet.transform.forward = direchtionWithSpread.normalized;

        //add forces
        currentBullet.GetComponent<Rigidbody>().AddForce(direchtionWithSpread.normalized * shootForce, ForceMode.Impulse);
        

        //Instantiate muzzle Flash
        if(muzzleFlash != null)
        {
            Instantiate(muzzleFlash, attackpoint.position, Quaternion.identity);
        }
        bulletsleft--;
        bulletShot++;

        //Invoke resetShot function(if not already invoked)
        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;    
        }

        //if more than one bulletsPerTabe make sure to repeat shoot function
        if (bulletShot < bulletsPerTap && bulletsleft >0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }
    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadingFinished", reloadTime);
    }

    private void ReloadingFinished()
    {
        bulletsleft = magazineSize;
        reloading = false;
    }
}
