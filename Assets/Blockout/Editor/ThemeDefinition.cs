/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>13th June 2017</date>
   <summary>Theme Definition File</summary>*/
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class ThemeDefinition : ScriptableObject
    {

        public Material FloorMaterial,
            WallMaterial,
            DynamicMaterial,
            TrimMaterial,
            FoliageMaterial,
            LeavesMaterial,
            WaterMateral,
            TriTrim,
            TriWalls,
            TriFloor,
            TriggerMaterial;

        public Material[] GetAllMaterials
        {
            get
            {
                return new Material[]
                {
                    DynamicMaterial, FloorMaterial, WallMaterial, TrimMaterial,
                    FoliageMaterial, LeavesMaterial, WaterMateral, TriTrim, TriWalls,
                    TriFloor, TriggerMaterial
                };
            }
        }

        public Material[] GetSortedUniqueMaterials
        {
            get
            {
                return new Material[]
                {
                    DynamicMaterial, FloorMaterial, WallMaterial, TrimMaterial,
                    FoliageMaterial
                };
            }
        }
    }


}