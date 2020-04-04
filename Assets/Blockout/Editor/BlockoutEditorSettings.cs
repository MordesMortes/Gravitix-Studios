using UnityEditor;

namespace RadicalForge.Blockout
{
    public enum PalletType
    {
        Preset,
        User
    }

    public static class BlockoutEditorSettings
    {
        public static readonly float OneColumnWidth = 380f;
        public static readonly float TwoColumnWidth = 188f;
        public static readonly float ThreeColumnWidth = 124f;
        public static readonly float FourColumnWidth = 92f;
        public static readonly float FiveColumnWidth = 76f;
        public static readonly float SixColumnWidth = 60f;

        public static readonly float TwoLineHeight = EditorGUIUtility.singleLineHeight * 3;

        public static BlockoutSceneSettings[] SceneDefinitions;
        public static BlockoutSceneSettings CurrentSceneSetting;
        public static int CurrentSettingIDX;
        public static int CurrentGirdTexture = 0;
        public static int CurrentMaterialTheme = 0;
        public static PalletType CurrentPallet;
        public static bool AutoSnap = true;

        public static readonly string VERSION = "1.1.0";
    }
}
