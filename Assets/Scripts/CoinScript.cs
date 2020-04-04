using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviour
    
{
    [Range(1f,30f)]
    public int CoinWeight;//weight of this coin for the second puzzle
    public bool IsFake;//is this a real or fake coin for the first puzzle
    Text CoinValue;//coin value will match the weight
    private void Awake()
    {
        CoinValue = GetComponent<Text>();
    }
    public void EnScale(float scale)
    {
        gameObject.transform.localScale = gameObject.transform.localScale * scale;
    }
    public void Reveal()
    {
        CoinValue.text = CoinWeight.ToString();
    }

    
}
