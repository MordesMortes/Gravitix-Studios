/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>13th June 2017</date>
   <summary>Determines if the trigger is visible when the game is running</summary>*/

using System;
using UnityEngine;
using UnityEngine.Events;

namespace RadicalForge.Blockout
{

    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Renderer))]
    public class BlockoutTrigger : MonoBehaviour
    {

        Renderer targetRenderer;

        [HideInInspector] public bool visibleInGame = false;

        [Serializable]
        public class OnTriggerEvent : UnityEvent<Collider> { }

        [SerializeField] private OnTriggerEvent  m_OnTriggerEnter = new OnTriggerEvent();
        [SerializeField] private OnTriggerEvent m_OnTriggerStay = new OnTriggerEvent();
        [SerializeField] private OnTriggerEvent m_OnTriggerExit = new OnTriggerEvent();

        public OnTriggerEvent onTriggerEnter
        {
            get { return m_OnTriggerEnter; }
            set { m_OnTriggerEnter = value; }
        }

        public OnTriggerEvent onTriggerStay
        {
            get { return m_OnTriggerStay; }
            set { m_OnTriggerStay = value; }
        }

        public OnTriggerEvent onTriggerExit
        {
            get { return m_OnTriggerExit; }
            set { m_OnTriggerExit = value; }
        }

        // Use this for initialization
        void Start()
        {
            targetRenderer = GetComponent<Renderer>();
            if (targetRenderer)
                targetRenderer.enabled = visibleInGame;
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            m_OnTriggerEnter.Invoke(other);
        }

        void OnTriggerStay(Collider other)
        {
            m_OnTriggerStay.Invoke(other);
        }

        void OnTriggerExit(Collider other)
        {
            m_OnTriggerExit.Invoke(other);
        }

    }

}