using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorSplashSection : BlockoutEditorSection
    {
        public string newVersion = "";

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Blockout Logo", new GUIContent(EUIResourceManager.Instance.GetTexture("Blockout_Logo"), "")),
                new EUISectionContent("Create Hierarchy", new GUIContent(EUIResourceManager.Instance.GetTexture("AUTO_GENERATE_SCENE_HIERARCHY"), "Creates a scene hierarchy for the blockout editor to use.")),
                new EUISectionContent("Intro Label", new GUIContent("First off, thank you for purchasing Blockout! While you probably" +
                                                     Environment.NewLine +
                                                     "should read at least the Quick Start section in the documentation, " +
                                                     Environment.NewLine +
                                                     "you probably want to dive right in and that’s fine (that’s exactly" +
                                                     Environment.NewLine +
                                                     "what we would do). There should be tooltips around each section " +
                                                     Environment.NewLine +
                                                     "of the Blockout window to help clarify its function." + Environment.NewLine +
                                                     Environment.NewLine +
                                                     "Note: this tool supports multi-scene editing. ", "")),
                
                new EUISectionContent("Example Scenes", new GUIContent(EUIResourceManager.Instance.GetTexture("Blockout_Example Scenes"))),
                new EUISectionContent("All Assets", new GUIContent(EUIResourceManager.Instance.GetTexture("ALL_ASSETS"), "Opens the all assets scene")),
                new EUISectionContent("FPS Scene", new GUIContent(EUIResourceManager.Instance.GetTexture("FPS_Button"), "Opens the FPS example scene")),
                new EUISectionContent("Rollerball Scene", new GUIContent(EUIResourceManager.Instance.GetTexture("Rollerball_Button"), "Opens the rollerball example scene")),
                new EUISectionContent("Open Documentation", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_Documentation"), "Opens the online documentation in the default web browser")),
                new EUISectionContent("Open Tutorials", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_Tutorials"), "Opens the online tutorials (YouTube playlist) in the default web browser")),
                new EUISectionContent("Feedback", new GUIContent(EUIResourceManager.Instance.GetTexture("Feedback_Bug"), "Opens your default email client to send us an email")),
                new EUISectionContent("Radical Forge Logo", new GUIContent(EUIResourceManager.Instance.GetTexture("RadicalForgeLogoLong")))

            };



            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }

        public override void DrawSection()
        {
            repaint = false;

            using (new HorizontalCenteredScope())
            {
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Blockout Logo"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(BlockoutEditorSettings.OneColumnWidth), GUILayout.Height(120));
            }

            if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Create Hierarchy"), GUILayout.Height(EditorGUIUtility.singleLineHeight * 6), GUILayout.Width(BlockoutEditorSettings.OneColumnWidth)))
            {
                if(!BlockoutStaticFunctions.FindHeirachy(ref blockoutHierarchy))
                {
                    if (EditorSceneManager.SaveOpenScenes())
                    {
                        using (new UndoScope("Create Blockout Hierarchy"))
                        {
                            blockoutHierarchy.root = new GameObject("Blockout").transform;
                            Undo.RegisterCreatedObjectUndo(blockoutHierarchy.root.gameObject, "");
                            blockoutHierarchy.root.gameObject.AddComponent<BlockoutCommentInGameGUI>();
                            blockoutHierarchy.root.gameObject.AddComponent<Notepad>();
                            var section = blockoutHierarchy.root.gameObject.AddComponent<BlockoutSection>();
                            section.Section = SectionID.Root;
                            BlockoutStaticFunctions.CreateBlockoutSubHeirachyWithRoot(blockoutHierarchy.root);
                            BlockoutStaticFunctions.FindHeirachy(ref blockoutHierarchy);
                            BlockoutStaticFunctions.TryLoadSceneDefinitions();
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Blockout Editor Error",  "You need to have a SAVED scene before using the Blockout system", "OK");
                    }
                }
            }

            GUILayout.Space(5);
            GUILayout.Label(EUIResourceManager.Instance.GetContent("Intro Label"));
            GUILayout.Space(5);

            if (newVersion != BlockoutEditorSettings.VERSION)
            {
                GUILayout.Space(5);
                using (new HorizontalCenteredScope())
                {
                    GUILayout.Label("New Version Available (" + newVersion + ")", EditorStyles.boldLabel,
                                    GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                }

                GUILayout.Space(5);
            }

            GUILayout.Label(EUIResourceManager.Instance.GetContent("Example Scenes"), GUILayout.Width(390), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
            if (GUILayout.Button(EUIResourceManager.Instance.GetContent("All Assets"), GUILayout.Width(390),
                                 GUILayout.Height(EditorGUIUtility.singleLineHeight * 3 + 8)))
                EditorSceneManager.OpenScene("Assets/Blockout/Examples/All Assets.unity");

            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("FPS Scene"), GUILayout.Width(193), GUILayout.Height(125)))
                    EditorSceneManager.OpenScene("Assets/Blockout/Examples/FPS.unity");
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Rollerball Scene"), GUILayout.Width(193), GUILayout.Height(125)))
                    EditorSceneManager.OpenScene("Assets/Blockout/Examples/Rollerball.unity");
            }

            GUILayout.Space(5);

            GUILayout.Label("Keyboard Shortcuts");

            using (new GUILayout.HorizontalScope())
            {
                if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    GUILayout.Label("Ctrl + ALT + B", EditorStyles.boldLabel, GUILayout.Width(152));
                    GUILayout.Label("Show The Blockout Window");
                }
                else
                {
                    GUILayout.Label("Cmd + ALT + B", EditorStyles.boldLabel, GUILayout.Width(152));
                    GUILayout.Label("Show / Hide The Blockout Window");
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Alt + S", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Toggle Auto Grid Snapping");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Alt + C", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Toggle Comments");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Alt + Z", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Decrease Grid Snapping Value");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Alt + X", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Increase Grid Snapping Value");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("End", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Snap to ground (-Y)");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("L", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Toggle Locked Assets");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("G", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Jump To Selected Prefab In Project");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("C (Hold)", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("(Hold) Show Scroll Picker");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("(Release) Confirm Swap");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Space", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Scrollpicker: Spawn Selected Asset");
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Quickpicker: Show / Select Item");
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Arrow Keys", EditorStyles.boldLabel, GUILayout.Width(152));
                GUILayout.Label("Quickpicker: Change selection");
            }

            GUILayout.Space(5);


            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Open Documentation"), GUILayout.Width(193),
                                     GUILayout.Height(EditorGUIUtility.singleLineHeight * 3 + 8)))
                    BlockoutStaticFunctions.OpenDocumentation();
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Open Tutorials"), GUILayout.Width(193),
                                     GUILayout.Height(EditorGUIUtility.singleLineHeight * 3 + 8)))
                    BlockoutStaticFunctions.OpenTutorials();
            }

            GUILayout.Space(5);

            if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Feedback"), GUILayout.Width(390), GUILayout.Height(EditorGUIUtility.singleLineHeight * 3 + 8)))
                BlockoutStaticFunctions.SubmitBugReport();

            GUILayout.Space(10);

            using (new HorizontalCenteredScope())
            {
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Radical Forge Logo"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(150),
                              GUILayout.Height(45));
            }

        }
    }
}
