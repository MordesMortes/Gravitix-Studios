using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBenchScript : MonoBehaviour
{
    public GameObject RealtimeCubes;//the hidden realitime cubes folder to be set active
    public GameObject[] NonRealtimeCubes;//the folder of the non realtime cubes to be set inactive
    public GameObject BalanceScales;//the fancy scales that use physics for the first puzzle
    public GameObject RealtimeScales;//the scale for the realtime cubes as they dislike physics
    public GameObject Collection;//gameobject for the collectionTable
    public AudioSource Sound;//audio source
    public AudioClip Disapointment;//the sound file that indicates failure
    public AudioClip Success;//the sound that indicates success
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
                BalanceScales.SetActive(false);
                RealtimeScales.SetActive(true);
                for (int i = 0; i < NonRealtimeCubes.Length; i++)
                {
                    //NonRealtimeCubes[i].GetComponent<CoinScript>().Shade("Dissolve");//adds Dissolve shader 
                    NonRealtimeCubes[i].SetActive(false);
                }
                for (int i = 0; i < FindObjectsOfType<CoinScript>().Length; i++)
                {                    
                    FindObjectsOfType<CoinScript>()[i].EnMass();
                }
                Sound.clip = Success;
                Sound.Play();

            }
            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == false)
            { 
               
                for (int i = 0; i < FindObjectsOfType<CoinScript>().Length; i++)
                {
                    FindObjectsOfType<CoinScript>()[i].Return();
                    //FindObjectsOfType<CoinScript>()[i].Shade("HRDP/Lit");
                } 
                BalanceScales.SetActive(false);
                Collection.GetComponent<CollectionTableScript>().enabled = true;
                Collection.GetComponent<CollectionTableScript>().Blockcount = 0;
                Sound.clip = Disapointment;
                Sound.Play();

                gameObject.SetActive(false);

            }
        }
    }
}
