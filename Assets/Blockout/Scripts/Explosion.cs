/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>01st September 2017</date>
   <summary>A generic explosion script that can spawn an explosion object</summary>*/

using UnityEngine;

namespace RadicalForge.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float affectedRadius = 5.0f;
        [SerializeField] private float force = 5.0f;
        [SerializeField] private float explodeUpModifier = 1.0f;
        [Space(10.0f)]
        [SerializeField] private GameObject explosionPrefab;

        private bool armed = false;

        [SerializeField] private bool DEBUG_VISABILITY = false;

        // Use this for initialization
        void Update()
        {
            if(armed && Input.GetKeyDown(KeyCode.F))
                Explode(true);
        }
        
        public void Explode(bool destroyOriginalObject = false)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, affectedRadius);

            foreach (Collider col in colliders)
            {
                Rigidbody rb = col.GetComponent<Rigidbody>();
                if(rb)
                    rb.AddExplosionForce(force, transform.position, affectedRadius, explodeUpModifier);
            }
            if (explosionPrefab)
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            if (destroyOriginalObject)
                Destroy(gameObject);
        }

        void OnDrawGizmos()
        {
            if (!DEBUG_VISABILITY) return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, affectedRadius);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                armed = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
                armed = false;
        }

        void OnGUI()
        {
            if (armed)
            {
                GUI.BeginGroup(new Rect(Screen.width / 2 - 70, Screen.height / 2 - 50, 140, 100));
                // All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.

                // We'll make a box so you can see where the group is on-screen.
                GUI.Label(new Rect(10, 40, 120, 30), "Press F To Explode");
                // End the group we started above. This is very important to remember!
                GUI.EndGroup();
            }
        }
    }

}