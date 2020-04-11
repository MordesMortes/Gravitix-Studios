using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBenchScript : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            
            collision.gameObject.GetComponent<CoinScript>().Reveal();
            if (collision.gameObject.GetComponent<CoinScript>().Reveal() == true)
            {
                gameObject.GetComponent<Rigidbody>().Sleep();
                gameObject.GetComponent<Rigidbody>().detectCollisions = false;
                for (int i = 0; i < FindObjectsOfType<CoinScript>().Length; i++)
                {
                    FindObjectsOfType<CoinScript>()[i].EnMass();
                }

            }
        }
    }
}
