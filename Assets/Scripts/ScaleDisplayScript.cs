using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleDisplayScript : MonoBehaviour
{
    public GameObject LeftScale;//gameobject of the left scale
    public GameObject RightScale;//gameobject of the right scale
    private int FinalScore;
    private void Start()
    {
        FinalScore = Random.Range(1, 41);   
    }

    public int Weigh()
    {
        int weight = LeftScale.GetComponent<LeftScaleScript>().CoinWeight - RightScale.GetComponent<RightScaleScript>().CoinWeight;//compares values of the scales
        if (weight <= 0)
        {
            weight = weight * -1;//corrects negatives into positives
        }
        Debug.Log("the weight is " + weight.ToString() + " it is not " + FinalScore.ToString());//testing message remove on cleanup
        if (weight == FinalScore)
        {
            Debug.Log("you have matched the score weight of " + FinalScore.ToString());//testing message remove on cleanup

        }
        
        return weight;
    }
}
