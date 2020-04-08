using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleScript : MonoBehaviour

{
    Text Display;// The value that displays on the cube
    private void Start()
    {
        Display = gameObject.GetComponent<Text>();
    }
    private void OnCollisionStay(Collision collision)
    {
        int weight = collision.gameObject.GetComponent<CoinScript>().CoinWeight;
        ScaleValue(weight);
    }
    public int ScaleValue(int weight)
    {   
        Debug.Log(weight.ToString());
        return weight;       
    }
}
