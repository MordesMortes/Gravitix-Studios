/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>06th July 2017</date>
   <summary>A scriptable wizzard to make assets suitable for the Blockout system</summary>*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{


    public class CreateBlockoutPrefabWizzard : ScriptableWizard
    {
        public GameObject[] SelectedObjects;
        public SectionID[] Sections;

        [MenuItem("Window/Blockout/Create Blockout Assets", false, 13)]
        static void CreateWizard()
        {
            CreateBlockoutPrefabWizzard wizz =
                ScriptableWizard.DisplayWizard<CreateBlockoutPrefabWizzard>("Create Blockout Asset", "Create");
            wizz.SelectedObjects = Selection.gameObjects;
            wizz.Sections = new SectionID[wizz.SelectedObjects.Length];
        }

        void OnWizardCreate()
        {
            if (!Directory.Exists("Assets/Blockouts/NewPrefabs"))
                Directory.CreateDirectory("Assets/Blockouts/NewPrefabs");

            List<string> names = new List<string>();
            List<Vector3> positions = new List<Vector3>();
            List<Quaternion> rotations = new List<Quaternion>();
            List<Vector3> scales = new List<Vector3>();
            List<GameObject> toSelect = new List<GameObject>();

            for (int i = 0; i < SelectedObjects.Length; ++i)
            {
                names.Add(SelectedObjects[i].name);
               
                PrefabUtility.CreatePrefab("Assets/Blockouts/NewPrefabs/" + SelectedObjects[i].name + ".prefab", SelectedObjects[i],
                    ReplacePrefabOptions.ConnectToPrefab);
                positions.Add(SelectedObjects[i].transform.position);
                rotations.Add(SelectedObjects[i].transform.rotation);
                scales.Add(SelectedObjects[i].transform.localScale);
                DestroyImmediate(SelectedObjects[i]);
                GameObject toSpawn = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Blockouts/NewPrefabs/" + names[i]+".prefab");
                toSpawn.AddComponent<BlockoutHelper>().initialBlockoutSection = Sections[i];
                if (Sections[i] == SectionID.Particles)
                {
                    toSpawn.AddComponent<BlockoutParticleHelper>().ShouldPlay = true;
                }
                GameObject spawned = PrefabUtility.InstantiatePrefab(toSpawn) as GameObject;
                spawned.transform.position = positions[i];
                spawned.transform.rotation = rotations[i];
                spawned.transform.localScale = scales[i];
                toSelect.Add(spawned);
            }
            


            EditorUtility.DisplayDialog("Blockout Prefab Creator",
                "Created " + SelectedObjects.Length + " New Blockout Assets", "OK");

            Selection.objects = toSelect.ToArray();
            Close();
        }

        void OnGUI()
        {
            GUILayout.BeginVertical();

            for (int i = 0; i < SelectedObjects.Length; ++i)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);

                SelectedObjects[i] = (GameObject) EditorGUILayout.ObjectField(SelectedObjects[i], typeof(GameObject), true);
                Sections[i] = (SectionID)EditorGUILayout.EnumPopup(Sections[i], GUILayout.MaxWidth(150));

                GUILayout.Space(10);
                GUILayout.EndHorizontal();
            }

            GUILayout.FlexibleSpace();
            
            
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(10);

                if (GUILayout.Button("Add Selected GameObejcts"))
                {
                    var current = SelectedObjects.ToList();
                    current.AddRange(Selection.gameObjects);
                    SelectedObjects=current.Distinct().ToArray();

                    var currSections = Sections.ToList();
                    for(int i = 0; i < Selection.gameObjects.Length; ++i)
                        currSections.Add(SectionID.Root);
                    Sections = currSections.ToArray();
                }
                if (GUILayout.Button("Create Blockout Prefabs"))
                {
                    OnWizardCreate();
                }

                GUILayout.Space(10);
            }

            GUILayout.Space(10);
            GUILayout.EndVertical();
        }

        void OnWizardUpdate()
        {
            helpString = "Please set the color of the light!";
        }

    }
}