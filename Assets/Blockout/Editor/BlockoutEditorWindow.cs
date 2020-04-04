/* Radical Forge Copyright (c) 2017 All Rights Reserved
   </copyright>
   <author>Frederic Babord</author>
   <date>15th June 2017</date>
   <summary>The main blockout editor window logic and visual apperance</summary>*/

// Sorry got bored @ 03:42
//                                        "-._
//              ______________________---"__,-'___  __________________________________
//          _,-'/| #### | |              ####     || _  ______________________________
//       _-'            | |                       |||O||____||____||____||_____||_____
//      (-------------------------------RF--------|||_|-----RF------------------------
//       \------(o)~~~~(o)------------(o)~~~~(o)--'`---(o)~~~(o)----------------------


using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace RadicalForge.Blockout
{
    public enum BlockoutAxis
    {
        X = 0,
        Y,
        Z
    }

    public class BlockoutEditorWindow : EditorWindow
    {
        [MenuItem("Window/Blockout/Editor", false, 10)]
        public static void Init()
        {
            var filesToDel = Directory
                .GetFiles(Application.dataPath + "/Blockout/Editor/", "*", SearchOption.AllDirectories)
                .Where(x => x.Contains("BlockoutBlockHelper.cs")).ToArray();
            for(int i = 0; i < filesToDel.Length; ++i)
                File.Delete(filesToDel[i]);

            // Get existing open window or if none, make a new one:
            var window = (BlockoutEditorWindow)GetWindow(typeof(BlockoutEditorWindow));
            window.maxSize = new Vector2(4000, 4000);
            window.minSize = new Vector2(405, 100);

           

            window.Show();
            
        }

        public BlockoutEditorAssetsSection assetsSection;
        public BlockoutEditorScalableObjectsSection scalableObjectsSection;
        public BlockoutEditorTransformSection transformSection;
        public BlockoutEditorOthersSection othersSection;
        public BlockoutEditorCameraAnchorSection cameraAnchorSection;
        public BlockoutGridSnapping gridSnapping;
        public BlockoutEditorHierarchyToolsSection hierarchyTools;
        public BlockoutEditorCommentsSection commentsSection;
        public BlockoutEditorSuggestedSection suggestedSection;
        public BlockoutEditorSplashSection splashScreen;

        private BlockoutEditorSection[] sections;

        private Vector2 scroll;
        private BlockoutHierarchy hierarchy;

        private Scene currentScene;
        private bool renderingSplash = true;

        public static bool isVisible = false;

        public GameObject previousSelected;
        
        private string newVersion;

        void OnEnable()
        {
            hierarchy = new BlockoutHierarchy();
            BlockoutStaticFunctions.FindHeirachy(ref hierarchy);

            EUIResourceManager rm = EUIResourceManager.Instance;

            splashScreen = new BlockoutEditorSplashSection();
            splashScreen.InitSection("Splash Section", hierarchy);

            assetsSection = new BlockoutEditorAssetsSection();
            assetsSection.InitSection("Blockout Assets", hierarchy);

            scalableObjectsSection = new BlockoutEditorScalableObjectsSection();
            scalableObjectsSection.InitSection("Scalable Objects", hierarchy);

            transformSection = new BlockoutEditorTransformSection();
            transformSection.InitSection("Transform Controls", hierarchy);

            othersSection = new BlockoutEditorOthersSection();
            othersSection.InitSection("Others", hierarchy);

            cameraAnchorSection = new BlockoutEditorCameraAnchorSection();
            cameraAnchorSection.InitSection("Camera Anchors", hierarchy);

            gridSnapping = new BlockoutGridSnapping();
            gridSnapping.InitSection("Grid Snapping", hierarchy);

            hierarchyTools = new BlockoutEditorHierarchyToolsSection();
            hierarchyTools.InitSection("Hierarchy Tools", hierarchy);

            commentsSection = new BlockoutEditorCommentsSection();
            commentsSection.InitSection("Comments", hierarchy);

            suggestedSection = new BlockoutEditorSuggestedSection();
            suggestedSection.InitSection("Suggesstions", hierarchy);

            titleContent = new GUIContent("Blockout", EUIResourceManager.Instance.GetTexture(EditorGUIUtility.isProSkin ? "Blockout_Icon_Light" : "Blockout_Icon_Dark"));

            sections = new BlockoutEditorSection[]
            {
                assetsSection,
                scalableObjectsSection,
                transformSection,
                gridSnapping,
                hierarchyTools,
                commentsSection,
                suggestedSection,
                cameraAnchorSection,
                othersSection
            }; 

            BlockoutEditorSettings.CurrentSceneSetting = Resources.LoadAll<BlockoutSceneSettings>("")
                .First(x => x.sceneName == SceneManager.GetActiveScene().name);
            isVisible = true;
            BlockoutEditorHelper.Awake(this);
            EditorHotkeysTracker.Init(this);
            
            BlockoutStaticFunctions.UpdateAvailable(out newVersion);
            splashScreen.newVersion = newVersion;
        }
        

        void OnGUI()
        {
            using (var sv = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = sv.scrollPosition;

                using (new HorizontalCenteredScope())
                {
                    using (new GUILayout.VerticalScope(GUILayout.Width(390)))
                    {
                        if (BlockoutStaticFunctions.FindHeirachy(ref hierarchy))
                        {
                            renderingSplash = false;
                            DrawMainWindow();
                        }
                        else
                        {
                            renderingSplash = true;
                            splashScreen.DrawSection();
                        }
                    }
                }
            }

            if(sections.Count(x => x.Repaint) > 0)
                Repaint();

        }

        void Update()
        {
            sections.ToList().ForEach(x => x.Update());

            if (currentScene != SceneManager.GetActiveScene() && !renderingSplash)
            {
                BlockoutEditorSettings.CurrentSceneSetting = null;
                currentScene = SceneManager.GetActiveScene();
                BlockoutStaticFunctions.TryLoadSceneDefinitions();
                BlockoutStaticFunctions.FindHeirachy(ref hierarchy);
            }
        }

        private GameObject previousGameObject;

        // TODO: Check mac fixes

        void OnInspectorUpdate()
        {
            if (previousGameObject != Selection.activeGameObject)
            {
                previousGameObject = Selection.activeGameObject;
                if (Selection.activeGameObject)
                {
                    var bh = Selection.activeGameObject.GetComponent<BlockoutHelper>();
                    if (bh)
                    {
                        if (bh.ReapplyMaterialTheme)
                        {
                            BlockoutStaticFunctions.ApplyCurrentTheme();
                        }
                    }
                }
            }

            if (sections.Count(x => x.Repaint) > 0)
                Repaint();
        }

        void OnDisable()
        {
            isVisible = false;
            BlockoutEditorHelper.Destroy();
            EditorHotkeysTracker.Destroy();
        }

        void DrawMainWindow()
        {
            assetsSection.DrawSection();
            scalableObjectsSection.DrawSection();
            transformSection.DrawSection();
            gridSnapping.DrawSection();
            hierarchyTools.DrawSection();
            suggestedSection.DrawSection();
            commentsSection.DrawSection();
            cameraAnchorSection.DrawSection();
            othersSection.DrawSection();
            
        }

    }

}