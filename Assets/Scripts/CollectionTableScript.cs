using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using TMPro;

public class CollectionTableScript : MonoBehaviour
{
    [HideInInspector]
    public int Blockcount = 0;
    public GameObject BalanceScales;
    public TextMeshPro Message;
    public Material Hologram;//the hologram material to add to the object
    public Material DefaultMaterial;//the default material
    public Material DefaultTransparent;//the default material only transparent
    public Material[] materials;
    public AudioSource audioSource;
    public AudioClip Success;//success sound
    public AudioClip[] Dissapointment;//failure sound
    private MeshRenderer myRend;
    private void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        Message = GetComponentInChildren<TextMeshPro>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Blockcount++;
            //collision.gameObject.GetComponent<RealtimeView>().enabled = false;
            //collision.gameObject.GetComponent<RealtimeTransform>().enabled = false;
            collision.gameObject.GetComponent<CoinScript>().Shade();//remove if hologram can't be fixed
            
        }
        if (Blockcount == 5)
        {
            BalanceScales.SetActive(true);
            Message.text = "Oh what's that behind you?";
            collision.gameObject.GetComponent<CoinScript>().Shade();
            Shade();

            audioSource.clip = Success;
            audioSource.Play();

        }
        if (Blockcount == 0)
        {
            UnShade();//remove if not using hologram
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
            //collision.gameObject.GetComponent<RealtimeView>().enabled = true;
            //collision.gameObject.GetComponent<RealtimeTransform>().enabled = true;
            collision.gameObject.GetComponent<CoinScript>().UnShade();//remove if not using hologram
            
        }
        
    }
    public void Shade()
        {
            materials[0] = DefaultTransparent;
            materials[1] = Hologram;
            myRend.materials = materials;
        }
    public void UnShade()
    {
        materials[0] = DefaultMaterial;
        materials[1] = DefaultMaterial;
        myRend.materials = materials;
    }
}
