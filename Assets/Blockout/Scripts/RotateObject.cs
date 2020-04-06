/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>26th June 2017</date>
   <summary>Rotates an object</summary>*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadicalForge.Gameplay
{


    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotationAxis = Vector3.up;

        public float angle = 0.25f;
        

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(rotationAxis, angle);
        }
    }

}