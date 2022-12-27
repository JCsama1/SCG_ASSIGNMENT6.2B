using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{

    //Camera will shoot a raycast and when it hits an object it will recognize it
    [Header("Rifle Things")]
    public Camera camera;
    public float giveDamageOf = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;

    [Header("Rifle Ammunition and Shooting")]
    private float nextTimeToShoot = 0f;

    [Header("Rifle Effects")]
    public ParticleSystem muzzleSpark;
    public GameObject impactEffect;

   // [Header("Sounds and UI")]

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Fire1") && Time.time >= nextTimeToShoot)
        {
            nextTimeToShoot= Time.time + 1f/fireCharge;
            Shoot();
        }
    }

    void Shoot()
    {
        muzzleSpark.Play();
        RaycastHit hitInfo;

        if(Physics.Raycast(camera.transform.position, camera.transform.forward, out hitInfo, shootingRange)) 
        {
            Debug.Log(hitInfo.transform.name);

            Objects objects = hitInfo.transform.GetComponent<Objects>();

            if(objects != null)
            {
                objects.objectHitDamage(giveDamageOf);
                GameObject impactGO = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
                Destroy(impactGO, 1f);
            }
        }
    }
}
