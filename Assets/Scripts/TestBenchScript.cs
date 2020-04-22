using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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
    int BlockCount = 0;//count of blocks
    TextMeshPro Display;//display for showing the riddle and number of brothers
    FinalScoreScript _blockCount;//realtime component for int for keeping track of blockcount
    
    private void Start()
    {
        
        _blockCount = FindObjectOfType<FinalScoreScript>();
        Display = GetComponentInChildren<TextMeshPro>();
        Display.SetText("one of these things is not like the others one of these things is not quite the same, all his brothers go in before him and then they never come out again");
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            BlockCount++;
            _blockCount.SetBlockCount(BlockCount);
            BlockCount = _blockCount._blockCount;
            
            if (BlockCount == 6)
            {
                BlockCount = 1;
                
            }
            Display.SetText("one of these things is not like the others one of these things is not quite the same, all his brothers go in before him and then they never come out again" + "\n" + BlockCount.ToString());

        }
        collision.gameObject.GetComponent<CoinScript>().Reveal();
            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == true && BlockCount == 5)//_blockCount._blockCount == 5)
            {
                
                gameObject.SetActive(false);
                RealtimeCubes.SetActive(true);
                BalanceScales.SetActive(false);
                RealtimeScales.SetActive(true);
                for (int i = 0; i < NonRealtimeCubes.Length; i++)
                {
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


               
                Sound.clip = Disapointment;
                Sound.Play();

                

            }

            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == false && BlockCount == 5)
            {
            Display.SetText("What's it like being a failure?");
            }

            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == true && BlockCount != 5)//_blockCount._blockCount != 5)
            {

               
                Sound.clip = Disapointment;
                Sound.Play();

                
            }


        
    }

    
}
