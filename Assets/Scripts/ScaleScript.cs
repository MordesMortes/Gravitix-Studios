using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleScript : MonoBehaviour

{
    public bool LeftScale;//indicates this is the left scale
    public bool RightScale;//indicates this is the right scale
    Text Display;// The value that displays on the cube
    [HideInInspector]
    public int CoinWeight;
    MeshRenderer MyRend;//meshrenderer to change colours for testing
    private void Start()
    {
        MyRend = gameObject.GetComponent<MeshRenderer>();
        Display = gameObject.GetComponent<Text>();
        CoinWeight = 0;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (CoinWeight == 0)

        {
            CoinWeight = collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            Display.text = CoinWeight.ToString();
            
        }
        if (CoinWeight !=0)
        {
            CoinWeight = CoinWeight + collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            Display.text = CoinWeight.ToString();
        }
        if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        {
            MyRend.material.color = Random.ColorHSV();
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (CoinWeight != 0)
        {
            CoinWeight = CoinWeight - collision.gameObject.GetComponent<CoinScript>().CoinWeight;
            Display.text = CoinWeight.ToString();
        }
        if (collision.gameObject.GetComponent<CoinScript>().IsFake)
        {
            MyRend.material.color = Color.grey;
        }
    }
    public int ScaleValue()
    {
        if (LeftScale)
        {
            Debug.Log(CoinWeight.ToString());
            return CoinWeight;
        }
        if (RightScale)
        {
            CoinWeight = CoinWeight  * -1;
            Debug.Log(CoinWeight.ToString());
            return CoinWeight;
        }
        else
        {
            Debug.Log("you need to set the sides of the scales");
            return 0;
        }
    }
}
