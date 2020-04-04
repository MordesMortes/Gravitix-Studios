/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>14th October 2017</date>
   <summary>Blockout static helper functions</summary>*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using Object = UnityEngine.Object;

namespace RadicalForge.Blockout
{
    public static class BlockoutStaticFunctions
    {
        public static Vector3 Abs(this Vector3 target)
        {
            target.x = Mathf.Abs(target.x);
            target.y = Mathf.Abs(target.y);
            target.z = Mathf.Abs(target.z);

            return target;
        }

        /// <summary>
        /// Snaps the position of the selected transforms
        /// </summary>
        public static void SnapPositionSelection()
        {
            float snap = EditorPrefs.HasKey("Blockout::Snap") ? EditorPrefs.GetFloat("Blockout::Snap") : 0.25f;
            Undo.RecordObjects(Selection.transforms, "Snap");
            foreach (var transform in Selection.transforms)
            {
                var p = transform.transform.position;
                p.x = Round(p.x, snap);
                p.y = Round(p.y, snap);
                p.z = Round(p.z, snap);
                transform.transform.position = p;
            }
        }

        /// <summary>
        /// Snaps the scales of the selected transforms
        /// </summary>
        public static void SnapScaleSelection()
        {
            float snap = EditorPrefs.HasKey("Blockout::Snap") ? EditorPrefs.GetFloat("Blockout::Snap") : 0.25f;
            Undo.RecordObjects(Selection.transforms, "Snap");
            foreach (var transform in Selection.transforms)
            {
                var s = transform.transform.lossyScale;
                s.x = Round(s.x, snap);
                s.y = Round(s.y, snap);
                s.z = Round(s.z, snap);
                if (snap > s.x && s.x >= 0)
                    s.x = snap;
                if (0 > s.x && s.x >= -snap)
                    s.x = -snap;

                if (snap > s.y && s.y >= 0)
                    s.y = snap;
                if (0 > s.y && s.y >= -snap)
                    s.y = -snap;

                if (snap > s.z && s.z >= 0)
                    s.z = snap;
                if (0 > s.z && s.z >= -snap)
                    s.z = -snap;

                transform.SetGlobalScale(s);
            }
        }

        /// Snap an object to another object in a global direction
        /// <param name="direction"> The global vcttor to snap along</param>
        /// <param name="axis"></param>
        public static void Snap(Vector3 direction, BlockoutAxis axis, bool forceGlobal = false)
        {
            // Get the selected gameobjects and raycast all of them in the snap direction
            // If it hits someting then set its new position to be the snap point offset
            // by how large the obects bounds is
            var sel = Selection.transforms;
            Undo.RecordObjects(sel, "Snap Objects");
            for (var i = 0; i < sel.Length; ++i)
            {
                var childColliders = sel[i].GetComponentsInChildren<Collider>().ToList();
                var rend = sel[i].GetComponent<Renderer>();
                if (rend)
                {
                    var ray = new Ray(rend.bounds.center, direction);

                    var pivotOffset = sel[i].transform.position - rend.bounds.center;

                    var allHits = Physics.RaycastAll(ray, 500.0f, Physics.DefaultRaycastLayers);
                    if (allHits.Length > 0)
                        for (var h = 0; h < allHits.Length; ++h)
                        {
                            if (childColliders.Contains(allHits[h].collider))
                                continue;

                            var targetPoint = allHits[i].point;

                            sel[i].position = targetPoint + pivotOffset;
                            sel[i].position += -direction.normalized *
                                               (rend.bounds.size[(int) axis] / 2);
                            break;
                        }
                }
            }
            SceneView.lastActiveSceneView.FrameSelected();
        }

        /// <summary>
        ///     Rounds the specified float. Used in snapping
        /// </summary>
        /// <param name="input">The float to round.</param>
        /// <param name="snapValue">The amount to round to</param>
        /// <returns></returns>
        private static float Round(float input, float snapValue = 0.25f)
        {
            return snapValue * Mathf.Round(input / snapValue);
        }

        /// <summary>
        /// Open URL to submit feedback lke html mailto
        /// </summary>
        [MenuItem("Window/Blockout/Submit Feedback or Bug Report", false, 57)]
        public static void SubmitBugReport()
        {
            Application.OpenURL("mailto:support@radicalforge.com?subject=Blockout");
        }

        /// <summary>
        /// Open URL to show online docs
        /// </summary>
        [MenuItem("Window/Blockout/Documentation", false, 55)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("http://blockout.radicalforge.com/");
        }

        /// <summary>
        /// Open URL to show online docs
        /// </summary>
        [MenuItem("Window/Blockout/Tutorials", false, 56)]
        public static void OpenTutorials()
        {
            Application.OpenURL("https://www.youtube.com/playlist?list=PLpMc62-aOz2wt7DG-R6Dm87V1pyj9SCFH");
        }

        public static void TryLoadSceneDefinitions()
        {
            var sceneDefs = Resources.LoadAll<BlockoutSceneSettings>("").ToArray();
            BlockoutEditorSettings.SceneDefinitions = sceneDefs;
            BlockoutEditorSettings.CurrentSettingIDX = -1;

            for (var i = 0; i < sceneDefs.Length; ++i)
            {
                if (SceneManager.GetActiveScene().name == sceneDefs[i].sceneName)
                {
                    var target = i;
                    LoadSceneDefinition(target);
                    break;
                }
            }

            if (BlockoutEditorSettings.CurrentSettingIDX < 0)
            {
                var newSetting = ScriptableObject.CreateInstance<BlockoutSceneSettings>();
                newSetting.sceneName = SceneManager.GetActiveScene().name;
                newSetting.assetDictionary = new List<AssetDefinition>();
                newSetting.cameraAnchor = new List<CameraAnchor>();

                Directory.CreateDirectory("Assets/Blockout/Editor/Resources/Blockout/SceneDefinitions");
                AssetDatabase.CreateAsset(newSetting,
                                          "Assets/Blockout/Editor/Resources/Blockout/SceneDefinitions/" +
                                          typeof(BlockoutSceneSettings).Name + "_" +
                                          newSetting.sceneName + ".asset");
                var list = BlockoutEditorSettings.SceneDefinitions.ToList();
                list.Add(newSetting);
                BlockoutEditorSettings.SceneDefinitions = list.ToArray();
                BlockoutEditorSettings.CurrentSettingIDX = BlockoutEditorSettings.SceneDefinitions.ToList().FindIndex(x => x.sceneName == newSetting.name);
                LoadSceneDefinition(BlockoutEditorSettings.CurrentSettingIDX);
            }
            
        }


        /// <summary>
        /// Load a current scene definition thats already been globally loaded
        /// </summary>
        /// <param name="setting">The scene setting ID</param>
        private static void LoadSceneDefinition(int setting)
        {
            BlockoutEditorSettings.CurrentSettingIDX = setting;

            if (BlockoutEditorSettings.CurrentSettingIDX > BlockoutEditorSettings.SceneDefinitions.Length)
            {
                BlockoutEditorSettings.CurrentSettingIDX = -1;
                Debug.LogError(
                               "BLOCKOUT :: (INTERNAL) Unable to load scene definition. Please Close and reopen any Blockout window");
                return;
            }
            if (BlockoutEditorSettings.CurrentSettingIDX < 0)
                return;
            BlockoutEditorSettings.CurrentSceneSetting = BlockoutEditorSettings.SceneDefinitions[setting];
            BlockoutEditorSettings.CurrentMaterialTheme = BlockoutEditorSettings.SceneDefinitions[setting].currentTheme;
            if (BlockoutEditorSettings.CurrentMaterialTheme < 0)
                BlockoutEditorSettings.CurrentPallet = PalletType.User;
            else
                BlockoutEditorSettings.CurrentPallet = PalletType.Preset;

            BlockoutEditorSettings.CurrentGirdTexture = BlockoutEditorSettings.CurrentSceneSetting.currentTexture;
        }

        /// <summary>
        /// Gets a spawn poisition in front of the scene camera while not going
        /// behind an object that is less than 10 units from the scene camera
        /// </summary>
        /// <returns>A position in world space</returns>
        public static Vector3 GetSceneViewSpawnPosition()
        {
            var spawnPos = SceneView.lastActiveSceneView.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));

            Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10f))
                spawnPos = hit.point;

            return spawnPos;
        }

        /// <summary>
        /// Apply the current blockout material theme
        /// </summary>
        public static void ApplyCurrentTheme()
        {
            if (BlockoutEditorSettings.CurrentPallet == PalletType.Preset)
            {
                if (BlockoutEditorSettings.CurrentMaterialTheme < 0 || BlockoutEditorSettings.CurrentMaterialTheme >=
                    EUIResourceManager.Instance.BlockoutThemes.Length)
                    BlockoutEditorSettings.CurrentMaterialTheme = 0;
                ApplyTheme(EUIResourceManager.Instance.BlockoutThemes[BlockoutEditorSettings.CurrentMaterialTheme]);
            }
            else
            {
                ApplyTheme(EUIResourceManager.Instance.UserTheme);
            }
        }

        /// <summary>
        ///     Applies the material theme color pallet and texture to all blockout objects.
        /// </summary>
        /// <param name="theme">Target Theme.</param>
        public static void ApplyTheme(ThemeDefinition theme)
        {
           // using (new UndoScope("Change Blockout Material"))
            {


                Undo.RecordObject(BlockoutEditorSettings.CurrentSceneSetting, "");
                BlockoutEditorSettings.CurrentSceneSetting.currentTheme =
                    BlockoutEditorSettings.CurrentPallet == PalletType.Preset
                        ? BlockoutEditorSettings.CurrentMaterialTheme
                        : -1;

                EditorUtility.SetDirty(BlockoutEditorSettings.CurrentSceneSetting);
                AssetDatabase.SaveAssets();

                EditorUtility.DisplayProgressBar("Blockout", "Collecting Sections In Active Scene", 0f);
                var allSections = FindObjectsOfTypeInActiveScene<BlockoutSection>();
                
                EditorUtility.DisplayProgressBar("Blockout", "Applying New Theme", 0.1f);
                for (var i = 0; i < allSections.Length; i++)
                {
                    BlockoutSection section = allSections[i];
                    EditorUtility.DisplayProgressBar("Blockout", "Applying New Theme", i / (float)allSections.Length + 5);
                    switch (section.Section)
                    {
                        case SectionID.Floors:
                            ApplyNewMaterialScheme(theme.FloorMaterial, section.gameObject);
                            break;
                        case SectionID.Walls:
                            ApplyNewMaterialScheme(theme.WallMaterial, section.gameObject);
                            break;
                        case SectionID.Trim:
                            ApplyNewMaterialScheme(theme.TrimMaterial, section.gameObject);
                            break;
                        case SectionID.Dynamic:
                            ApplyNewMaterialScheme(theme.DynamicMaterial, section.gameObject);
                            break;
                        case SectionID.Foliage:
                            ApplyNewMaterialScheme(theme.FoliageMaterial, section.gameObject);
                            break;
                        case SectionID.Root:
                        case SectionID.Particles:
                        case SectionID.Triggers:
                        case SectionID.Cameras:
                        case SectionID.Comments:
                            break;
                    }
                }


                /*var floorObjs = allSections.Where(x => x.Section == SectionID.Floors).Select(x => x.gameObject);
                foreach (var floor in floorObjs)
                    ApplyNewMaterialScheme(theme.FloorMaterial, floor);

                var wallObjs = allSections.Where(x => x.Section == SectionID.Walls).Select(x => x.gameObject);
                foreach (var wall in wallObjs)
                    ApplyNewMaterialScheme(theme.WallMaterial, wall);

                var dynObjs = allSections.Where(x => x.Section == SectionID.Dynamic).Select(x => x.gameObject);
                foreach (var dyn in dynObjs)
                    ApplyNewMaterialScheme(theme.DynamicMaterial, dyn);

                var foliageObjs = allSections.Where(x => x.Section == SectionID.Foliage).Select(x => x.gameObject);
                foreach (var foliage in foliageObjs)
                    ApplyNewMaterialScheme(theme.FoliageMaterial, foliage);

                var trimObjs = allSections.Where(x => x.Section == SectionID.Trim).Select(x => x.gameObject);
                foreach (var trim in trimObjs)
                    ApplyNewMaterialScheme(theme.TrimMaterial, trim);*/

                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - Water", allSections.Length / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.WaterMateral,
                    FindObjectsOfTypeInActiveScene<Transform>()
                        .Where(x => x.name.Contains("Water") && x.gameObject.activeInHierarchy)
                        .Select(x => x.gameObject)
                        .ToArray());
                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - Leaves", allSections.Length + 1 / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.LeavesMaterial,
                    FindObjectsOfTypeInActiveScene<Transform>()
                        .Where(x => x.name.Contains("Leaves") && x.gameObject.activeInHierarchy)
                        .Select(x => x.gameObject)
                        .ToArray());
                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - Triggers", allSections.Length + 2 / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.TriggerMaterial,
                    FindObjectsOfTypeInActiveScene<BlockoutTrigger>().Select(x => x.gameObject).ToArray());

                var wallTargets = new List<GameObject>();
                var floorTargets = new List<GameObject>();
                var trimTargets = new List<GameObject>();

                var gameObjectsWithCorrentBlockoutSection = FindObjectsOfTypeInActiveScene<Transform>().ToList().Where(
                    x =>
                    {
                        if (x.parent == null)
                            return false;
                        var bs = x.GetComponentInParent<BlockoutSection>();
                        if (!bs)
                            return false;

                        return bs.Section == SectionID.Walls || bs.Section == SectionID.Floors ||
                               bs.Section == SectionID.Trim;
                    }).Select(x => x.gameObject).ToList();

                foreach (var go in gameObjectsWithCorrentBlockoutSection)
                {
                    var targetSection = go.transform.GetComponentInParent<BlockoutSection>();
                    if (!targetSection) continue;
                    if (!go.name.Contains("(Tri-Planar)")) continue;
                    switch (targetSection.Section)
                    {
                        case SectionID.Walls:
                            wallTargets.Add(go);
                            break;
                        case SectionID.Floors:
                            floorTargets.Add(go);
                            break;
                        case SectionID.Trim:
                            trimTargets.Add(go);
                            break;
                    }
                }

                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - TriWalls", allSections.Length + 3 / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.TriWalls, wallTargets.ToArray());
                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - TriFloor", allSections.Length + 4 / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.TriFloor, floorTargets.ToArray());
                EditorUtility.DisplayProgressBar("Blockout", "Applying Theme To Group - Tri-Trim", allSections.Length + 5 / (float)allSections.Length + 5);
                ApplyMaterialToGroup(theme.TriTrim, trimTargets.ToArray());

                var blockoutRoots = SceneManager.GetActiveScene().GetRootGameObjects()
                    .Where(x => x.GetComponent<BlockoutSection>())
                    .Where(x => x.GetComponent<BlockoutSection>().Section == SectionID.Root).Select(x => x.transform);

                foreach (Transform t in blockoutRoots)
                    ApplyTextureIncChildren(
                        EUIResourceManager.Instance.GridTextures[BlockoutEditorSettings.CurrentGirdTexture], t);
                EditorUtility.ClearProgressBar();

            }
        }

        /// <summary>
        ///     Applies the new material scheme without undo.
        /// </summary>
        /// <param name="targetMaterial">The target material.</param>
        /// <param name="current">The current gameobject.</param>
        public static void ApplyNewMaterialSchemeWithoutUndo(Material targetMaterial, GameObject current)
        {
            var renderers = current.GetComponentsInChildren<Renderer>();


            if (renderers != null)
                foreach (var r in renderers)
                {
                    if (r.gameObject.name.Contains("Comment") || r.sharedMaterial.name.Contains("Water") ||
                        r.gameObject.name.Contains("Water"))
                        continue;
                    if (r.GetComponent<Notepad>())
                        continue;
                    r.sharedMaterial = targetMaterial;
                }
        }

        /// Recursively apply a material to the current gameobject and its children
        /// <param name="current"> The current gameobject to change</param>
        /// <param name="targetMaterial"> The material to apply to the renderer</param>
        public static void ApplyNewMaterialScheme(Material targetMaterial, GameObject current)
        {
            var renderers = current.GetComponentsInChildren<Renderer>();

            if (renderers == null) return;

            //Undo.RegisterCompleteObjectUndo(renderers, "Assigning Material Scheme");
            string section = current.GetComponent<BlockoutSection>().Section.ToString();
            for (var i = 0; i < renderers.Length; i++)
            {
                EditorUtility.DisplayProgressBar("Blockout Sub Theme", "Applying New Theme To Section " + section, i / (float)renderers.Length);
                var r = renderers[i];
                r.sharedMaterial = targetMaterial;
            }
            EditorUtility.ClearProgressBar();
        }

        /// Recursively apply a texture to the current gameobject and its children
        /// <param name="root"> The root objects to apply the textures to itself and children</param>
        /// <param name="texture"> The texture to apply to the renderer</param>
        public static void ApplyTextureIncChildren(Texture texture, Transform root)
        {
            using (new UndoScope("Apply Texture"))
            {
                if (!BlockoutEditorSettings.CurrentSceneSetting)
                {
                    BlockoutEditorSettings.CurrentSceneSetting = Resources.LoadAll<BlockoutSceneSettings>("")
                                                                          .First(x => x.sceneName == SceneManager.GetActiveScene().name);
                }
                Undo.RecordObject(BlockoutEditorSettings.CurrentSceneSetting, "");
                BlockoutEditorSettings.CurrentSceneSetting.currentTexture = BlockoutEditorSettings.CurrentGirdTexture;

                EditorUtility.SetDirty(BlockoutEditorSettings.CurrentSceneSetting);
                AssetDatabase.SaveAssets();

                var targetRenderers = root.GetComponentsInChildren<Renderer>().Where(x => !x.GetComponent<Notepad>()).ToArray();
                if (targetRenderers.Length > 0)
                {
                    var materialsToEdit = new List<Material>();
                    foreach (var r in targetRenderers)
                    {
                        foreach (var m in r.sharedMaterials)
                        {
                            if (!m) continue;
                            if (m.HasProperty("_Texture"))
                                materialsToEdit.Add(m);
                        }

                    }

                    //Undo.R(materialsToEdit.ToArray(), "Assigning Textures");

                    for (var index = 0; index < materialsToEdit.Count; index++)
                    {
                        EditorUtility.DisplayProgressBar("Blockout", "Applying Blockout Textures", index / (float)materialsToEdit.Count);
                        Material mat = materialsToEdit[index];
                        if (mat)
                            mat.SetTexture("_Texture", texture);
                    }
                    EditorUtility.ClearProgressBar();
                }

            }
        }

        /// <summary>
        /// Same as find obejct opf type, but only for the active scene
        /// </summary>
        /// <typeparam name="T">Object type to search for</typeparam>
        /// <returns>An array of objetcs</returns>
        private static T[] FindObjectsOfTypeInActiveScene<T>()
        {
            var targetSections = new List<T>();
            var goArray = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var t in goArray) targetSections.AddRange(t.GetComponentsInChildren<T>());

            return targetSections.ToArray();
        }

        /// Apply a material to the given group of gameobjects
        /// <param name="targetMaterial"> The target material</param>
        /// <param name="group"> The group of objects to chnage the material of</param>
        public static void ApplyMaterialToGroup(Material targetMaterial, GameObject[] group)
        {
            var targetRenderers = new List<Renderer>();
            group.ToList().ForEach(x => { targetRenderers.AddRange(x.GetComponentsInChildren<Renderer>()); });

            if (targetRenderers.Count <= 0) return;

            //using (new UndoScope("Apply Material Group"))
            {
                //Undo.RegisterCompleteObjectUndo(targetRenderers.ToArray(), "");
                foreach (var r in targetRenderers)
                    r.sharedMaterial = targetMaterial;
            }
        }

        /// <summary>
        /// Pings and object(s) in the project window. It will open a project window if one doesnt exist
        /// </summary>
        /// <param name="asset1">First Asset name to try (no extension)</param>
        /// <param name="asset2">Seccond Asset name to try (no extension)</param>
        public static void PingAssetInProjectWindow(string asset1, string asset2)
        {
            EditorApplication.ExecuteMenuItem("Window/Project");

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var guids = AssetDatabase.FindAssets(asset1 + " t:prefab", null);
            if (guids.Length == 0)
                guids = AssetDatabase.FindAssets(asset2 + " t:prefab", null);
            if (guids.Length > 0)
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        /// <summary>
        ///     Luminance the specified color.
        /// </summary>
        /// <param name="col">Color.</param>
        public static float Luminance(Color col)
        {
            float h, s, l;
            Color.RGBToHSV(col, out h, out s, out l);
            return l;
        }

        private static readonly Random random = new Random((int) DateTime.Now.Ticks);

        /// <summary>
        ///     Generates random color saturations and luminances based on hue.
        /// </summary>
        /// <returns>List of colors with varying saturations and luminances.</returns>
        /// <param name="colorCount">Color count.</param>
        /// <param name="hue">Base Hue.</param>
        public static List<Color> GenerateColors_SaturationLuminance(int colorCount, float hue)
        {
            var colors = new List<Color>();

            for (var i = 0; i < colorCount; i++)
            {
                var hslColor = new Color(hue, (float) random.NextDouble(), (float) random.NextDouble());

                colors.Add(hslColor);
            }

            return colors;
        }

        /// <summary>
        ///     Generates random color saturations based on hue and luminance.
        /// </summary>
        /// <returns>List of colors with varying saturations.</returns>
        /// <param name="colorCount">Color count.</param>
        /// <param name="hue">Hue.</param>
        /// <param name="luminance">Luminance.</param>
        public static List<Color> GenerateColors_Saturation(int colorCount, float hue, float luminance)
        {
            var colors = new List<Color>();

            for (var i = 0; i < colorCount; i++)
            {
                var hslColor = new Color(hue, (float) random.NextDouble(), luminance);

                colors.Add(hslColor);
            }

            return colors;
        }

        /// <summary>
        /// Using the current selected asset, find it's prefab in thew project window
        /// </summary>
        public static void SelectAsset()
        {
            var go = Selection.activeGameObject;
            var regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
            var output = Regex.Replace(go.name, regex, "");
            var res = AssetDatabase.FindAssets(output + " t:prefab");
            if (res.Length > 0)
            {
                var asset = (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(res[0]),
                    typeof(GameObject));
                if (PrefabUtility.GetPrefabParent(asset) == null && PrefabUtility.GetPrefabObject(asset) != null)
                    EditorGUIUtility.PingObject(asset);
            }
        }

        static bool ValidateIsPrefab(GameObject target)
        {
            GameObject go = target as GameObject;
            if (go == null)
                return false;

            return PrefabUtility.GetPrefabType(go) == PrefabType.Prefab ||
                   PrefabUtility.GetPrefabType(go) == PrefabType.ModelPrefab;
        }

        public static void ReplaceObject(GameObject[] selection, GameObject target)
        {
            if (!ValidateIsPrefab(target))
                return;

            using (new UndoScope("Replace Assets"))
            {

                foreach (var s in selection)
                {
                    var pos = s.transform.position;
                    var rot = s.transform.rotation;
                    var newAsset = PrefabUtility.InstantiatePrefab(target) as GameObject;
                    newAsset.transform.position = pos;
                    newAsset.transform.rotation = rot;
                    Undo.RegisterCreatedObjectUndo(newAsset, "");
                }
                selection.ToList().ForEach(Undo.DestroyObjectImmediate);

            }
        }

        public static void ReplaceObject(GameObject[] selection, GameObject target, out GameObject[] replacement)
        {
            replacement = new GameObject[]{ };

            if (!ValidateIsPrefab(target))
                return;

            List<GameObject> spawnedAssets = new List<GameObject>(selection.Length);

            using (new UndoScope("Replace Assets"))
            {

                foreach (var s in selection)
                {
                    var pos = s.transform.position;
                    var rot = s.transform.rotation;
                    var scale = s.transform.lossyScale;
                    var newAsset = PrefabUtility.InstantiatePrefab(target) as GameObject;
                    newAsset.transform.position = pos;
                    newAsset.transform.rotation = rot;
                    newAsset.transform.SetGlobalScale(scale);
                    Undo.RegisterCreatedObjectUndo(newAsset, "");
                    spawnedAssets.Add(newAsset);
                }
                selection.ToList().ForEach(Undo.DestroyObjectImmediate);

            }

            replacement = spawnedAssets.ToArray();

            ApplyCurrentTheme();
        }

        /// <summary>
        ///     Rotates selected objects around the specified axis and by amount degrees.
        /// </summary>
        /// <param name="axis">Axis.</param>
        /// <param name="amount">Amount in degrees.</param>
        public static void Rotate(Vector3 axis, float amount)
        {
            var sel = Selection.gameObjects;
            Undo.RecordObjects(sel.ToList().Select(x => x.transform).ToArray(), "Rotate objects");

            for (var i = 0; i < sel.Length; ++i)
            {
                var spaceMode = Tools.pivotRotation;

                // Unity is pivot rotation inverted?
                if (spaceMode == PivotRotation.Global)
                {
                    if (axis == Vector3.forward)
                        sel[i].transform.Rotate(sel[i].transform.forward * amount);
                    else if (axis == Vector3.right)
                        sel[i].transform.Rotate(sel[i].transform.right * amount);
                    else if (axis == Vector3.up)
                        sel[i].transform.Rotate(sel[i].transform.up * amount);
                }
                else
                {
                    sel[i].transform.Rotate(axis * amount);
                }
            }
        }

        /// <summary>
        ///     Mirror selected objects along the specified axis.
        /// </summary>
        /// <param name="axis">Axis to mirror on</param>
        public static void Mirror(Vector3 axis)
        {
            var selelected = Selection.gameObjects;
            Undo.RecordObjects(selelected.ToList().Select(x => x.transform).ToArray(), "Mirror objects");
            foreach (var s in selelected)
                s.transform.localScale = new Vector3(s.transform.localScale.x * (Math.Abs(axis.x) < 0.01f ? 1 : axis.x),
                    s.transform.localScale.y * (Math.Abs(axis.y) < 0.01f ? 1 : axis.y),
                    s.transform.localScale.z *
                    (Math.Abs(axis.z) < 0.01f ? 1 : axis.z));
        }

        /// <summary>
        /// Finds the first child object with a specific name
        /// </summary>
        /// <param name="fromGameObject">The target parent to check</param>
        /// <param name="withName">The target name</param>
        /// <returns>An array of child objects that match the name</returns>
        public static GameObject GetChildGameObject(GameObject fromGameObject, string withName)
        {
            var ts = fromGameObject.GetComponentsInChildren<Transform>();
            foreach (var t in ts) if (t.gameObject.name == withName) return t.gameObject;
            return null;
        }

        /// <summary>
        ///     Finds the blockout hierarchy.
        /// </summary>
        /// <returns>Always true as automatically repopulated if missing objects, but only false if there is no root object</returns>
        public static bool FindHeirachy(ref BlockoutHierarchy hierarchy)
        {
            var goArray = SceneManager.GetActiveScene().GetRootGameObjects();
            GameObject rootObj = null;
            for (var i = 0; i < goArray.Length; i++)
            {
                rootObj = GetChildGameObject(goArray[i], "Blockout");
                if (rootObj != null)
                    break;
            }
            if (rootObj)
            {
                hierarchy.root = rootObj.transform;
                //commentInGame = root.GetComponent<BlockoutCommentInGameGUI>();
                //GlobalNotes = root.GetComponent<Notepad>();
                //if (!GlobalNotes)
                //    GlobalNotes = root.gameObject.AddComponent<Notepad>();

                //commentInGame.GlobalNotes = GlobalNotes;
                //CommentBoxSceneGUI.GlobalNotes = GlobalNotes;

                var missingSections = new List<string>();

                var targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Floors && x.transform.parent == rootObj.transform).Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.floor = targetObj[0];
                else
                    missingSections.Add("Floors");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Walls && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.walls = targetObj[0];
                else
                    missingSections.Add("Walls");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Trim && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.trim = targetObj[0];
                else
                    missingSections.Add("Trim");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Dynamic && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.dynamic = targetObj[0];
                else
                    missingSections.Add("Dynamic");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Foliage && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.foliage = targetObj[0];
                else
                    missingSections.Add("Foliage");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Particles && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.particles = targetObj[0];
                else
                    missingSections.Add("Particles");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Triggers && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.triggers = targetObj[0];
                else
                    missingSections.Add("Triggers");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Cameras && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.cameras = targetObj[0];
                else
                    missingSections.Add("Cameras");

                targetObj = hierarchy.root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == SectionID.Comments && x.transform.parent == rootObj.transform)
                    .Select(x => x.transform).ToArray();
                if (targetObj.Length > 0)
                    hierarchy.comments = targetObj[0];
                else
                    missingSections.Add("Comments");

                if (missingSections.Count > 0)
                {
                    RegenHeirachy(missingSections, ref hierarchy);
                    FindHeirachy(ref hierarchy);
                    return true;
                }
                else
                {
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        ///     Regens the hierarchy with missing object.
        /// </summary>
        /// <param name="missingObjects">The missing objects.</param>
        /// <param name="hierarchy">Target Hierarchy</param>
        public static void RegenHeirachy(IList<string> missingObjects, ref BlockoutHierarchy hierarchy)
        {
            foreach (var missingObject in missingObjects)
            {
                var newObj = new GameObject(missingObject);
                newObj.transform.SetParent(hierarchy.root);
                switch (missingObject)
                {
                    case "Walls":
                        hierarchy.walls = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.walls.GetComponent<BlockoutSection>().Section = SectionID.Walls;
                        break;
                    case "Floors":
                        hierarchy.floor = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.floor.GetComponent<BlockoutSection>().Section = SectionID.Floors;
                        break;
                    case "Trim":
                        hierarchy.trim = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.trim.GetComponent<BlockoutSection>().Section = SectionID.Trim;
                        break;
                    case "Dynamic":
                        hierarchy.dynamic = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.dynamic.GetComponent<BlockoutSection>().Section = SectionID.Dynamic;
                        break;
                    case "Foliage":
                        hierarchy.foliage = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.foliage.GetComponent<BlockoutSection>().Section = SectionID.Foliage;
                        break;
                    case "Particles":
                        hierarchy.particles = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.particles.GetComponent<BlockoutSection>().Section = SectionID.Particles;
                        break;
                    case "Triggers":
                        hierarchy.triggers = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.triggers.GetComponent<BlockoutSection>().Section = SectionID.Triggers;
                        break;
                    case "Cameras":
                        hierarchy.cameras = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.cameras.GetComponent<BlockoutSection>().Section = SectionID.Cameras;
                        break;
                    case "Comments":
                        hierarchy.comments = newObj.AddComponent<BlockoutSection>().transform;
                        hierarchy.comments.GetComponent<BlockoutSection>().Section = SectionID.Comments;
                        break;
                    default:
                        Debug.LogError("BLOCKOUT :: (INTERNAL)" + missingObject + " NOT VALID!");
                        break;
                }
            }

        }

        /// <summary>
        ///     Creates the parent and force child.
        /// </summary>
        public static void CreateParentAndForceChild(string parentName, BlockoutHierarchy hierarchy)
        {
            if (Selection.gameObjects.Length <= 0)
            {
                EditorUtility.DisplayDialog("Blockout Error",
                    "Unable to create parent because no gameobjects selected", "Ok");
                return;
            }
            if (parentName == "")
            {
                EditorUtility.DisplayDialog("Blockout Error",
                    "Unable to create parent as object requires a name. Please provide a name!",
                    "Ok");
                return;
            }

            using (new UndoScope("Create Parent"))
            {

                var newParent = new GameObject(parentName);
                Undo.RegisterCreatedObjectUndo(newParent, "Created Parent");

                CreateBlockoutSubHeirachyWithRoot(newParent.transform, parentName + "_");

                var parentBounds = new Bounds(Selection.gameObjects[0].transform.position, Vector3.one);
                var renderers = new List<Renderer>();
                Selection.gameObjects.ToList().ForEach(x => renderers.AddRange(x.GetComponentsInChildren<Renderer>()));
                renderers.ForEach(x => parentBounds.Encapsulate(x.bounds));

                newParent.transform.SetParent(hierarchy.root);
                newParent.transform.position = parentBounds.center;
                Selection.gameObjects.ToList()
                    .ForEach(x => ReparentObjectToTargetRoot(x.transform, newParent.transform));

                TrimTargetBlockoutHierarchy(newParent);

                Selection.activeGameObject = newParent;
                EditorGUIUtility.PingObject(newParent);


            }
        }

        /// <summary>
        ///     Reparents the object to target root in blockout helper.
        /// </summary>
        /// <param name="target">The target objecty.</param>
        /// <param name="root">The root object.</param>
        public static void ReparentObjectToTargetRoot(Transform target, Transform root)
        {
            Transform[] parents;
            if (target.GetComponent<BlockoutHelper>())
            {
                parents = root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.transform.parent == root)
                    .Where(x => x.Section == target.GetComponentInParent<BlockoutHelper>().initialBlockoutSection)
                    .Select(x => x.transform).ToArray();
            }
            else
            {

                parents = root.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == target.GetComponentInParent<BlockoutSection>().Section)
                    .Select(x => x.transform).ToArray();
            }
            if (parents.Length > 0)
            {
                var targetParent = parents.First();
                if (targetParent)
                    Undo.SetTransformParent(target, targetParent, "");
            }
        }

        /// <summary>
        /// Removes excess children of a Blockout hierarchy that are not used
        /// </summary>
        /// <param name="targetObject">The target Blockout hierarchy root</param>
        public static void TrimTargetBlockoutHierarchy(GameObject targetObject)
        {
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Floors)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Floors).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Walls)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Walls).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Trim)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Trim).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Dynamic)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Dynamic).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Foliage)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Foliage).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Particles)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Particles).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Triggers)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Triggers).ToArray()[0].gameObject);
            if (targetObject.GetComponentsInChildren<BlockoutSection>().Where(x => x.Section == SectionID.Cameras)
                            .ToArray()[0].transform.childCount == 0)
                Object.DestroyImmediate(targetObject.GetComponentsInChildren<BlockoutSection>()
                                             .Where(x => x.Section == SectionID.Cameras).ToArray()[0].gameObject);
        }

        /// <summary>
        ///     Creates the blockout sub hierarchy with root.
        /// </summary>
        /// <param name="targetRoot">The target root object of the sub hierarchy.</param>
        public static void CreateBlockoutSubHeirachyWithRoot(Transform targetRoot, string namePrefix = "")
        {
            Undo.IncrementCurrentGroup();
            Undo.SetCurrentGroupName("Create Blockout SubHeirachy");
            var groupIndex = Undo.GetCurrentGroup();

            GameObject internalGO;
            BlockoutSection section;
            if (!targetRoot.GetComponent<BlockoutSection>())
                targetRoot.gameObject.AddComponent<BlockoutSection>().Section = SectionID.Root;
            if (!targetRoot.Find(namePrefix + "Floors"))
            {
                internalGO = new GameObject(namePrefix + "Floors");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Floors;
            }
            if (!targetRoot.Find(namePrefix + "Walls"))
            {
                internalGO = new GameObject(namePrefix + "Walls");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Walls;
            }
            if (!targetRoot.Find(namePrefix + "Trim"))
            {
                internalGO = new GameObject(namePrefix + "Trim");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Trim;
            }
            if (!targetRoot.Find(namePrefix + "Dynamic"))
            {
                internalGO = new GameObject(namePrefix + "Dynamic");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Dynamic;
            }
            if (!targetRoot.Find(namePrefix + "Foliage"))
            {
                internalGO = new GameObject(namePrefix + "Foliage");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Foliage;
            }
            if (!targetRoot.Find(namePrefix + "Triggers"))
            {
                internalGO = new GameObject(namePrefix + "Triggers");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Triggers;
            }
            if (!targetRoot.Find(namePrefix + "Particles"))
            {
                internalGO = new GameObject(namePrefix + "Particles");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Particles;
            }
            if (!targetRoot.Find(namePrefix + "Cameras"))
            {
                internalGO = new GameObject(namePrefix + "Cameras");
                Undo.RegisterCreatedObjectUndo(internalGO.gameObject, "Created go");
                internalGO.GetComponent<Transform>().SetParent(targetRoot);
                section = internalGO.gameObject.AddComponent<BlockoutSection>();
                section.Section = SectionID.Cameras;
            }
            Undo.CollapseUndoOperations(groupIndex);
        }

        public static bool UpdateAvailable(out string version)
        {
            WebClient webClient = new WebClient();
            version = "";
            try
            {
                version = webClient.DownloadString("http://blockout.radicalforge.com/BlockoutVersion.txt");
                return version != BlockoutEditorSettings.VERSION;
            }
            catch (WebException e)
            {
                Debug.LogError("Unable to check for Blockout updates: " + e.Status);
                throw;
            }
            
        }

    }
}