using System.Collections;
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
    Vector3 InitialPosition;//initial position of player holding gravity gun for respawning 
    GameObject player;//the player in the scene 

    private void Start()
    {
        throwForce = minThrowForce;
        InitialPosition = FindObjectOfType<Player>().transform.position;
        player = FindObjectOfType<Player>().gameObject;
        
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
        objectRB.constraints = RigidbodyConstraints.None;
        objectIHave.transform.parent = null;
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
        //Ray ray = cam.ScreenPointToRay(Input.mousePosition);//works for 3d games not for VR
        Ray ray = new Ray(HoldPosition.position, HoldPosition.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDist))
        {
            
            if (hit.collider.CompareTag("Block") && hit.collider.GetComponent<RealTimeThrowable>())
            {
                
                objectIHave = hit.collider.gameObject;
                objectIHave.transform.SetParent(HoldPosition);                
                objectRB = objectIHave.GetComponent<Rigidbody>();
                objectRB.constraints = RigidbodyConstraints.FreezeAll;
                objectIHave.GetComponent<RealTimeThrowable>().Grabbed();
                hasObject = true;

                CalculateRotVector();
            }
            if (hit.collider.CompareTag("Block") && !hit.collider.GetComponent<RealTimeThrowable>())
            {
                objectIHave = hit.collider.gameObject;
                objectIHave.transform.SetParent(HoldPosition);
                objectRB = objectIHave.GetComponent<Rigidbody>();
                objectRB.constraints = RigidbodyConstraints.FreezeAll;
                hasObject = true;
            }
        }

    }
   public void ReSpawn()
    {        
        player.transform.position = InitialPosition;
    }
    
}
