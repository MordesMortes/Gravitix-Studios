using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RadicalForge.Blockout
{
    public class BlockoutEditorCommentsSection : BlockoutEditorSection
    {
        public bool showSceneInformation = false;
        private bool showSceneInformationSave = false;
        private bool showSceneInformationInternal = true;
        protected bool showAreaComments = true;
        protected bool showPinComments = true;
        protected bool showAreaCommentsInternal = false;
        protected bool showPinCommentsInternal = false;
        protected Color sceneViewTextColorInternal, sceneViewTextColor = Color.white;
        protected bool initiallyVisabilityStateInGame = false;
        protected string m_parentName = "";
        protected string pinCommentName = "";
        protected string areaCommentName = "";
        protected int currentCommentTexture = 0;
        public int commentPinToPlace = -1;

        public Notepad selectedNote, GlobalNotes;
        protected Color commentColor;

        public GameObject[] pinObjects;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Comments Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Icon_Comments"), "")), 
                new EUISectionContent("Comment Info Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Comment_Info"), "")),

                new EUISectionContent("Show Scene Info", new GUIContent("Show Scene Information", "Enables the scene view UI for the comments system")),
                new EUISectionContent("Area Comment Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Area_Comments"), "")),
                new EUISectionContent("Pin Comment Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Pin_Comments"), "")),
                new EUISectionContent("Click In Scene", new GUIContent(EUIResourceManager.Instance.GetTexture("Click_In_The_Scene"), "")),
                new EUISectionContent("Back Button", new GUIContent(EUIResourceManager.Instance.GetTexture("Button_Back"), "Cancels placing a pin comment"))

            };

            pinObjects = Resources.LoadAll<GameObject>("Blockout/Pins").OrderBy(x => x.name).ToArray();

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);
        }

        public override void DrawSection()
        {
            repaint = false;

            GlobalNotes = SceneManager.GetActiveScene().GetRootGameObjects().Where(x => x.GetComponent<Notepad>()).Select(x => x.GetComponent<Notepad>()).FirstOrDefault();
            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Comments Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
            {
                if (showSceneInformation)
                {
                    showSceneInformationSave = true;
                    showSceneInformation = false;
                }
                return;
            }

            if (showSceneInformationSave && !showSceneInformation)
            {
                showSceneInformation = true;
                showSceneInformationSave = false;
            }

            DrawCommentHeader();
            
            if (selectedNote)
                DrawSelectedComment();

            DrawCommentCreation();

        }

        public override void Update()
        {
            if (showSceneInformationInternal != showSceneInformation)
            {
                showSceneInformationInternal = showSceneInformation;
                CommentBoxSceneGUI.GlobalNotes = GlobalNotes;

                if (showSceneInformation)
                    CommentBoxSceneGUI.Enable();
                else
                    CommentBoxSceneGUI.Disable();

                if (SceneView.lastActiveSceneView)
                    SceneView.lastActiveSceneView.Repaint();
            }

            if (showAreaCommentsInternal != showAreaComments)
            {
                CommentBoxSceneGUI.showAreaComments = showAreaComments;
                showAreaCommentsInternal = showAreaComments;
            }

            if (showPinCommentsInternal != showPinComments)
            {
                CommentBoxSceneGUI.showPinComments = showPinComments;
                showPinCommentsInternal = showPinComments;
            }

            if (CommentBoxSceneGUI.ShowCommentInfoInternal != CommentBoxSceneGUI.ShowCommentInfo)
            {
                CommentBoxSceneGUI.GlobalNotes = GlobalNotes;
                if (SceneView.currentDrawingSceneView)
                    SceneView.currentDrawingSceneView.Repaint();
                else if (SceneView.lastActiveSceneView)
                    SceneView.lastActiveSceneView.Repaint();
                CommentBoxSceneGUI.ShowCommentInfoInternal = CommentBoxSceneGUI.ShowCommentInfo;
            }

            // Repaint window is a area / pin comment is selected
            if (Selection.activeGameObject)
            {
                if (Selection.activeGameObject.GetComponentInParent<BlockoutPinGizmo>())
                {
                    selectedNote = Selection.activeGameObject.GetComponentInParent<BlockoutPinGizmo>()
                                            .GetComponent<Notepad>();
                    repaint = true;
                }
                else if (Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>())
                {
                    selectedNote = Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>()
                                            .GetComponent<Notepad>();
                    repaint = true;
                }
                else if (Selection.activeGameObject.GetComponent<Notepad>())
                {
                    selectedNote = Selection.activeGameObject.GetComponent<Notepad>();
                    repaint = true;
                }
                else
                {
                    selectedNote = null;
                    repaint = true;
                }
            }
            else
            {
                selectedNote = null;
                repaint = true;
            }

            // Set text colour if chanaged
            if (sceneViewTextColor != sceneViewTextColorInternal)
            {
                CommentBoxSceneGUI.textColor = sceneViewTextColor;
                sceneViewTextColorInternal = sceneViewTextColor;
                if(SceneView.lastActiveSceneView)
                    SceneView.lastActiveSceneView.Repaint();
            }
        }

        public void DrawCommentHeader()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                showSceneInformation = GUILayout.Toggle(showSceneInformation, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight));
                GUILayout.Label(EUIResourceManager.Instance.GetContent("Show Scene Info"));
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                showAreaComments = GUILayout.Toggle(showAreaComments, "",
                                                    EUIResourceManager.Instance.Skin.button,
                                                    GUILayout.Height(EditorGUIUtility
                                                                         .singleLineHeight),
                                                    GUILayout.Width(EditorGUIUtility
                                                                        .singleLineHeight));
                GUILayout.Label("Show Area Comments");
                GUILayout.FlexibleSpace();
                showPinComments = GUILayout.Toggle(showPinComments, "",
                                                   EUIResourceManager.Instance.Skin.button,
                                                   GUILayout.Height(EditorGUIUtility
                                                                        .singleLineHeight),
                                                   GUILayout.Width(EditorGUIUtility
                                                                       .singleLineHeight));
                GUILayout.Label("Show Pin Comments");
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                GUILayout.Label("Scene View Text Color:");
                if (!Application.isPlaying && Application.isEditor)
                    sceneViewTextColor = EditorGUILayout.ColorField(sceneViewTextColor, GUILayout.MaxWidth(175));
                else
                    GUILayout.Label("Feature Not Available during play mode");
            }

            if (GUILayout.Button("Edit Global Notes"))
            {
                Selection.activeGameObject = blockoutHierarchy.root.gameObject;
                var pt = Assembly.GetAssembly(typeof(Editor)).GetType("UnityEditor.InspectorWindow");
                EditorWindow.GetWindow(pt).Show();
            }
        }

        public void DrawCommentCreation()
        {
            #region Area Comments

            GUILayout.Box(EUIResourceManager.Instance.GetContent("Area Comment Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(256), GUILayout.Height(24));


            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                initiallyVisabilityStateInGame = GUILayout.Toggle(initiallyVisabilityStateInGame, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight));
                GUILayout.Label("Initial Visibility State In Game");
            }

            using (new HorizontalCenteredScope())
            {
                GUILayout.Space(5);
                GUILayout.Label("Name:");
                GUILayout.Space(5);
                areaCommentName = GUILayout.TextField(areaCommentName, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth - 8));

                if (GUILayout.Button("Create Area Comment", GUILayout.Width(BlockoutEditorSettings.ThreeColumnWidth + 18)))
                    CreateAreaComment(areaCommentName);
            }

            using (new HorizontalCenteredScope())
            {

                // Loop through all the loaded grid textures and display them in a button.
                // If its selected then apply that texture to every gameobject in the scene
                for (var i = 0; i < EUIResourceManager.Instance.AreaCommentTextures.Length; ++i)
                {
                    if (i >= EUIResourceManager.Instance.AreaCommentIcons.Length)
                        continue;
                    if (GUILayout.Button(EUIResourceManager.Instance.AreaCommentIcons[i], GUILayout.Height(BlockoutEditorSettings.SixColumnWidth),
                                         GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                    {
                        currentCommentTexture = i;
                        SetAreaCommentTexture();
                    }
                }

            }
            using (new HorizontalCenteredScope())
            {
                if (GUILayout.Button("Reparent Objects in comment area", GUILayout.Width(BlockoutEditorSettings.OneColumnWidth)))
                {
                    if (Selection.activeGameObject)
                        if (Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>())
                            ReparentToBoundsContent(Selection.activeGameObject,
                                                    Selection.activeGameObject.GetComponent<Collider>().bounds);
                }
            }

            #endregion

            #region Pin Comments

            GUILayout.Box(EUIResourceManager.Instance.GetContent("Pin Comment Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(256), GUILayout.Height(24));


            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Space(5);
                initiallyVisabilityStateInGame = GUILayout.Toggle(initiallyVisabilityStateInGame, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.singleLineHeight));
                GUILayout.Label("Initial Visibility State In Game");
            }

            using (new HorizontalCenteredScope())
            {
                GUILayout.Label("Name:");
                GUILayout.Space(10);
                pinCommentName = GUILayout.TextField(pinCommentName, GUILayout.Width(BlockoutEditorSettings.OneColumnWidth - BlockoutEditorSettings.SixColumnWidth));

            }

            using (new HorizontalCenteredScope())
            {
                if (commentPinToPlace >= 0)
                {
                    if (GUILayout.Button(EUIResourceManager.Instance.GetContent("Back Button"),
                                         GUILayout.Height(BlockoutEditorSettings.SixColumnWidth), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                        commentPinToPlace = -1;
                    GUILayout.Label(EUIResourceManager.Instance.GetContent("Click In Scene"), GUILayout.Height(BlockoutEditorSettings.SixColumnWidth), GUILayout.Width(BlockoutEditorSettings.OneColumnWidth - BlockoutEditorSettings.SixColumnWidth - 1));
                }
                else
                {
                    for (var i = 0; i < 6; ++i)
                    {
                        if (GUILayout.Button(EUIResourceManager.Instance.PinCommentIcons[i],
                                             GUILayout.Height(BlockoutEditorSettings.SixColumnWidth), GUILayout.Width(BlockoutEditorSettings.SixColumnWidth)))
                            commentPinToPlace = i;
                    }
                }
            }

            if (commentPinToPlace >= 0)
                GUILayout.Space(2);
            #endregion
        }

        void DrawSelectedComment()
        {

            GUILayout.Box(EUIResourceManager.Instance.GetContent("Comment Info Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(256), GUILayout.Height(24));


            using (new HorizontalCenteredScope())
            {
                GUILayout.Label("Note name:", GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));
                selectedNote.gameObject.name = GUILayout.TextField(selectedNote.gameObject.name, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));
            }


            using (new HorizontalCenteredScope())
            {
                GUILayout.Label("Comment Color:", GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));

                if (Selection.activeGameObject)
                {
                    if (Selection.activeGameObject.GetComponent<BlockoutPinGizmo>())
                        commentColor = Selection.activeGameObject.GetComponent<BlockoutPinGizmo>()
                                                .volumeColor;
                    else if (Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>())
                        commentColor = Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>()
                                                .volumeColor;

                    if (!Application.isPlaying && Application.isEditor)
                        commentColor = EditorGUILayout.ColorField(commentColor, GUILayout.Width(BlockoutEditorSettings.TwoColumnWidth));
                    else
                        GUILayout.Label("Feature Not Available during play mode");

                    if (Selection.activeGameObject.GetComponent<BlockoutPinGizmo>())
                    {
                        Selection.activeGameObject.GetComponent<BlockoutPinGizmo>().volumeColor = commentColor;
                        if (SceneView.lastActiveSceneView)
                            SceneView.lastActiveSceneView.Repaint();
                        repaint = true;
                    }
                    else if (Selection.activeGameObject.GetComponent<BlockoutSceneViewCubeGizmo>())
                    {
                        Selection.gameObjects.Where(x => x.GetComponent<BlockoutSceneViewCubeGizmo>() != null)
                                 .Select(x => x.GetComponent<BlockoutSceneViewCubeGizmo>()).ToList()
                                 .ForEach(x => x.volumeColor = commentColor);
                        if (SceneView.lastActiveSceneView)
                            SceneView.lastActiveSceneView.Repaint();

                        repaint = true;
                    }
                }
            }


            GUILayout.Label("General Notes");
            selectedNote.generalNotes = EditorGUILayout.TextArea(selectedNote.generalNotes,
                                                                 GUILayout.MinHeight(EditorGUIUtility
                                                                                         .singleLineHeight *
                                                                                     3));
            GUILayout.Label("TODO Notes");
            selectedNote.toDoNotes = EditorGUILayout.TextArea(selectedNote.toDoNotes,
                                                              GUILayout.MinHeight(EditorGUIUtility
                                                                                      .singleLineHeight * 3));
            GUILayout.Label("Other Notes");
            selectedNote.otherNotes = EditorGUILayout.TextArea(selectedNote.otherNotes,
                                                               GUILayout.MinHeight(EditorGUIUtility
                                                                                       .singleLineHeight * 3));
        }

        /// <summary>
        ///     Sets the area comment texture.
        /// </summary>
        private void SetAreaCommentTexture()
        {
            var targetRenderers = Selection.gameObjects.Where(x => x.GetComponentsInChildren<Renderer>().Length > 0)
                                           .SelectMany(x => x.GetComponentsInChildren<Renderer>()).ToArray();

            if (targetRenderers.Length > 0)
            {
                var materialsToEdit = new List<Material>();
                foreach (var r in targetRenderers)
                    foreach (var m in r.sharedMaterials)
                    {
                        if (!m) continue;
                        if (m.HasProperty("_Tex"))
                            materialsToEdit.Add(m);
                    }

                Undo.RecordObjects(materialsToEdit.ToArray(), "Assign Area Comment Textures");

                for (var i = 0; i < materialsToEdit.Count; ++i)
                    if (materialsToEdit[i])
                        materialsToEdit[i].SetTexture("_Tex", EUIResourceManager.Instance.AreaCommentTextures[currentCommentTexture]);
            }
        }

        /// <summary>
        ///     Creates an area comment.
        /// </summary>
        private void CreateAreaComment(string commentTargetName = "")
        {
            if (Selection.gameObjects.Length <= 1)
            {
                EditorUtility.DisplayDialog("Blockout Error",
                                            "There should be at least 2 (2+) objects selected! The area comment will encompass these objects",
                                            "Ok");
            }
            else
            {
                var encapsulatingBounds = new Bounds(Selection.gameObjects[0].transform.position, Vector3.one);
                for (var i = 0; i < Selection.gameObjects.Length; ++i)
                {
                    var rend = Selection.gameObjects[i].GetComponent<Renderer>();
                    if (rend)
                        encapsulatingBounds.Encapsulate(rend.bounds);
                }

                var anchorCenter = encapsulatingBounds.center;
                var ext = new Vector3(-encapsulatingBounds.extents.x, encapsulatingBounds.extents.y,
                                      -encapsulatingBounds.extents.z);
                anchorCenter += ext - new Vector3(0.5f, -0.5f, 0.5f);
                var asset = (GameObject)AssetDatabase.LoadAssetAtPath(
                                                                       AssetDatabase.GUIDToAssetPath(AssetDatabase
                                                                                                         .FindAssets("Area_Comment t:prefab")
                                                                                                         [0]),
                                                                       typeof(GameObject));
                var newComment = Instantiate(asset, anchorCenter, Quaternion.identity);
                Undo.RegisterCreatedObjectUndo(newComment, "Create Area Comment");
                showAreaComments = true;
                newComment.name = commentTargetName == "" ? "Area Comment" : commentTargetName;
                newComment.transform.localScale = encapsulatingBounds.size + Vector3.one;
                newComment.transform.SetParent(blockoutHierarchy.comments);
                newComment.GetComponent<BlockoutSceneViewCubeGizmo>().volumeColor = commentColor;
                Selection.activeGameObject = newComment;
                SceneView.lastActiveSceneView.FrameSelected();
            }
        }

        /// <summary>
        ///     Reparents and gameobject within the bounds of target object.
        /// </summary>
        /// <param name="targetObject">The target object.</param>
        /// <param name="target">The target.</param>
        private void ReparentToBoundsContent(GameObject targetObject, Bounds target)
        {

            using (new UndoScope("Reparent Objects to comment"))
            {

                var allObjects = FindObjectsOfType<GameObject>().Select(x => x.transform).Where(
                                                                                                x =>
                                                                                                {
                                                                                                    if (x.GetComponent<
                                                                                                        BlockoutSection
                                                                                                    >())
                                                                                                        return false;
                                                                                                    if (x.parent == null
                                                                                                    )
                                                                                                        return false;
                                                                                                    if (x.parent ==
                                                                                                        targetObject)
                                                                                                        return false;
                                                                                                    if (x
                                                                                                        .parent
                                                                                                        .GetComponent<
                                                                                                            BlockoutSection
                                                                                                        >())
                                                                                                        return true;
                                                                                                    return false;
                                                                                                }
                                                                                               ).ToArray();
                if (!targetObject.GetComponent<BlockoutSection>())
                    targetObject.AddComponent<BlockoutSection>().Section = SectionID.Root;
                BlockoutStaticFunctions.CreateBlockoutSubHeirachyWithRoot(targetObject.transform,
                                                                          targetObject.name + "_");

                Undo.RecordObjects(allObjects, "Reparent Objects");

                for (var i = 0; i < allObjects.Length; ++i)
                {
                    Bounds colliderBounds;
                    if (allObjects[i].GetComponent<Collider>())
                        colliderBounds = allObjects[i].GetComponent<Collider>().bounds;
                    else if (allObjects[i].GetComponent<Renderer>())
                        colliderBounds = allObjects[i].GetComponent<Renderer>().bounds;
                    else
                        continue;

                    if (target.Contains(colliderBounds.max) && target.Contains(colliderBounds.min))
                    {
                        var section = allObjects[i].transform.parent.GetComponent<BlockoutSection>();
                        if (section)
                            BlockoutStaticFunctions.ReparentObjectToTargetRoot(allObjects[i].transform,
                                                                               targetObject.transform);
                    }
                }

                BlockoutStaticFunctions.TrimTargetBlockoutHierarchy(targetObject);

            }
        }
    }
}
