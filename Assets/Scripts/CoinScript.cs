using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinScript : MonoBehaviour
    
{
    [Range(1f,30f)]
    public int CoinWeight;//weight of this coin for the second puzzle
    public bool IsFake;//is this a real or fake coin for the first puzzle
    Text CoinValue;//coin value will match the weight
    bool SizeToggle = false; //toggle for if the object is getting resized
    [HideInInspector]
    public GameObject CollidedObject;//object that has collided with the coin that is tagged with 
    private void Start()
    {
        CoinValue = GetComponent<Text>();
        if (IsFake)
        {
            gameObject.GetComponent<Rigidbody>().mass = gameObject.GetComponent<Rigidbody>().mass + 0.5f;
        }
        
    }
    public void EnScale(float scale)
    {
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
    }
    public bool Reveal()
    {
        
        if (IsFake == false)
        {
            
            SceneManager.LoadScene("testing");
            return false;
        }
        if (IsFake == true)
        {
            Destroy(gameObject, 3f);
            return true;
        }
        else 
        { 
            return false; 
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "EnScale"  && SizeToggle == false)
        {
            CollidedObject = collision.gameObject;
            GetComponent<CoinUpdaterScript>().enabled = true;
            SizeToggle = true;
        }
        if (collision.gameObject.tag == "EnScale" && SizeToggle)
        {
            GetComponent<CoinUpdaterScript>().enabled = false;
            SizeToggle = false;
        }

    }
     public void EnMass()
    {
        GetComponent<Rigidbody>().mass = CoinWeight;
    }
}
