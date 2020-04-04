using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorCameraAnchorSection : BlockoutEditorSection
    {

        private string anchorName = "Camera Name";
        private Material screenshotWhiteMaterial;
        private int screenshotWidth = 1920;
        private int screenshotHeight = 1080;
        private int supersizeResolution = 1;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Camera Anchor Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Camera_Anchors"), "")),

                new EUISectionContent("Save Camera", new GUIContent("Save Scene Camera Anchor", "Save current scene view to a camera anchor")),
                new EUISectionContent("White Screenshot", new GUIContent(EUIResourceManager.Instance.GetTexture("Screenshot"), "Captures a screenshot with a white material setup using the current game anchor camera")),
                new EUISectionContent("Color Screenshot", new GUIContent(EUIResourceManager.Instance.GetTexture("Screenshot_Coloured"), "Captures a screenshot of the current selected game anchor camera using the current theme")),
                new EUISectionContent("Game Vis On", new GUIContent(EUIResourceManager.Instance.GetTexture("Game_Visibility_On"), "Show camera in game view")),

                new EUISectionContent("Game Vis Off", new GUIContent (EUIResourceManager.Instance.GetTexture("Game_Visibility"), "Show camera in game view")),
                new EUISectionContent("Trash", new GUIContent(EUIResourceManager.Instance.GetTexture("Trash_Can"), "Delete the current camera anchor")),
                new EUISectionContent("Clear All Cameras", new GUIContent ("Clear All Cameras", "Deletes all the camera canchors in a scene")),

                new EUISectionContent("Screenshot Header", new GUIContent (EUIResourceManager.Instance.GetTexture("Screenshot_Settings"), "Deletes all the camera canchors in a scene")),
                new EUISectionContent("Resolution Scale", new GUIContent ("Resolution Scale", "Scales the internal resolution of the screenshot")),
                new EUISectionContent("Screenshot Width", new GUIContent ("Width", "The width of the screenshot")),
                new EUISectionContent("Screenshot Height", new GUIContent ("Height", "The height of the screenshot"))
            };

            screenshotWhiteMaterial = Resources.Load<Material>("Blockout/Materials/ScreenshotWhite");

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }


        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Camera Anchor Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            using (new HorizontalCenteredScope())
            {
                anchorName = GUILayout.TextField(anchorName, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Save Camera"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                    SaveSceneCamera();
            }

            GUILayout.Space(10);

            using (new GUILayout.VerticalScope())
            {
                for (int i = 0; i < BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.Count; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Space(5);
                        // Camera Anchor
                        if (GUILayout.Button(i + ". " + BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor[i].name,
                            GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth * 1.4f),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight + 5)))
                        {
                            if (i >= BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.Count)
                            {
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();
                                Debug.LogError("BLOCKOUT :: Camera Anchor In Scene Deleted...Removing Anchor");
                            }
                            else if (blockoutHierarchy.cameras.GetChild(i).gameObject.name !=
                                     BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor[i].name)
                            {
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();
                                Debug.LogError("BLOCKOUT :: Camera Anchor In Scene Deleted...Removing Anchor");
                            }
                            else if (SceneView.lastActiveSceneView)
                            {
                                var currentAnchor = BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor[i];
                                SceneView.lastActiveSceneView.size = currentAnchor.size;
                                SceneView.lastActiveSceneView.pivot = currentAnchor.position;
                                SceneView.lastActiveSceneView.rotation = currentAnchor.rotation;
                            }

                        }


                        if (i >= blockoutHierarchy.cameras.childCount)
                        {
                            BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                            BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();
                            continue;
                        }
                        

                        // White Screenshot
                        if (GUILayout.Button(EUIResourceManager.Instance.GetContent("White Screenshot"), GUILayout.Width(25),
                                                    GUILayout.Height(EditorGUIUtility.singleLineHeight + 5)))
                        {
                            CreateWhiteScreenShot(i);
                        }

                        // Color Screenshot
                        if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Color Screenshot"), GUILayout.Width(25),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight + 5)))
                        {
                            CreateColorScreenShot(i);
                        }

                        // show in game view
                        if (GUILayout.Button(EUIResourceManager.Instance.GetContent(
                            blockoutHierarchy.cameras.GetChild(i).gameObject.activeInHierarchy
                            ? "Game Vis On"
                            : "Game Vis Off"),
                            GUILayout.MaxWidth(25),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight + 5)))
                        {
                            if (i >= blockoutHierarchy.cameras.childCount)
                            {
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();
                                Debug.LogError("BLOCKOUT :: Camera Anchor In Scene Deleted...Removing Anchor");
                            }
                            else if (blockoutHierarchy.cameras.GetChild(i).gameObject.name !=
                                        BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor[i].name)
                            {
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();
                                Debug.LogError("BLOCKOUT :: Camera Anchor In Scene Deleted...Removing Anchor");
                            }
                            else
                            {
                                //if (!cameras)
                                //    if (!FindHeirachy())
                                //        FindHeirachy();
                                for (var c = 0; c < blockoutHierarchy.cameras.childCount; ++c)
                                {
                                    if (c == i)
                                        continue;
                                    blockoutHierarchy.cameras.GetChild(c).gameObject.SetActive(false);
                                }
                                blockoutHierarchy.cameras.GetChild(i).gameObject
                                    .SetActive(!blockoutHierarchy.cameras.GetChild(i).gameObject.activeSelf);
                            }
                        }

                        // Delete anchor
                        if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Trash"),
                            GUILayout.Width(EditorGUIUtility.singleLineHeight + 5),
                            GUILayout.Height(EditorGUIUtility.singleLineHeight + 5)))
                        {
                            using (new UndoScope("Delete Camera Anchor"))
                            {

                                Undo.RecordObject(BlockoutEditorSettings.CurrentSceneSetting, "Destroy Camera Anchor");
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.RemoveAt(i);
                                BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.TrimExcess();

                                EditorUtility.SetDirty(BlockoutEditorSettings.CurrentSceneSetting);
                                AssetDatabase.SaveAssets();

                                Undo.RecordObject(blockoutHierarchy.cameras.GetChild(i).gameObject, "");
                                DestroyImmediate(blockoutHierarchy.cameras.GetChild(i).gameObject);
                            }
                        }
                    }
                }

                if (BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.Count <= 0)
                {
                    GUILayout.Label("There are no camera anchors defined");
                }

                GUILayout.Space(10);

                using (new HorizontalCenteredScope())
                {
                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Clear All Cameras"), GUILayout.MaxWidth(BlockoutEditorSettings.OneColumnWidth)))
                    {
                        using (new UndoScope("Clear All Camera Anchors"))
                        {
                            var toDelete = new List<GameObject>();

                            for (var i = blockoutHierarchy.cameras.childCount - 1; i >= 0; --i)
                                toDelete.Add(blockoutHierarchy.cameras.GetChild(i).gameObject);
                            Undo.RecordObjects(toDelete.ToArray(), "");
                            for (var i = toDelete.Count - 1; i >= 0; --i)
                                DestroyImmediate(toDelete[i]);
                            Undo.RecordObject(BlockoutEditorSettings.CurrentSceneSetting, "");
                            BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.Clear();

                            EditorUtility.SetDirty(BlockoutEditorSettings.CurrentSceneSetting);
                            AssetDatabase.SaveAssets();

                        }
                    }
                }

                GUILayout.Space(5);

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Box(EUIResourceManager.Instance.GetContent("Screenshot Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
                }

                if (!Application.isPlaying && Application.isEditor)
                    supersizeResolution =
                        EditorGUILayout.IntSlider(EUIResourceManager.Instance.GetContent("Resolution Scale"), supersizeResolution, 1, 10);
                else
                    GUILayout.Label("Feature Not Available during play mode");
                screenshotWidth = EditorGUILayout.IntField(EUIResourceManager.Instance.GetContent("Screenshot Width"), screenshotWidth);
                screenshotHeight = EditorGUILayout.IntField(EUIResourceManager.Instance.GetContent("Screenshot Height"), screenshotHeight);

                GUILayout.Space(5);
            }
        }

        void SaveSceneCamera()
        {
            if(blockoutHierarchy.root.GetComponentsInChildren<Transform>(true).Any(x => x.name == anchorName))
                Debug.LogError("BLOCKOUT :: Unable to create new camera anchor. Please ensure the camera name is unique");
            else if (SceneView.lastActiveSceneView)
            {
                var newAnchor = new CameraAnchor
                {
                    name = anchorName,
                    position = SceneView.lastActiveSceneView.pivot,
                    rotation = SceneView.lastActiveSceneView.rotation,
                    size = SceneView.lastActiveSceneView.size
                };

                using (new UndoScope("Create Camera Anchor"))
                {
                    GameObject newCamera = new GameObject(newAnchor.name);
                    Undo.RegisterCreatedObjectUndo(newCamera, "");
                    Camera camera = newCamera.AddComponent<Camera>();
                    camera.depth = 100;

                    newCamera.transform.position =
                        SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
                    newCamera.transform.rotation = newAnchor.rotation;
                    newCamera.transform.SetParent(blockoutHierarchy.cameras);
                    newCamera.SetActive(false);

                    Undo.RecordObject(BlockoutEditorSettings.CurrentSceneSetting, "");
                    BlockoutEditorSettings.CurrentSceneSetting.cameraAnchor.Add(newAnchor);

                    EditorUtility.SetDirty(BlockoutEditorSettings.CurrentSceneSetting);
                    AssetDatabase.SaveAssets();

                }
            }
        }

        void CreateWhiteScreenShot(int i)
        {
            CommentBoxSceneGUI.Disable();
            var helpers = FindObjectsOfType<BlockoutHelper>().ToList();
            helpers.ForEach(x => x.HideLockedState());

            BlockoutStaticFunctions.ApplyNewMaterialSchemeWithoutUndo(screenshotWhiteMaterial, blockoutHierarchy.root.gameObject);

            CaptureScreenShot(i);

            BlockoutStaticFunctions.ApplyCurrentTheme();
            helpers.ForEach(x => x.ShowLockedState());
        }

        void CreateColorScreenShot(int i)
        {
            CommentBoxSceneGUI.Disable();

            var helpers = FindObjectsOfType<BlockoutHelper>().ToList();
            helpers.ForEach(x =>x.HideLockedState());

            CaptureScreenShot(i);

            helpers.ForEach(x => x.ShowLockedState());
        }

        void CaptureScreenShot(int camera)
        {
            var resWidthN = screenshotWidth * supersizeResolution;
            var resHeightN = screenshotHeight * supersizeResolution;

            var rt = new RenderTexture(resWidthN, resHeightN, 24);

            var myCamera = blockoutHierarchy.cameras.GetChild(camera).GetComponent<Camera>();
            var currentRT = myCamera.targetTexture;

            myCamera.targetTexture = rt;

            TextureFormat tFormat;
            tFormat = TextureFormat.RGB24;


            var screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
            myCamera.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
            myCamera.targetTexture = null;
            RenderTexture.active = null;
            var bytes = screenShot.EncodeToPNG();
            var filename = "BLOCKOUT SCREENSHOT " + DateTime.Now.ToString("yyyyMMddHHmmss") +
                           "_" + resWidthN + "x" + resHeightN + ".png";

            File.WriteAllBytes(filename, bytes);
            Application.OpenURL(filename);

            myCamera.targetTexture = currentRT;

            Debug.Log("SAVED SCREENSHOT: " + Application.dataPath + "/../" + filename);

            EditorUtility.RevealInFinder(Application.dataPath + "/../" + filename);
        }

        
    }
}
