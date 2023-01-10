using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : MonoBehaviour
{
    [Header("Computer On/Off")]
    public bool lightsOn = true;
    private float radius = 2.5f;
    public Light lights;

    [Header("Computer Assign Things")]
    public PlayerScript player;
    [SerializeField] private GameObject ComputerUI;
    [SerializeField] private int showComputerUIFor = 5;

    private void Awake()
    {
        lights= GetComponent<Light>();
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < radius)
        {
            if(Input.GetKeyDown("q"))
            {
                StartCoroutine(ShowComputerUI());
                lightsOn= false;
                lights.intensity= 0;
                //obejctive complete
                ObjectivesComplete.occurence.GetObjectivesDone(true, true, false, false);
                //sound effect
            }
        }
    }
    IEnumerator ShowComputerUI()
    {
        ComputerUI.SetActive(true);
        yield return new WaitForSeconds(showComputerUIFor);
        ComputerUI.SetActive(false);
    }
}
