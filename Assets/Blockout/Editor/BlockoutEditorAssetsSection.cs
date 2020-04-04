using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorAssetsSection : BlockoutEditorSection
    {
        private GUIContent[] m_gridIconLabels, m_themeLabels;
        private Color[] m_userThemeColors = new Color[8];
        private readonly bool[] m_lockColors = new bool[8];
        private bool m_userPalletFoldout;
        private bool globalLocked = false;
        private bool lockedVisability = false;
        private GameObject m_targetReplacementAsset;
        private readonly string[] m_colorStrings =
            {"Floor", "Wall", "Dynamic", "Trim", "Foliage", "Leaves", "Water", "Trigger"};
        

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);


            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Asset Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Blockout_Assets"), "")),

                new EUISectionContent("Randomize Pallet", new GUIContent("Randomize Pallet", "Generates and applies a random color pallet")),
                new EUISectionContent("Apply Pallet", new GUIContent("Apply User Pallet", "Applies the last generated random color pallet as the theme")),
                new EUISectionContent("Edit Pallet", new GUIContent("Edit User Pallet", "Shows the individual colors for the user pallet")),
                new EUISectionContent("Lock Color Label", new GUIContent("", "Lock this color choice from random generation?")),

                new EUISectionContent("Replace Asset", new GUIContent("Replace Asset", "Replaces all selected assets with the requested asset to the left mating the original transforms of the selection")),
                new EUISectionContent("Select Asset", new GUIContent("Select Asset", "Selects the object in the project window if it is a prefab")),

                new EUISectionContent("Block Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Blocks"), "Jumps to the blocks folder in the blockout project folder")),
                new EUISectionContent("Walls Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Walls"), "Jumps to the walls folder in the blockout project folder")),
                new EUISectionContent("Floors Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Floors"), "Jumps to the floors folder in the blockout project folder")),
                new EUISectionContent("Dynamic Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Dynamic"), "Jumps to the dynamic folder in the blockout project folder")),
                new EUISectionContent("Foliage Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Foliage"), "Jumps to the foliage folder in the blockout project folder")),
                new EUISectionContent("Particles Quickjump", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Button_Particles"), "Jumps to the particles folder in the blockout project folder")),

                new EUISectionContent("Lock Asset", new GUIContent("Toggle Locked", "Locks / Unlocks the selected assets")),
                new EUISectionContent("Toggle Locked Asset", new GUIContent("Toggle Locked (All)", "Locks / unlocks all assets")),
                new EUISectionContent("Toggle Lockd Visability", new GUIContent("Locked Visability", "Hides / Shows the assets locked state"))
            };

            m_gridIconLabels = new[]
            {
                new GUIContent(EUIResourceManager.Instance.GridIcons[0], string.Format("Select texture {0:D} to be applied", 1)),
                new GUIContent(EUIResourceManager.Instance.GridIcons[1], string.Format("Select texture {0:D} to be applied", 2)),
                new GUIContent(EUIResourceManager.Instance.GridIcons[2], string.Format("Select texture {0:D} to be applied", 3)),
                new GUIContent(EUIResourceManager.Instance.GridIcons[3], string.Format("Select texture {0:D} to be applied", 4)),
                new GUIContent(EUIResourceManager.Instance.GridIcons[4], string.Format("Select texture {0:D} to be applied", 5)),
                new GUIContent(EUIResourceManager.Instance.GridIcons[5], string.Format("Select texture {0:D} to be applied", 6))
            };

            m_themeLabels = new[]
            {
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[0], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 1)),
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[1], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 2)),
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[2], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 3)),
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[3], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 4)),
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[4], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 5)),
                new GUIContent(EUIResourceManager.Instance.ThemeIcons[5], string.Format("Select theme {0:D} to be applied to the blockout hierarchy", 6))
            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);

        }
        


        public override void DrawSection()
        {
            repaint = false;

            GUILayout.Space(10);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Asset Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(236), GUILayout.MaxHeight(30));
                var blockHelpers = Selection.gameObjects.ToList().Where(x => x.GetComponent<BlockoutHelper>()).Select(x => x.GetComponent<BlockoutHelper>()).ToArray();
                int locked = blockHelpers.Where(x => x.Locked).ToList().Count;
                if (locked > 0)
                {
                    using (new VerticalCenteredScope(GUILayout.MaxHeight(30)))
                    {
                        using (new HorizontalCenteredScope())
                        {
                            GUILayout.Label(String.Format("{0} object(s) locked!", locked), GUILayout.MaxWidth(120));
                            GUILayout.Label(EUIResourceManager.Instance.GetTexture("Lock_Closed"), GUILayout.Height(15),
                                            GUILayout.Width(15));
                        }
                    }
                }
                else
                    GUILayout.Space(125);
            }

            // Draw quick jump
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Block Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Block_1x1x1", "Block_Slope_1x1x1");

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Walls Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Wall_025x1x1", "Wall_025x3x1");

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Floors Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Floor_1x-025x1", "Floor_Angle_3x - 025x3");

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Dynamic Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Barrel", "Crate_1x1x1");

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Foliage Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Bush_1", "Vines_Large");

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Particles Quickjump"),
                    GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    BlockoutStaticFunctions.PingAssetInProjectWindow("Fire_Ground_1x1", "Water_Drip_1");
            }

            // Draw grid textures
            using (new HorizontalCenteredScope())
            {
                // Loop through all the loaded grid textures and display them in a button.
                // If its selected then apply that texture to every gameobject in the scene
                for (var i = 0; i < EUIResourceManager.Instance.GridTextures.Length; ++i)
                {
                    if (i >= EUIResourceManager.Instance.GridIcons.Length)
                        continue;
                    if (GUILayout.Button(m_gridIconLabels[i],
                        GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    {
                        BlockoutEditorSettings.CurrentGirdTexture = i;
                        BlockoutStaticFunctions.ApplyTextureIncChildren(EUIResourceManager.Instance.GridTextures[BlockoutEditorSettings.CurrentGirdTexture], blockoutHierarchy.root);
                    }
                }
            }

            // Draw themes
            using (new HorizontalCenteredScope())
            {
                for (var i = 0; i < EUIResourceManager.Instance.BlockoutThemes.Length; ++i)
                {
                    if (GUILayout.Button(m_themeLabels[i],
                        GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth / 2)))
                    {
                        BlockoutEditorSettings.CurrentMaterialTheme = i;
                        BlockoutEditorSettings.CurrentPallet = PalletType.Preset;
                        BlockoutStaticFunctions.ApplyCurrentTheme();
                    }
                }
            }

            // User color pallet
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Randomize Pallet"),
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                {
                    GenerateRandomTheme();
                    BlockoutEditorSettings.CurrentMaterialTheme = EUIResourceManager.Instance.BlockoutThemes.Length + 1;
                    BlockoutStaticFunctions.ApplyTheme(EUIResourceManager.Instance.UserTheme);
                    BlockoutEditorSettings.CurrentPallet = PalletType.User;
                }
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Apply Pallet"),
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                {
                    BlockoutEditorSettings.CurrentMaterialTheme = EUIResourceManager.Instance.BlockoutThemes.Length + 1;
                    BlockoutStaticFunctions.ApplyTheme(EUIResourceManager.Instance.UserTheme);
                    BlockoutEditorSettings.CurrentPallet = PalletType.User;
                }
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Edit Pallet"), 
                    GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    m_userPalletFoldout = !m_userPalletFoldout;
            }

            // User color pickers
            if (m_userPalletFoldout)
            {
                using (new HorizontalCenteredScope())
                {
                    using (new GUILayout.VerticalScope(GUILayout.MaxWidth(375)))
                    {
                        if (!Application.isPlaying && Application.isEditor)
                            for (var i = 0; i < m_colorStrings.Length; ++i)
                            {
                                GUILayout.BeginHorizontal();
                                m_userThemeColors[i] = EditorGUILayout.ColorField(m_colorStrings[i], m_userThemeColors[i]);

                                m_lockColors[i] = GUILayout.Toggle(m_lockColors[i],
                                    EUIResourceManager.Instance.GetContent("Lock Color"),
                                    EUIResourceManager.Instance.Skin.FindStyle("padlock"), GUILayout.Height(15),
                                    GUILayout.Width(15));
                                GUILayout.EndHorizontal();
                            }
                        else
                            GUILayout.Label("Feature not availble during play mode.");
                        GUILayout.Space(5);
                    }
                }
            }


            // Replace Assets
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Select Asset"), GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                    BlockoutStaticFunctions.SelectAsset();

                if (!Application.isPlaying && Application.isEditor)
                {
                    m_targetReplacementAsset =
                        EditorGUILayout.ObjectField(new GUIContent(""), m_targetReplacementAsset,
                            typeof(GameObject), false, GUILayout.Width(124)) as GameObject;

                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Replace Asset"), GUILayout.Width(124)))
                    {
                        BlockoutStaticFunctions.ReplaceObject(Selection.gameObjects, m_targetReplacementAsset);
                    }
                }
                else
                {
                    GUILayout.Label("Feature not availble during play mode.");
                }
            }


            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Lock Asset"),
                                     GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                {
                    Selection.gameObjects.ToList().Where(x => x.GetComponent<BlockoutHelper>()).Select(x => x.GetComponent<BlockoutHelper>()).ToList().ForEach(x => x.SetLock(!x.Locked));
                }

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Toggle Locked Asset"),
                                     GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                {
                    globalLocked = !globalLocked;
                    var helpers = FindObjectsOfType<BlockoutHelper>().ToList();
                    helpers.ForEach(x => x.SetLock(globalLocked));
                }

                var content = EUIResourceManager.Instance.GetContent("Toggle Lockd Visability");
                content.text = lockedVisability ? "Show Locked (On)" : "Show Locked (Off)";

                if (GUILayout.Button(content, GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth)))
                {
                    lockedVisability = !lockedVisability;
                    var helpers = FindObjectsOfType<BlockoutHelper>().ToList();
                    helpers.ForEach(x =>
                    {
                        if(lockedVisability)
                            x.ShowLockedState();
                        else
                            x.HideLockedState();
                    });
                }

            }

        }

        /// <summary>
        ///     Generates a random theme.
        /// </summary>
        private void GenerateRandomTheme()
        {
            var userTheme = EUIResourceManager.Instance.UserTheme;
            var baseTheme = Random.Range(0, EUIResourceManager.Instance.BlockoutThemes.Length);
            var mats = EUIResourceManager.Instance.BlockoutThemes[baseTheme].GetSortedUniqueMaterials;
            var randomMat = Random.Range(0, mats.Length);
            var baseThemeColor = mats[randomMat].GetColor("_Color");
            if (!m_lockColors[2])
                userTheme.DynamicMaterial.SetColor("_Color", baseThemeColor);

            float h, s, v;
            Color.RGBToHSV(baseThemeColor, out h, out s, out v);
            var pallet = BlockoutStaticFunctions.GenerateColors_SaturationLuminance(mats.Length, h);
            var sortedPallet = pallet.OrderBy(x => BlockoutStaticFunctions.Luminance(x)).ToList();

            for (var i = 0; i < sortedPallet.Count; ++i)
            {
                Color.RGBToHSV(sortedPallet[i], out h, out s, out v);
                s *= 0.5f;
                sortedPallet[i] = Color.HSVToRGB(h, s, v);
            }

            if (!m_lockColors[0])
            {
                userTheme.FloorMaterial.SetColor("_Color", sortedPallet[0]);
                userTheme.TriFloor.SetColor("_Color", sortedPallet[0]);
            }
            if (!m_lockColors[1])
            {
                userTheme.WallMaterial.SetColor("_Color", sortedPallet[1]);
                userTheme.TriWalls.SetColor("_Color", sortedPallet[1]);
            }
            if (!m_lockColors[4])
            {
                userTheme.TrimMaterial.SetColor("_Color", sortedPallet[2]);
                userTheme.TriTrim.SetColor("_Color", sortedPallet[2]);
            }
            if (!m_lockColors[5])
                userTheme.FoliageMaterial.SetColor("_Color", sortedPallet[3]);

            if (!m_lockColors[7])
            {
                userTheme.TriggerMaterial.SetColor("_Color", sortedPallet[0]);
                userTheme.TriggerMaterial.SetColor("_Color_1", sortedPallet[0]);
            }

            if (!m_lockColors[5])
            {
                Color.RGBToHSV(sortedPallet[1], out h, out s, out v);
                var leafMatOptions = BlockoutStaticFunctions.GenerateColors_Saturation(1, h, v);
                userTheme.LeavesMaterial.SetColor("_Color", leafMatOptions[0] * 0.5f);
                userTheme.LeavesMaterial.SetColor("_Color_1", leafMatOptions[0] * 0.5f);
            }


            m_userThemeColors[0] = userTheme.FloorMaterial.GetColor("_Color");
            m_userThemeColors[1] = userTheme.WallMaterial.GetColor("_Color");
            m_userThemeColors[2] = userTheme.DynamicMaterial.GetColor("_Color");
            m_userThemeColors[3] = userTheme.TrimMaterial.GetColor("_Color");
            m_userThemeColors[4] = userTheme.FoliageMaterial.GetColor("_Color");
            m_userThemeColors[5] = userTheme.LeavesMaterial.GetColor("_Color_1");
            m_userThemeColors[6] = userTheme.WaterMateral.GetColor("_Color_1");
            m_userThemeColors[7] = userTheme.TriggerMaterial.GetColor("_Color_1");
        }
    }
}