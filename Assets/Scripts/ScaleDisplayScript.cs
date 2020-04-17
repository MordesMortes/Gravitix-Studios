using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ScaleDisplayScript : MonoBehaviour
{
    public GameObject LeftScale;//gameobject of the left scale
    public GameObject RightScale;//gameobject of the right scale
    private int FinalScore;//the winning weight
    public TextMeshPro Display;//display of the winning weight
    public TextMeshPro DisplayWeight;//display current weight
    public AudioSource AudioSource;//The sound that plays when one wins
    public AudioClip[] Dissapointment;//the sound that plays when one fails
    public AudioClip Success;//teh sweet sound of success
    int Current = 0;//how the player is currently disapointing (int for playing fail sounds)
    private void Awake()
    {
        FinalScore = Random.Range(1, 41);
      
    }
    private void Start()
    {        
        Display.SetText(FinalScore.ToString());      

    }

    public int Weigh()
    {
        int weight = LeftScale.GetComponent<LeftScaleScript>().CoinWeight - RightScale.GetComponent<RightScaleScript>().CoinWeight;//compares values of the scales
        if (weight <= 0)
        {
            weight = weight * -1;//corrects negatives into positives
        }
        DisplayWeight.SetText(weight.ToString());
        if (weight == FinalScore)
        {           
            Display.SetText("Yay you can add and subtract mommy and daddy must be so proud");
            DisplayWeight.SetText("The other person did all the math for you didn't they");
            AudioSource.clip = Success;
            AudioSource.Play();
        }
        if (weight != FinalScore)
        {
            AudioSource.clip = Dissapointment[Current];
            AudioSource.Play();
            Current++;
                if (Current > Dissapointment.Length)
            {
                Current = 0;
            }
        }
        
        return weight;
    }
}
