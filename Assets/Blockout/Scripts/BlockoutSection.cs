/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>05th July 2017</date>
   <summary>
        Attatched to root blockout objects in a blockout hierarchy
   </summary>*/

using UnityEngine;

namespace RadicalForge.Blockout
{

    public enum SectionID
    {
        Root,
        Floors,
        Walls,
        Trim,
        Dynamic,
        Foliage,
        Particles,
        Triggers,
        Cameras,
        Comments
    }

    public class BlockoutSection : MonoBehaviour
    {
        public SectionID Section;
    }

}