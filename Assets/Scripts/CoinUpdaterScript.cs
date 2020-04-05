using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUpdaterScript : MonoBehaviour
{
    GameObject TargetObject;//the gameobject that the coin is linked to to determine its size
    // Start is called before the first frame update
    void Start()
    {
        TargetObject = GetComponent<CoinScript>().CollidedObject;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(gameObject.transform.position, TargetObject.transform.position);
        GetComponent<CoinScript>().EnScale(distance);
        
    }
}
