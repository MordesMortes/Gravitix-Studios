
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorTransformSection : BlockoutEditorSection
    {

        private Color   m_backgroundRed = new Color(1.0f, 0.467f, 0.465f),
                        m_backgroundGreen = new Color(0.467f, 1.0f, 0.514f),
                        m_backgroundBlue = new Color(0.467f, 0.67f, 1.0f);

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Transform Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Transform_Controls"), "")),

                new EUISectionContent("Global Position", new GUIContent("Center Global Position", "Generates and applies a random color pallet")),
                new EUISectionContent("Local Position", new GUIContent("Center Local Position", "Applies the last generated random color pallet as the theme")),
                new EUISectionContent("Global Rotation", new GUIContent("Reset Global Rotation", "Shows the individual colors for the user pallet")),
                new EUISectionContent("Local Rotation", new GUIContent("Reset Local Rotation", "Lock this color choice from random generation?")),

                new EUISectionContent("X 90", new GUIContent (EUIResourceManager.Instance.GetTexture("Button_x90"), "Rotate a object clockwise 90 degrees along the X asis")),
                new EUISectionContent("X 180", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_x180"), "Rotate a object clockwise 180 degrees along the X axis")),
                new EUISectionContent("Y 90", new GUIContent (EUIResourceManager.Instance.GetTexture("Button_y90"), "Rotate a object clockwise 90 degrees along the Y axis")),
                new EUISectionContent("Y 180", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_y180"), "Rotate a object clockwise 180 degrees along the Y axis")),
                new EUISectionContent("Z 90", new GUIContent (EUIResourceManager.Instance.GetTexture("Button_z90"), "Rotate a object clockwise 90 degrees along the Z axis")),
                new EUISectionContent("Z 180", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_z180"), "Rotate a object clockwise 180 degrees along the Z axis")),

                new EUISectionContent("Mirror X", new GUIContent(EUIResourceManager.Instance.GetTexture("Mirror_x"), "Mirror in the X Axis")),
                new EUISectionContent("Mirror Y", new GUIContent(EUIResourceManager.Instance.GetTexture("Mirror_y"), "Mirror in the Y Axis")),
                new EUISectionContent("Mirror Z", new GUIContent(EUIResourceManager.Instance.GetTexture("Mirror_z"), "Mirror in the Z Axis"))
            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }

        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Transform Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            // Position
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Global Position"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                {
                    var selection = Selection.gameObjects;
                    Undo.RecordObjects(selection.ToList().Select(x => x.transform).ToArray(), "Reset To Global Position");
                    foreach (var sel in selection)
                        sel.transform.position = Vector3.zero;
                }

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Local Position"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                {
                    var sel = Selection.gameObjects;
                    Undo.RecordObjects(sel.Select(x => x.transform).ToArray(), "Reset To Parent Position");
                    foreach (var s in sel)
                        s.transform.localPosition = Vector3.zero;
                }
            }

            // Rotation
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Global Rotation"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                {
                    var sel = Selection.gameObjects;
                    Undo.RecordObjects(sel.Select(x => x.transform).ToArray(), "Reset Global Rotation");
                    for (var i = 0; i < sel.Length; ++i)
                        sel[i].transform.rotation = Quaternion.identity;
                }

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Local Rotation"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                {
                    var sel = Selection.gameObjects;
                    Undo.RecordObjects(sel.Select(x => x.transform).ToArray(), "Reset To Parent Rotation");
                    for (var i = 0; i < sel.Length; ++i)
                        sel[i].transform.localRotation = Quaternion.identity;
                }
            }

            // Snap rotation
            using (new HorizontalCenteredScope())
            {
                var originalColour = GUI.backgroundColor;
                var originalContentColour = GUI.contentColor;

                GUI.backgroundColor = new Color(1.0f, 0.467f, 0.465f);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("X 90"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.right, -90);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("X 180"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.right, 180);

                GUI.backgroundColor = new Color(0.467f, 1.0f, 0.514f);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Y 90"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.up, -90);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Y 180"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.up, 180);

                GUI.backgroundColor = new Color(0.467f, 0.67f, 1.0f);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Z 90"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.forward, 90);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Z 180"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.Rotate(Vector3.forward, 180);

                GUI.backgroundColor = originalColour;
                GUI.contentColor = originalContentColour;
            }

            // Mirror Tools
            using (new HorizontalCenteredScope())
            {
                var originalColour = GUI.backgroundColor;

                GUI.backgroundColor = m_backgroundRed;
                
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Mirror X"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth),
                    GUILayout.Height(30)))
                    BlockoutStaticFunctions.Mirror(-Vector3.right);
                GUI.backgroundColor = m_backgroundGreen;
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Mirror Y"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth),
                    GUILayout.Height(30)))
                    BlockoutStaticFunctions.Mirror(-Vector3.up);
                GUI.backgroundColor = m_backgroundBlue;
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Mirror Z"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth),
                    GUILayout.Height(30)))
                    BlockoutStaticFunctions.Mirror(-Vector3.forward);

                GUI.backgroundColor = originalColour;
            }

        }
    }
}
