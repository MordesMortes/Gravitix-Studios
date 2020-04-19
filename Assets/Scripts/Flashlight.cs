using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

//Code by Ken :)

public class Flashlight : MonoBehaviour
{
    private Light myLight;


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
        }

      
    }
}
