
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutGridSnapping : BlockoutEditorSection
    {
        private int selectedSnapping = 1;
        private readonly string[] Options = { "Custom", "0.25", "0.5", "1.0" };
        private readonly float[] OptionValues = { 0.0f, 0.25f, 0.5f, 1.0f };

        private readonly Color backgroundRed = new Color(1.0f, 0.467f, 0.465f);
        private readonly Color backgroundGreen = new Color(0.467f, 1.0f, 0.514f);
        private readonly Color backgroundBlue = new Color(0.467f, 0.67f, 1.0f);

        private Vector3 prevPosition = Vector3.zero, prevScale = Vector3.one;
        private float snapValue;
        private GameObject previousObjectMain, currentObjectMain;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Grid Snapping Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Grid_Snapping"), "")),
                new EUISectionContent("Auto Snapping Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Auto"), "Automatically snap position and scale")),
                new EUISectionContent("Snapping Label", new GUIContent("Snap Distance", "")),
                new EUISectionContent("Snap +X", new GUIContent("+X", "Snap Object To First Collider Positive X (RED) Axis in WORLD space")),
                new EUISectionContent("Snap -X", new GUIContent("-X", "Snap Object To First Collider Negative X (RED) Axis in WORLD space")),
                new EUISectionContent("Snap +Y", new GUIContent("+Y", "Snap Object To First Collider Positive Y (GREEN) Axis in WORLD space")),
                new EUISectionContent("Snap -Y", new GUIContent("-Y", "Snap Object To First Collider Negative Y (GREEN) Axis in WORLD space")),
                new EUISectionContent("Snap +Z", new GUIContent("+Z", "Snap Object To First Collider Positive Z (BLUE) Axis in WORLD space")),
                new EUISectionContent("Snap -Z", new GUIContent("-Z", "Snap Object To First Collider Negative Z (BLUE) Axis in WORLD space"))

            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);

            snapValue = OptionValues[selectedSnapping];
            EditorPrefs.SetFloat("Blockout::Snap", snapValue);
        }

        public override void Update()
        {
            // Auto snapping position
            if (BlockoutEditorSettings.AutoSnap
                && !EditorApplication.isPlaying
                && Selection.transforms.Length > 0
                && Selection.transforms[0].position != prevPosition)
                if ((Selection.transforms[0].position - prevPosition).magnitude > snapValue)
                {
                    prevPosition = Selection.transforms[0].position;
                    previousObjectMain = currentObjectMain;
                    currentObjectMain = Selection.transforms[0].gameObject;
                }
                else if (previousObjectMain != null)
                {
                    BlockoutStaticFunctions.SnapPositionSelection();
                    prevPosition = Selection.transforms[0].position;
                }
            // Auto snapping scale
            if (BlockoutEditorSettings.AutoSnap
                && !EditorApplication.isPlaying
                && Selection.transforms.Length > 0
                && Selection.transforms[0].lossyScale != prevScale)
                if ((Selection.transforms[0].lossyScale - prevScale).magnitude > snapValue)
                {
                    prevScale = Selection.transforms[0].lossyScale;
                    previousObjectMain = currentObjectMain;
                    currentObjectMain = Selection.transforms[0].gameObject;
                }
                else if (previousObjectMain != null)
                {
                    BlockoutStaticFunctions.SnapScaleSelection();
                    prevScale = Selection.transforms[0].lossyScale;
                }
        }

        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Grid Snapping Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));

                GUILayout.FlexibleSpace();

                GUILayout.Box(EUIResourceManager.Instance.GetContent("Auto Snap"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(60), GUILayout.Height(30));
                BlockoutEditorSettings.AutoSnap = GUILayout.Toggle(BlockoutEditorSettings.AutoSnap, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
            }

            if (!showSection)
                return;

            using (new HorizontalCenteredScope(GUILayout.MaxWidth(BlockoutEditorSettings.OneColumnWidth)))
            {
                GUILayout.Label(EUIResourceManager.Instance.GetContent("Snapping Label"));
                GUILayout.FlexibleSpace();
                selectedSnapping = GUILayout.SelectionGrid(selectedSnapping, Options, Options.Length, EditorStyles.miniButton, GUILayout.Width(292));
            }

            using (new HorizontalCenteredScope(GUILayout.MaxWidth(BlockoutEditorSettings.OneColumnWidth)))
            {
                if (!Application.isPlaying && Application.isEditor)
                    if (selectedSnapping < OptionValues.Length && selectedSnapping > 0)
                    {
                        snapValue = OptionValues[selectedSnapping];
                    }
                    else
                    {
                        snapValue = EditorGUILayout.FloatField("Custom Snap", snapValue, GUILayout.Width(BlockoutEditorSettings.OneColumnWidth));
                    }
                else
                    GUILayout.Label("Feature not availble during play mode.");

                EditorPrefs.SetFloat("Blockout::Snap", snapValue);
            }


            using (new HorizontalCenteredScope())
            {
                var originalColour = GUI.backgroundColor;
                GUI.backgroundColor = backgroundRed;

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap +X"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(Vector3.right, BlockoutAxis.X);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap -X"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(-Vector3.right, BlockoutAxis.X);

                GUI.backgroundColor = backgroundGreen;
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap +Y"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(Vector3.up, BlockoutAxis.Y);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap -Y"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(-Vector3.up, BlockoutAxis.Y);

                GUI.backgroundColor = backgroundBlue;
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap +Z"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(Vector3.forward, BlockoutAxis.Z);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Snap -Z"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Snap(-Vector3.forward, BlockoutAxis.Z);

                GUI.backgroundColor = originalColour;
            }

        }

        public void IncreaseSnapValue()
        {
            if (selectedSnapping < OptionValues.Length - 1)
                selectedSnapping++;
            snapValue = OptionValues[selectedSnapping];
        }

        public void DecreaseSnapValue()
        {
            if (selectedSnapping > 0)
                selectedSnapping--;
            snapValue = OptionValues[selectedSnapping];
        }
    }
}
