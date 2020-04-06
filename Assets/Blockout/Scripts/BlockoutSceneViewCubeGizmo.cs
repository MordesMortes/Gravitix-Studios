/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Draws a rotation locked comment box in the scene</summary>*/
   
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RadicalForge.Blockout
{
    [ExecuteInEditMode][SelectionBase]
    public class BlockoutSceneViewCubeGizmo : MonoBehaviour
    {
        public Color volumeColor;
		private Color previousColor;
        public Color outlineColor;

        [HideInInspector]public bool commentsActive = true;
		private bool previousCommentsActive = false;

		private Material gizmoMaterial;
		private List<Renderer> renderers = new List<Renderer>();

		private Vector3 previousScale;
        private List<Transform> corners = new List<Transform>();
        private List<Transform> lines = new List<Transform>();
        private Transform wireframeRoot;

        void OnEnable()
		{
		    wireframeRoot = transform.Find("Wireframe");
            renderers = wireframeRoot.GetComponentsInChildren<Renderer> ().ToList();
		    renderers.Add(GetComponent<Renderer>());

		    if (wireframeRoot.childCount < 19)
		    {
		        Debug.LogError("Area comment wireframe is not complete! Did you delete something?");
		        return;
		    }

		    corners.Clear();
            for (int i = 0; i < 8; ++i)
		        corners.Add(wireframeRoot.GetChild(i));
            lines.Clear();
		    for (int i = 8; i < 20; ++i)
		        lines.Add(wireframeRoot.GetChild(i));
		    previousCommentsActive = true;
		}

        void Start()
        {
            if (Application.isPlaying)
            {
                previousCommentsActive = false;
                commentsActive = false;
            }
        }

		public void Update()
		{
            transform.rotation = Quaternion.identity;
            

			if (previousCommentsActive != commentsActive) {
				renderers.ToList ().ForEach (x => x.enabled = commentsActive);
				previousCommentsActive = commentsActive;
			}
			if (previousColor != volumeColor) {
				renderers.ToList ().ForEach (x => {
					if(!x.gameObject.name.Contains("Corner"))
					{
						var tex = x.sharedMaterial.GetTexture("_Tex");
						Material[] mats = new Material[]{CommentMaterialSetupWithTexture(volumeColor, tex)};
						x.sharedMaterials = mats;
					}
					else
					{
                        Material[] mats = new Material[]{CommentMaterialSetup(volumeColor) };
						x.sharedMaterials = mats;
					}
				});
				previousColor = volumeColor;
			}

			if (previousScale != transform.lossyScale) {
				
				corners.ToList ().ForEach (x => {
					if(x != wireframeRoot)
					{
					    x.SetGlobalScale(Vector3.one * 2);
                    }
				});
			    var collider = GetComponent<BoxCollider>();

                Vector3[] boundPoint = new Vector3[8];

                boundPoint[0] = collider.bounds.min;
			    boundPoint[1] = collider.bounds.max;
			    boundPoint[2] = new Vector3(boundPoint[0].x, boundPoint[0].y, boundPoint[1].z);
			    boundPoint[3] = new Vector3(boundPoint[0].x, boundPoint[1].y, boundPoint[0].z);
			    boundPoint[4] = new Vector3(boundPoint[1].x, boundPoint[0].y, boundPoint[0].z);
			    boundPoint[5] = new Vector3(boundPoint[0].x, boundPoint[1].y, boundPoint[1].z);
			    boundPoint[6] = new Vector3(boundPoint[1].x, boundPoint[0].y, boundPoint[1].z);
			    boundPoint[7] = new Vector3(boundPoint[1].x, boundPoint[1].y, boundPoint[0].z);

			    for (int i = 0; i < 8; ++i)
			    {
			        corners[i].transform.position = boundPoint[i];
			        lines[i].transform.position = corners[i].transform.position;
			    }

			    lines[8].transform.position = corners[4].transform.position;
			    lines[9].transform.position = corners[6].transform.position;
			    lines[10].transform.position = corners[6].transform.position;
			    lines[11].transform.position = corners[5].transform.position;

                lines.ToList().ForEach(x => {
			        if (x != wireframeRoot && !corners.Contains(x))
			        {
                        x.SetGlobalScale(Vector3.one);
			            Vector3 localScale = x.localScale;
			            localScale.x = 0.995f;
			            x.localScale = localScale;
			        }
			    });

                previousScale = transform.lossyScale;
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
        Material CommentMaterialSetupWithTexture(Color col , Texture tex)
		{
			Material mat = new Material(Shader.Find("Blockout/Blockout_Shader_Comment"));
			mat.SetColor("_Color",col);
			mat.SetTexture("_Tex", tex);
			return mat;
		}

		/// <summary>
		/// Sets up a new materail for an area comment
		/// </summary>
		/// <returns>The material setup.</returns>
		/// <param name="col">Color</param>
		/// <param name="tex">Texture</param>
		Material CommentMaterialSetup(Color col)
		{
			Material mat = new Material(Shader.Find("Blockout/Area_Comment_Wireframe"));
			mat.SetColor("_Color",col);
			return mat;
		}

    }

    // TODO: Move to another script
	public static class RuntimeExtensions
	{
		/// <summary>
		/// Sets the global scale of a transform.
		/// </summary>
		/// <param name="transform">The target transform.</param>
		/// <param name="globalScale">The new global scale.</param>
		public static void SetGlobalScale(this Transform transform, Vector3 globalScale)
		{
			transform.localScale = Vector3.one;
			transform.localScale = new Vector3(globalScale.x / transform.lossyScale.x, globalScale.y / transform.lossyScale.y, globalScale.z / transform.lossyScale.z);
		}
	}

}