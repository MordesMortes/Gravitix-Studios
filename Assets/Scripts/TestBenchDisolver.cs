using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBenchDisolver : MonoBehaviour
{
    public GameObject Testbench;//the testbench to be dissolved
    public GameObject CollectionTable;//the collection table to be dissolved
    // Start is called before the first frame update
    void Start()
    {
        Testbench.SetActive(false);
        CollectionTable.SetActive(false);
    }

    
}
