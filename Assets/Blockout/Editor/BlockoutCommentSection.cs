

using UnityEngine;

namespace RadicalForge.Blockout
{
    public class BlockoutCommentSection : BlockoutEditorSection
    {

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Comments Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Comment_Header"), "")),
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
            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Comments Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;


        }
    }
}
