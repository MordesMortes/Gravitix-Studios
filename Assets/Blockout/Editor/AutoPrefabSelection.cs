/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>05th July 2017</date>
   <summary>
        Auto Prefab selected gameobjects using name as prefab name. Will go to a predefined target folder
   </summary>*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RadicalForge.Blockout
{

    public static class AutoPrefabSelection
    {
        public static void PrefabSelection()
        {
            List<GameObject> selection = Selection.gameObjects.ToList();
            if (selection.Count == 0)
                Debug.LogError("There are no selected objects!");
            else
            {
                if (!Directory.Exists("Assets/Blockout/NewPrefabs"))
                    Directory.CreateDirectory("Assets/Blockout/NewPrefabs");

                foreach (var o in selection)
                {
                    bool isChildInSelection = false;
                    foreach (var s in selection)
                    {
                        if (s.GetComponentsInChildren<Transform>().Select(x => x.gameObject).ToList().Contains(o) &&
                            s != o)
                            isChildInSelection = true;
                    }
                    if (!isChildInSelection)
                    {
                        var prefab = PrefabUtility.CreatePrefab("Assets/Blockout/NewPrefabs/" + o.name + ".prefab", o,
                            ReplacePrefabOptions.ConnectToPrefab);
                        EditorGUIUtility.PingObject(prefab);
                    }
                }
            }
        }
    }
    

}