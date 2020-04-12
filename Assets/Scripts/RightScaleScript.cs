using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightScaleScript : MonoBehaviour
{
    public GameObject ScaleDisplay;//the object that displays the scales value.
    Text Display;// The value that displays on the cube
    [HideInInspector]
    public int CoinWeight;
    MeshRenderer MyRend;//meshrenderer to change colours for testing
    private void Start()
    {
        MyRend = gameObject.GetComponent<MeshRenderer>();
        Display = gameObject.GetComponent<Text>();
        CoinWeight = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {      
       
            CoinWeight = CoinWeight + collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            Display.text = CoinWeight.ToString();
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

       
        if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        {
            MyRend.material.color = Random.ColorHSV();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (CoinWeight != 0)
        {
            CoinWeight = CoinWeight - collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            Display.text = CoinWeight.ToString();
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

        }
        if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        {
            MyRend.material.color = Color.grey;
        }
    }


}