using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorHierarchyToolsSection : BlockoutEditorSection
    {
        private string m_parentName = "";
        private Transform m_targetHeirachyBaseObject;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Hierarchy Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Heirachy_Tools"), "")),
                new EUISectionContent("Create Parent", new GUIContent("Create Parent", "Create a parent object and forces selected object to be children of the new parent")),
                new EUISectionContent("Hierarchy Selected", new GUIContent("Hierarchy Selected", "Hierarchy selected GameObjects to the target parent")),
                new EUISectionContent("Parent Floor", new GUIContent("Floors", "Reparent selected gameobjects to the 'Floor' obejct in the blockout hierarchy")),
                new EUISectionContent("Parent Walls", new GUIContent("Walls", "Reparent selected gameobjects to the 'Walls' obejct in the blockout hierarchy")),
                new EUISectionContent("Parent Trim", new GUIContent("Trim", "Reparent selected gameobjects to the 'Trim' obejct in the blockout hierarchy")),
                new EUISectionContent("Parent Dynamic", new GUIContent("Dynamic", "Reparent selected gameobjects to the 'Dynamic' obejct in the blockout hierarchy")),
                new EUISectionContent("Parent Foliage", new GUIContent("Foliage", "Reparent selected gameobjects to the 'Foliage' obejct in the blockout hierarchy")),
                new EUISectionContent("Parent Particles", new GUIContent("Particles", "Reparent selected gameobjects to the 'Particles' obejct in the blockout hierarchy")),
                new EUISectionContent("Create Prefab", new GUIContent("Create Prefab", "Creates a prefab for EACH SELECTED gameobject."))

            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }

        public override void DrawSection()
        {
            repaint = false;
            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Hierarchy Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            using (new HorizontalCenteredScope())
            {
                GUILayout.Label("Name:");
                GUILayout.Space(5);
                m_parentName = GUILayout.TextField(m_parentName, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));
                GUILayout.Space(18);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Create Parent"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    BlockoutStaticFunctions.CreateParentAndForceChild(m_parentName, blockoutHierarchy);
            }

            using (new HorizontalCenteredScope())
            {
                GUILayout.Label("Target: ");
                if (!Application.isPlaying && Application.isEditor)
                {
                    m_targetHeirachyBaseObject = EditorGUILayout.ObjectField(m_targetHeirachyBaseObject, typeof(Transform), true) as Transform;

                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Hierarchy Selected"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    {
                        if(m_targetHeirachyBaseObject != null)
                        {
                            using (new UndoScope("Rehierarchy Selected Objects"))
                            {
                                Selection.transforms.ToList().ForEach(x => Undo.SetTransformParent(x, m_targetHeirachyBaseObject, ""));
                                EditorGUIUtility.PingObject(m_targetHeirachyBaseObject);
                                Selection.activeGameObject = m_targetHeirachyBaseObject.gameObject;
                            }
                        }
                        else
                            Debug.LogError("BLOCKOUT :: Assign a target object to hierarchy to!");
                    }
                }
                else
                {
                    GUILayout.Label("Feature Not available during play mode");
                }
            }

            GUILayout.Label("Reparent Selection");

            using (new HorizontalCenteredScope())
            {
                GUILayout.Space(4);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Floor"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Floors);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Walls"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Walls);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Trim"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Trim);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Dynamic"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Dynamic);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Foliage"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Foliage);
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Parent Particles"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    ApplyParentTransformFromSection(Selection.gameObjects, SectionID.Particles);
            }

            if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Create Prefab")))
                AutoPrefabSelection.PrefabSelection();
        }

        /// Apply a new parent transform to the target gameobjects
        /// <param name="targets"> The gameobjects to reparent</param>
        /// <param name="blockoutSection"> The new section that it needs to find in the level above to parent to</param>
        private void ApplyParentTransformFromSection(GameObject[] targets, SectionID blockoutSection)
        {
            for (var i = 0; i < targets.Length; ++i)
            {
                var vl = targets[i].GetComponentsInParent<BlockoutSection>().Where(x => x.Section == SectionID.Root)
                    .ToList();

                var localRoot = vl.Count > 0 ? vl[0].gameObject : blockoutHierarchy.root.gameObject;
                BlockoutStaticFunctions.CreateBlockoutSubHeirachyWithRoot(localRoot.transform, localRoot.name + "_");

                var targetTransform = localRoot.GetComponentsInChildren<BlockoutSection>()
                    .Where(x => x.Section == blockoutSection)
                    .ToList()[0].transform;

                targets[i].transform.SetParent(targetTransform);

                BlockoutStaticFunctions.TrimTargetBlockoutHierarchy(localRoot);
            }
            BlockoutStaticFunctions.ApplyCurrentTheme();
        }

    }
}
