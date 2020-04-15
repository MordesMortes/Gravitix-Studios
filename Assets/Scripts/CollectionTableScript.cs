using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionTableScript : MonoBehaviour
{
    int Blockcount;
    public GameObject BalanceScales;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Block"))
        {
            Blockcount++;
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
        }
    }
}
