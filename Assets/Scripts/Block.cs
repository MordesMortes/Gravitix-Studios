using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{

    private MeshRenderer myRend;

    private void Start()
    {
        myRend = GetComponent<MeshRenderer>();
        myRend.material.color = Random.ColorHSV();
    }
}
