﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Normal.Realtime;

public class GravityGun : MonoBehaviour
{    
    [Range(1f,50f)]
    public float interactDist;

    public Transform HoldPosition;//transform of where you want object to hover
    public float attractSpeed;//speed at which object comes to your hold position

    public float minThrowForce;
    public float maxThrowForce;
    private float throwForce;

    private GameObject objectIHave;
    private Rigidbody objectRB;

    private Vector3 rotateVector = Vector3.one;

    private bool hasObject = false;
    
    
    //public int Ownership = -1;//setting ownership for normcore as it wasn't recognising the raycast as imparting ownership --Richard
    //RealtimeTransform RTTransform;//reatime transform to grab ownership for the raycast --Richard



    private void Start()
    {
        throwForce = minThrowForce;
        //RTTransform = gameObject.GetComponent<RealtimeTransform>();
    }

    private void Update()
    {
        if (SteamVR_Actions.default_GrabGrip.GetStateDown(SteamVR_Input_Sources.Any) && !hasObject)
        {
            DoRay();
            
        }

        if (SteamVR_Actions.default_GrabPinch.GetStateDown(SteamVR_Input_Sources.Any) && hasObject)
        {
            throwForce += 0.1f;
        }

        if (SteamVR_Actions.default_GrabPinch.GetStateUp(SteamVR_Input_Sources.Any) && hasObject)
        {
            ShootObj();
        }

        if(SteamVR_Actions.default_GrabGrip.GetStateUp(SteamVR_Input_Sources.Any) && hasObject)
        {
            DropObj();
        }

        if (hasObject)
        {
            RotateObj();

            if(CheckDist() >= 1f)
            {
                MoveObjToPos();
            }
        }



    }



    //----------------Polish Stuff
    private void CalculateRotVector()
    {
        
        float x = Random.Range(-0.5f, 0.5f);
        float y = Random.Range(-0.5f, 0.5f);
        float z = Random.Range(-0.5f, 0.5f);

        rotateVector = new Vector3(x, y, z);
    }

    private void RotateObj()
    {
        objectIHave.transform.Rotate(rotateVector);
    }


    //----------------Functional Stuff

    public float CheckDist()
    {
        float dist = Vector3.Distance(objectIHave.transform.position, HoldPosition.transform.position);
        return dist;
    }

    private void MoveObjToPos()
    {
        objectIHave.transform.position = Vector3.Lerp(objectIHave.transform.position, HoldPosition.position, attractSpeed * Time.deltaTime);
    }

    private void DropObj()
    {
        //objectRB.velocity = Vector3.zero;
        objectRB.constraints = RigidbodyConstraints.None;
        objectIHave.transform.SetParent(null, false); //edited to setParent from parent as it was yeeting cubes into orbit -Richard
        objectIHave = null;
        hasObject = false;
    }

    private void ShootObj()
    {
        throwForce = Mathf.Clamp(throwForce, minThrowForce, maxThrowForce);
        objectRB.AddForce(HoldPosition.transform.forward * throwForce, ForceMode.Impulse);
        throwForce = minThrowForce;
        DropObj();
    }

    private void DoRay()
    {
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Ray ray = new Ray(HoldPosition.position, HoldPosition.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDist))
        {
            
            if (hit.collider.CompareTag("Block"))
            {
                
                objectIHave = hit.collider.gameObject;
                objectIHave.transform.SetParent(HoldPosition);
                //objectIHave.GetComponent<RealTimeThrowable>().Grabbed();
                objectRB = objectIHave.GetComponent<Rigidbody>();
                objectRB.constraints = RigidbodyConstraints.FreezeAll;

                hasObject = true;

                CalculateRotVector();
            }
        }

    }
   
    
}
