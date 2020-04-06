using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadicalForge.Gameplay
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	public class CameraForceDepthRender : MonoBehaviour
	{
		private Camera cam;

		void Start()
		{
			cam = GetComponent<Camera>();
			cam.depthTextureMode = cam.actualRenderingPath == RenderingPath.Forward ? DepthTextureMode.Depth : DepthTextureMode.DepthNormals;
		}
	}

}