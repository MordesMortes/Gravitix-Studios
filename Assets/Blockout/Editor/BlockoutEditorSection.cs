using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RadicalForge.Blockout
{
	public class BlockoutHierarchy
	{
		public Transform root, floor, walls, trim, dynamic, foliage, triggers, particles, cameras, comments;
	}

	public abstract class BlockoutEditorSection : Object {
		
		public bool showSection = false;
		protected GUIContent sectionTitleLabel;
		protected BlockoutHierarchy blockoutHierarchy;
	    protected bool repaint = false;
        
        public bool Repaint { get { return repaint; } }

		public virtual void InitSection(string sectionName, BlockoutHierarchy hierarchy){
			sectionTitleLabel = new GUIContent(sectionName);
			blockoutHierarchy = hierarchy;
		}

		public virtual void Update(){}

		public abstract void DrawSection();

	}

}