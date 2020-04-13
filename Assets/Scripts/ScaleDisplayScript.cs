using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleDisplayScript : MonoBehaviour
{
    public GameObject LeftScale;//gameobject of the left scale
    public GameObject RightScale;//gameobject of the right scale
    Text Display;//remove if not using (1)
    private void Start()
    {
        Display = gameObject.GetComponent<Text>();//remove entire start if not using (1)
    }
    public int Weigh()
    {
        int weight = LeftScale.GetComponent<LeftScaleScript>().CoinWeight - RightScale.GetComponent<RightScaleScript>().CoinWeight;
        if (weight <= 0)
        {
            weight = weight * -1;//corrects negatives into positives
        }
        Debug.Log("the weight is " + weight.ToString());//testing message remove on cleanup
        Display.text = weight.ToString();//testing message remove on cleanup if text isn't going to be used in experience (1)
        return weight;
    }
}
