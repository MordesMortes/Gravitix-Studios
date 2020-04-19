using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class CollectionTableScript : MonoBehaviour
{
    [HideInInspector]
    public int Blockcount = 0;
    public GameObject BalanceScales;
    public AudioSource Success;//success sound
    public AudioSource Dissapointment;//failure sound
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Blockcount++;
            collision.gameObject.GetComponent<RealtimeView>().enabled = false;
            collision.gameObject.GetComponent<RealtimeTransform>().enabled = false;
            //collision.gameObject.GetComponent<CoinScript>().Shade("Shader Graphs/Hologram");
        }
        if (Blockcount == 5)
        {
            BalanceScales.SetActive(true);
            //gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Shader Graphs/Hologram");

        }
        if (Blockcount == 0)
        {
            //gameObject.GetComponent<Renderer>().material.shader = Shader.Find("HDRP/Lit");
        }
        if (Blockcount < 0)
        {
            Blockcount = 0;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block") && Blockcount < 5)
        {
            Blockcount--; 
            collision.gameObject.GetComponent<RealtimeView>().enabled = true;
            collision.gameObject.GetComponent<RealtimeTransform>().enabled = true;
            //collision.gameObject.GetComponent<CoinScript>().Shade("HDRP/Lit");
        }
        
    }
}
