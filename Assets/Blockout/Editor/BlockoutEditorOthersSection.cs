using System.Linq;
using UnityEngine;
using UnityEditor;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorOthersSection : BlockoutEditorSection
    {
        private GameObject m_triggerPrefab;
        private bool m_globalTriggerVisibilityAll, m_globalParticlePlayAll, m_continueRulerSpawn;
        private int m_rulerObjectToPlace, m_selectedObjectCount;

        public int rulerObjectToPlace { get { return m_rulerObjectToPlace; } set { m_rulerObjectToPlace = value; } }
        public GameObject rulerPrefab;
        public BlockoutRulerObject previousRuler;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            m_triggerPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("Trigger t:prefab")[0]), typeof(GameObject));

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Other Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Others"))),
                new EUISectionContent("Create Trigger Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Create_Trigger"), "Creates a trigger volume with a blockout trigger script")),
                new EUISectionContent("All Trigger On Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_All_Triggers_On"), "Toggles the visibility of all blockout triggers in game. (Still visible when the game is not running)")),
                new EUISectionContent("All Trigger Off Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_All_Triggers_Off"), "Toggles the visibility of all blockout triggers in game. (Still visible when the game is not running)")),
                new EUISectionContent("Selectd Trigger On Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_Selected_Triggers_On"), "Toggles the visibility of selected blockout triggers in game. (Still visible when the game is not running)")),
                new EUISectionContent("Selectd Trigger Off Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_Selected_Triggers_Off"), "Toggles the visibility of selected blockout triggers in game. (Still visible when the game is not running)")),
                new EUISectionContent("Particle On Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_All_Particles_On"), "Toggles the play state of all particle systems in the editor if a particle system in selected")),
                new EUISectionContent("Particle Off Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Toggle_All_Particles_Off"), "Toggles the play state of all particle systems in the editor if a particle system in selected")),
                new EUISectionContent("Export All Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_Export"), "Exports the entire hierarchy as a single Wavefront Obj file")),
                new EUISectionContent("Export Selected Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_Export_Selected"), "Exports the selected items as a single Wavefront Obj file")),
                new EUISectionContent("Object Count Label", new GUIContent("Objects in scene", "If there are objects selected thoes will be counted, otherwise the amount of gameobjects in the scene will be counted.")),
                new EUISectionContent("Create Ruler Button", new GUIContent("Create Ruler", "Creates a ruler to mesure beteween 2 points")),
                new EUISectionContent("Ruler 1st Marker Button", new GUIContent("Place 1st Marker", "Click in the scene to place the first ruler marker")),
                new EUISectionContent("Ruler 2nd Marker Button", new GUIContent("Place 2st Marker", "Click in the scene to place the seccond ruler marker"))
            };

            rulerPrefab = Resources.LoadAll<GameObject>("Blockout/Ruler").OrderBy(x => x.name).ToList()[0];

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }

        private float internalClock = 0f;
        private float posttimer = 1.5f;


        public override void Update()
        {
            if (m_rulerObjectToPlace == 0 && previousRuler)
            {
                internalClock += Time.deltaTime;
                if (internalClock > posttimer)
                {
                    internalClock = 0f;
                    previousRuler = null;
                }
                else
                {
                    Selection.activeGameObject = previousRuler.gameObject;
                }
            }

            if (Selection.activeGameObject && previousRuler != null)
            {
                if (rulerObjectToPlace > 0)
                    Selection.activeGameObject = previousRuler.gameObject;
            }
        }


        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Other Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Create Trigger"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    CreateTrigger();

                if (GUILayout.Button(m_globalTriggerVisibilityAll ? EUIResourceManager.Instance.GetContent("All Trigger On") : EUIResourceManager.Instance.GetContent("All Trigger Off"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    ToggleTriggerVisibility(true);

                var bts = Selection.gameObjects.ToList().Where(x => x.GetComponent<BlockoutTrigger>()).Select(s => s.GetComponent<BlockoutTrigger>()).ToList();

                if (GUILayout.Button(bts.Count == 0 ? EUIResourceManager.Instance.GetContent("Selectd Trigger Off") : bts[0].visibleInGame ? EUIResourceManager.Instance.GetContent("Selected Trigger On") : EUIResourceManager.Instance.GetContent("Selectd Trigger Off"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                {
                    if (bts.Count == 0)
                        Debug.LogWarning("BLOCKOUT :: Please select some objects with a Blockout Trigger");
                    else
                        ToggleTriggerVisibility(false, !bts[0].visibleInGame);
                }

                if (GUILayout.Button(m_globalParticlePlayAll ? EUIResourceManager.Instance.GetContent("Particle On") : EUIResourceManager.Instance.GetContent("Particle Off"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                {
                    var ps = FindObjectsOfType<BlockoutParticleHelper>();
                    if (ps.Length > 0)
                        Selection.objects = ps.Select(x => x.gameObject).Cast<Object>().ToArray();
                    ToggleParticleSystems(true);
                }

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Export All"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                {
                    Selection.activeGameObject = blockoutHierarchy.root.gameObject;
                    EditorObjExporter.ExportWholeSelectionToSingle();
                }

                if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Export Selected"), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth)))
                    EditorObjExporter.ExportWholeSelectionToSingle();
                
            }

            using (new HorizontalCenteredScope())
            {
                // Output the amount of gameobjects selected to the console
                var objectCountLabel = EUIResourceManager.Instance.GetContent("Object Count");
                objectCountLabel.text = "Count Objects (" +
                                        (Selection.gameObjects.Length != 0 ? "Selected: " : "Global: ") +
                                        m_selectedObjectCount + ")";

                if (GUILayout.Button(objectCountLabel, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                {
                    m_selectedObjectCount = Selection.gameObjects.Length != 0
                                                ? Selection.gameObjects.Length
                                                : FindObjectsOfType<GameObject>().Length;
                    repaint = true;
                }

                if (m_rulerObjectToPlace == 0)
                {
                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Create Ruler"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)))
                    {
                        m_rulerObjectToPlace++;
                        repaint = true;
                    }
                }
                else if (m_rulerObjectToPlace == 1)
                {
                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Ruler 1st Marker"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)) && m_continueRulerSpawn)
                    {
                        m_continueRulerSpawn = false;
                        m_rulerObjectToPlace++;
                        repaint = true;
                    }
                }
                else if (m_rulerObjectToPlace == 2)
                {
                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Ruler 2nd Marker"), GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth)) && m_continueRulerSpawn)
                    {
                        m_continueRulerSpawn = false;
                        m_rulerObjectToPlace = 3;
                        repaint = true;
                    }
                }
                
            }

        }

        /// <summary>
        /// Creates a Blockout trigger in 10 units in front of the scene camera
        /// </summary>
        void CreateTrigger()
        {
            var target = Instantiate(m_triggerPrefab);
            Undo.RegisterCreatedObjectUndo(target, "Created Blockout Trigger");
            
            target.transform.position = BlockoutStaticFunctions.GetSceneViewSpawnPosition();
            target.transform.SetParent(blockoutHierarchy.triggers);
            target.name = m_triggerPrefab.name;
            Selection.activeGameObject = target;
            BlockoutStaticFunctions.SnapPositionSelection();
            SceneView.lastActiveSceneView.FrameSelected();
        }

        /// <summary>
        ///     Toggles the trigger visibility for both the selected assets and globaly.
        /// </summary>
        /// <param name="global">if set to <c>true</c> [global].</param>
        /// <param name="value">only possible to use if not set globaly</param>
        private void ToggleTriggerVisibility(bool global = false, bool value = false)
        {
            var targets = Selection.gameObjects.Where(x => x.GetComponent<BlockoutTrigger>() != null)
                .Select(x => x.GetComponent<BlockoutTrigger>()).ToArray();

            if (global)
            {
                targets = FindObjectsOfType<BlockoutTrigger>().ToArray();
                m_globalTriggerVisibilityAll = !m_globalTriggerVisibilityAll;
            }

            if (targets.Length <= 0) return;

            using (new UndoScope("Toggle Trigger Visibility"))
            {
                Undo.RecordObjects(targets, "");

                foreach (var target in targets)
                {
                    var trigger = target.GetComponent<BlockoutTrigger>();
                    trigger.visibleInGame = global ? m_globalTriggerVisibilityAll : value;
                }
            }
        }

        /// <summary>
        ///     Toggles the particle systems.
        /// </summary>
        /// <param name="global">if set to <c>true</c> [global].</param>
        private void ToggleParticleSystems(bool global = false)
        {
            var targets = Selection.gameObjects;

            if (global)
            {
                targets = FindObjectsOfType<BlockoutParticleHelper>().Select(x => x.gameObject).ToArray();
                m_globalParticlePlayAll = !m_globalParticlePlayAll;
                Selection.objects = targets.Cast<Object>().ToArray();
            }

            foreach (var target in targets)
            {
                var helper = target.GetComponent<BlockoutParticleHelper>();
                if (helper)
                    helper.ShouldPlay = global ? m_globalParticlePlayAll : !helper.ShouldPlay;
            }
        }

    }

}