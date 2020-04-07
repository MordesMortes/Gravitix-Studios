/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>13th June 2017</date>
   <summary>Used to play the particle system when a particle system is selected when not in play mode</summary>*/

using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RadicalForge.Blockout
{
    [ExecuteInEditMode]
    public class BlockoutParticleHelper : MonoBehaviour
    {
        public bool ShouldPlay = false;
        private bool shouldPlayInternal = false;
#if UNITY_EDITOR
        private ParticleSystem targetParticleSystem;
#endif
        void OnEnable()
        {
#if UNITY_EDITOR
            targetParticleSystem = GetComponent<ParticleSystem>();
#endif
            if(Application.isPlaying)
                Destroy(this);
        }

        // Update is called once per frame
        void Update()
        {
#if UNITY_EDITOR
            if (Application.isEditor && !Application.isPlaying)
            {
                if (Selection.objects.ToList().Contains(this))
                    targetParticleSystem.Play(true);
                else if (ShouldPlay != shouldPlayInternal)
                {

                    if (ShouldPlay)
                        targetParticleSystem.Play(true);
                    else
                        targetParticleSystem.Stop(true);

                    shouldPlayInternal = ShouldPlay;

                }
            }
#endif
        }
    }

}