/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>01st September 2017</date>
   <summary>A demo collectable object container. 
   Attatch to an object that isnt the triggered object or a collectable</summary>*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadicalForge.Gameplay
{

    public class CollectionTrigger : MonoBehaviour
    {

        [SerializeField] private List<GameObject> targets = new   List<GameObject>();

        [SerializeField] private GameObject triggeredObject;

        void Start()
        {
            triggeredObject.SetActive(false);
        }

        public void Collect(GameObject obj)
        {
            if(targets.Contains(obj))
                targets.Remove(obj);
            targets.TrimExcess();
            if(targets.Count == 0)
                triggeredObject.SetActive(true);
        }
    }

}