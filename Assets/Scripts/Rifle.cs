using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{

    //Camera will shoot a raycast and when it hits an object it will recognize it
    [Header("Rifle Things")]
    public Camera cam;
    public float giveDamageOf = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    public Animator animator;
    public PlayerScript player;

    [Header("Rifle Ammunition and Shooting")]
    private int maximumAmmunition = 20;
    private int mag = 15;
    private int presentAmmunition;
    public float relaodingTime = 1.3f;
    private float reloadingTime = 1.3f;
    private bool setReloading = false;
    private float nextTimeToShoot = 0f;

    [Header("Rifle Effects")]
    public ParticleSystem muzzleSpark;
    public GameObject impactEffect;
    public GameObject goreEffect;

    // [Header("Sounds and UI")]

    private void Awake()
    {
        presentAmmunition = maximumAmmunition;
    }

    // Update is called once per frame
    void Update()
    {
        if(setReloading)
        return;

        if(presentAmmunition <= 0)
        {
            StartCoroutine(Reload());
            return;
        }


        if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
        {
            animator.SetBool("Fire", true);
            animator.SetBool("Idle", false);
            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
        }
        else if(Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IdleAim", true);
            animator.SetBool("FireWalk", true);
            animator.SetBool("Walk", true);
            animator.SetBool("Reloading", false);
        }
        else if (Input.GetButton("Fire2") && Input.GetButton("Fire1"))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IdleAim", true);
            animator.SetBool("FireWalk", true);
            animator.SetBool("Walk", true);
            animator.SetBool("Reloading", false);
        }
        else 
        {
            animator.SetBool("Fire", false);    
            animator.SetBool("Idle", true);
            animator.SetBool("FireWalk", false);
            animator.SetBool("Reloading", false);

        }
    }

    void Shoot()
    {
        //cehck for mag

        if (mag == 0)
        {
            //show no ammo text
            return;
        }

        presentAmmunition--;

        if(presentAmmunition == 0)
        {
            mag--;
        }

        muzzleSpark.Play();
        RaycastHit hitInfo;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange)) 
        {
            Debug.Log(hitInfo.transform.name);

            Objects objects = hitInfo.transform.GetComponent<Objects>();
            Enemy enemy = hitInfo.transform.GetComponent<Enemy>();

            if(objects != null)
            {
                objects.objectHitDamage(giveDamageOf);
                GameObject impactGO = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(impactGO, 1f);
            }
            else if(enemy != null)
            {
                enemy.enemyHitDamage(giveDamageOf);
                GameObject impactGO = Instantiate(goreEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(impactGO, 2f);
            }
        }
    }


    //reload function
    IEnumerator Reload()
    {
        player.playerSpeed = 0f;
        player.playerSprint = 0f;
        setReloading = true;
        Debug.Log("Reloading...");
        animator.SetBool("Reloading", true);
        yield return new WaitForSeconds(reloadingTime);
        animator.SetBool("Reloading", false);
        presentAmmunition = maximumAmmunition;
        player.playerSpeed = 1.9f;
        player.playerSprint = 3;
        setReloading = false;
    }
}
