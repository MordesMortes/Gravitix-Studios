using System;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorScalableObjectsSection : BlockoutEditorSection
    {
        private GameObject m_scalableWallPrefab;
        private GameObject m_scalableFloorPrefab;
        private GameObject m_scalableTrimPrefab;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            
            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Scalable Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Scalable_Object"), "")),

                new EUISectionContent("Scalable Wall", new GUIContent("Create Scalable" + Environment.NewLine + "Wall Block", "Creats a scalable wall base")),
                new EUISectionContent("Scalable Floor", new GUIContent("Create Scalable" + Environment.NewLine + "Floor Block", "Creats a scalable floor base")),
                new EUISectionContent("Scalable Trim", new GUIContent("Create Scalable" + Environment.NewLine + "Trim Block", "Creats a scalable trim base")),
            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);

            m_scalableWallPrefab = (GameObject) AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Tri_Wall t:prefab")[0]),
                typeof(GameObject));

            m_scalableFloorPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Tri_Floor t:prefab")[0]),
                typeof(GameObject));

            m_scalableTrimPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Tri_Trim t:prefab")[0]),
                typeof(GameObject));
        }

        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Scalable Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            using (new HorizontalCenteredScope())
            {

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Scalable Wall"),
                    GUILayout.Height(BlockoutEditorSettings.TwoLineHeight),
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    CreateTriPlanerAsset(m_scalableWallPrefab);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Scalable Floor"),
                    GUILayout.Height(BlockoutEditorSettings.TwoLineHeight),
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    CreateTriPlanerAsset(m_scalableFloorPrefab);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Scalable Trim"),
                    GUILayout.Height(BlockoutEditorSettings.TwoLineHeight),
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    CreateTriPlanerAsset(m_scalableTrimPrefab);
            }
        }

        /// <summary>
        ///     Creates a tri planer asset in front of the camera.
        /// </summary>
        /// <param name="prefab">The target tri planar prefab</param>
        private void CreateTriPlanerAsset(GameObject prefab)
        {
            var target = Instantiate(prefab);
            Undo.RegisterCreatedObjectUndo(target, "Created Tri-Planer Asset");

            target.transform.position = BlockoutStaticFunctions.GetSceneViewSpawnPosition();
            target.name = prefab.name + " (Tri-Planar)";
            Selection.activeGameObject = target;
            BlockoutStaticFunctions.SnapPositionSelection();

            SceneView.lastActiveSceneView.FrameSelected();

            Tools.current = Tool.Scale;
            BlockoutStaticFunctions.ApplyCurrentTheme();
        }
    }
}