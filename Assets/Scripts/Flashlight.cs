using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//Code by Ken :)

public class Flashlight : MonoBehaviour
{
    private Light myLight;
    public AudioSource click;//audiosource for clicks
    public AudioClip on;//on click
    public AudioClip off;//off click


    void Start()
    {
        myLight = GetComponent<Light>();
        myLight.enabled = false;
    }

    void Update()
    {
        if (SteamVR_Actions.default_LightToggle.GetStateUp(SteamVR_Input_Sources.Any))
        {
            myLight.enabled = !myLight.enabled;
            if (myLight.enabled)
            {
                click.clip = on;
                click.Play();
            }
            if (myLight.enabled == false)
            {
                click.clip = off;
                click.Play();
            }
        }

      
    }
}
