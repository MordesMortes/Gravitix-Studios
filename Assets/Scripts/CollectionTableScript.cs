using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class CollectionTableScript : MonoBehaviour
{
    [HideInInspector]
    public int Blockcount;
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
            collision.gameObject.GetComponent<CoinScript>().Shade("Hologram");
        }
        if (Blockcount == 5)
        {
            BalanceScales.SetActive(true);

        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Blockcount--; 
            collision.gameObject.GetComponent<RealtimeView>().enabled = true;
            collision.gameObject.GetComponent<RealtimeTransform>().enabled = true;
            collision.gameObject.GetComponent<CoinScript>().Shade("lit");
        }
    }
}
