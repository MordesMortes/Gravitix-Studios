/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>Settings for each Blockout enabled scene</summary>*/

using System.Collections.Generic;
using UnityEngine;

namespace RadicalForge.Blockout
{

    [System.Serializable]
    public class AssetDefinition
    {
        public string assetName = "";
        public int assetQuantity = 0;
    }

    [System.Serializable]
    public class CameraAnchor
    {
        public string name = "";
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public float size = 60;
    }

    public class BlockoutSceneSettings : ScriptableObject
    {
        public string sceneName;
        public int currentTheme = 0;
        public int currentTexture = 0;
        
        public List<CameraAnchor> cameraAnchor;
        public List<AssetDefinition> assetDictionary;
    }

}