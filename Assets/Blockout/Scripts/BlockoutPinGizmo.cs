/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>05th July 2017</date>
   <summary>Blockout Pin Styling Behaviour</summary>*/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace RadicalForge.Blockout
{
    [ExecuteInEditMode][SelectionBase]
    public class BlockoutPinGizmo : MonoBehaviour
    {

        public Color volumeColor;
        private Color previousColor;

        [HideInInspector] public bool commentsActive = true;
        private bool previousCommentsActive = false;

        private Material gizmoMaterial;
        private List<Renderer> renderers = new List<Renderer>();
        public bool SelectAfterFrame = false;
        public Vector3 localChildPos = Vector3.zero;

        void OnEnable()
        {
            renderers.AddRange(GetComponentsInChildren<Renderer>());
        }

        void Start()
        {
            localChildPos = transform.GetChild(0).localPosition;
            if (Application.isPlaying)
            {
                previousCommentsActive = false;
                commentsActive = false;
            }
#if UNITY_EDITOR
            else
            {
                if (SelectAfterFrame)
                    Invoke("SelectEditFrame", 0.55f);
            }
#endif
        }

#if UNITY_EDITOR
        void SelectEditFrame()
        {
            Selection.activeGameObject = gameObject;
        }
#endif
        public void Update()
        {
            if (localChildPos != Vector3.zero)
                transform.GetChild(0).localPosition = localChildPos;
            else
                localChildPos = transform.GetChild(0).localPosition;
#if UNITY_EDITOR
            if (Selection.activeGameObject == transform.GetChild(0).gameObject)
                Selection.activeGameObject = gameObject;
#endif
            if (previousCommentsActive != commentsActive)
            {
                renderers.ToList().ForEach(x => x.enabled = commentsActive);
                previousCommentsActive = commentsActive;
            }
            if (previousColor != volumeColor)
            {
                renderers.ToList().ForEach(x =>
                {
                    Material[] mats = { CommentMaterialSetup(volumeColor, "Blockout/Pin_Backfacing"), CommentMaterialSetup(volumeColor, "Blockout/Blockout_Pin_Shader") };
                    x.sharedMaterials = mats;
                });
                previousColor = volumeColor;
            }
            
        }

        public void SetActive(bool value)
        {
            commentsActive = value;

            renderers.ToList().ForEach(x => x.enabled = commentsActive);
            previousCommentsActive = commentsActive;
        }
        
        /// <summary>
        /// Sets up a new materail for an area comment
        /// </summary>
        /// <returns>The material setup.</returns>
        /// <param name="col">Color</param>
        /// <param name="tex">Texture</param>
        Material CommentMaterialSetup(Color col, string name)
        {
            Material mat = new Material(Shader.Find(name));
            mat.SetColor("_Color", col);
            if(mat.HasProperty("_Depth_Blend"))
                mat.SetFloat("_Depth_Blend", 0.74f);
            return mat;
            
        }

    }
}