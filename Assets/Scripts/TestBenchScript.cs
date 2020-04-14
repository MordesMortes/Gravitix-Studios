using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBenchScript : MonoBehaviour
{
    public GameObject RealtimeCubes;//the hidden realitime cubes folder to be set active
    public GameObject NonRealtimeCubes;//the folder of the non realtime cubes to be set inactive
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            
            collision.gameObject.GetComponent<CoinScript>().Reveal();
            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == true)
            {
                gameObject.GetComponent<Rigidbody>().Sleep();
                gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                RealtimeCubes.SetActive(true);
                NonRealtimeCubes.SetActive(false);
                for (int i = 0; i < FindObjectsOfType<CoinScript>().Length; i++)
                {                    
                    FindObjectsOfType<CoinScript>()[i].EnMass();
                }

            }
        }
    }
}
