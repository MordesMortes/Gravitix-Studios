using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBenchScript : MonoBehaviour
{
    public GameObject RealtimeCubes;//the hidden realitime cubes folder to be set active
    public GameObject[] NonRealtimeCubes;//the folder of the non realtime cubes to be set inactive
    public GameObject FancyScales;//the fancy scales that use physics for the first puzzle
    public GameObject RealtimeScales;//the scale for the realtime cubes as they dislike physics
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
                //NonRealtimeCubes.SetActive(false);
                FancyScales.SetActive(false);
                RealtimeScales.SetActive(true);
                for (int i = 0; i < NonRealtimeCubes.Length; i++)
                {
                    NonRealtimeCubes[i].SetActive(false);
                }
                for (int i = 0; i < FindObjectsOfType<CoinScript>().Length; i++)
                {                    
                    FindObjectsOfType<CoinScript>()[i].EnMass();
                }

            }
        }
    }
}
