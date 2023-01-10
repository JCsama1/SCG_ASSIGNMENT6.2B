using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [Header("Generator Lights and button")]
    public GameObject greenLight;
    public GameObject redLight;
    public bool button;

    [Header("Generator Sound Effects and radius")]
    private float radius = 2f;
    public PlayerScript player;
    public Animator animation;
    public AudioSource audioSource;

    private void Awake()
    {
        button= false;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(Input.GetKeyDown("q") && Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            button = true;
            animation.enabled= false;
            greenLight.SetActive(false);
            redLight.SetActive(true);
            audioSource.Stop();
            //objective complete
            ObjectivesComplete.occurence.GetObjectivesDone(true, true, true, false);
        }
        else if (button == false)
        {
            greenLight.SetActive(true);
            redLight.SetActive(false);
        }
    }
}
