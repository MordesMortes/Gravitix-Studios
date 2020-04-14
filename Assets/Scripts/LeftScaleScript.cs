using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftScaleScript : MonoBehaviour

{
    public GameObject ScaleDisplay;//the object that displays the scales value.
   
    [HideInInspector]
    public int CoinWeight=0;
   
    private void OnCollisionEnter(Collision collision)
    {
        
            CoinWeight = CoinWeight + collision.gameObject.GetComponent<CoinScript>().CoinWeight;
      
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

      
    }
    private void OnCollisionExit(Collision collision)
    {
        if (CoinWeight != 0)
        {
            CoinWeight = CoinWeight - collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            
            ScaleDisplay.gameObject.GetComponent<ScaleDisplayScript>().Weigh();

        }
        
    }
    
    
}
