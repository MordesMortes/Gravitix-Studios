using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleDisplayScript : MonoBehaviour
{
    public GameObject LeftScale;//gameobject of the left scale
    public GameObject RightScale;//gameobject of the right scale
    Text Display;
    private void Start()
    {
        Display = gameObject.GetComponent<Text>();
    }
    public int Weigh()
    {
        int weight = LeftScale.GetComponent<LeftScaleScript>().CoinWeight - RightScale.GetComponent<RightScaleScript>().CoinWeight;
        if (weight <= 0)
        {
            weight = weight * -1;
        }
        Debug.Log("the weight is " + weight.ToString());
        Display.text = weight.ToString();
        return weight;
    }
}
