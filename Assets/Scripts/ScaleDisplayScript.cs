using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleDisplayScript : MonoBehaviour
{
    public GameObject LeftScale;//gameobject of the left scale
    public GameObject RightScale;//gameobject of the right scale
    
    public int Weigh()
    {
        int weight = LeftScale.GetComponent<LeftScaleScript>().CoinWeight - RightScale.GetComponent<RightScaleScript>().CoinWeight;
        if (weight <= 0)
        {
            weight = weight * -1;//corrects negatives into positives
        }
        Debug.Log("the weight is " + weight.ToString());//testing message remove on cleanup
        
        return weight;
    }
}
