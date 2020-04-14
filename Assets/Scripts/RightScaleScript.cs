using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightScaleScript : MonoBehaviour
{
    public GameObject ScaleDisplay;//the object that displays the scales value.
    //Text Display;// The value that displays on the scale remove if not using(1)
    [HideInInspector]
    public int CoinWeight = 0;
    MeshRenderer MyRend;//meshrenderer to change colours for testing remove if not using (2)
    private void Start()
    {
       //MyRend = gameObject.GetComponent<MeshRenderer>();//(2)
        //Display = gameObject.GetComponent<Text>();//(1)
        //CoinWeight = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {      
       
            CoinWeight = CoinWeight + collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            //Display.text = CoinWeight.ToString();//(1)
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

       
        //if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        //{
        //    MyRend.material.SetColor("_BaseColor", Random.ColorHSV());// remove entire if statement if not using (2)
        //}
    }
    private void OnCollisionExit(Collision collision)
    {
        if (CoinWeight != 0)
        {
            CoinWeight = CoinWeight - collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            //Display.text = CoinWeight.ToString();//(1)
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

        }
        //if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        //{
        //    MyRend.material.SetColor("_BaseColor", Color.gray);//remove entire if statement if not using (2)            
        //}
    }


}