using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RadicalForge.Blockout
{
    [System.Serializable]
    public struct BlockoutItemPreview
    {
        public string name;
        public GameObject prefab;
        public Texture2D previewImage;
    }

    

    public class BlockoutEditorSuggestedSection : BlockoutEditorSection
    {

        private static class Styles
        {
            public static readonly string[] TabTypes = new string[]
            {
                "Replace",
                "Create New"
            };
        }

        public List<int> shownItems = new List<int>();

        private static bool overSceneView;
        //private BlockoutEditorWindow parentWindow;
        private GameObject spwanedAsset;
        int amountOfItemsToShow = 6;
        float currentSize = 60;
        Rect bottomBarRect;
        bool dragging = false;
        BlockoutItemPreview[] suggestedItems, favouriteItems;


        public bool visible = false;
        
        

        GameObject selectedObjectCurrent = null;
        
        int swapToPrefab;
        GameObject targetPrefabSwap;
        public bool showQuickPicker = false;
        public bool showScrollPicker = false;
        private Mesh originalMesh;

        private int quickerMode = 0;
        public int selected = 0;
        public int maxPerRow;
        private BlockoutDatabase database;

        public override void InitSection(string sectionName, BlockoutHierarchy hierarchy)
        {
            base.InitSection(sectionName, hierarchy);

            currentSize = BlockoutEditorSettings.SixColumnWidth;

            EUISectionContent[] sectionContents =
            {
                new EUISectionContent("Suggested Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Suggestions"), "")),
                new EUISectionContent("Suggested Assets Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Suggested_Assets"), "")),
                new EUISectionContent("Favourite Assets Header", new GUIContent(EUIResourceManager.Instance.GetTexture("Scene_Favourites"), ""))
            };

            EUIResourceManager.Instance.RegisterGUIContent(sectionContents);

            database = Resources.Load<BlockoutDatabase>("Blockout/BlockoutSuggestionDatabase");

            if (!database)
            {
                var cd = ScriptableObject.CreateInstance<BlockoutDatabase>();
                cd.Database = new BlockoutItemPreview[0];
                Directory.CreateDirectory("Assets/Blockout/Editor/Resources/Blockout/SceneDefinitions");
                AssetDatabase.CreateAsset(cd,
                                          "Assets/Blockout/Editor/Resources/Blockout/BlockoutSuggestionDatabase.asset");
                database = Resources.Load<BlockoutDatabase>("Blockout/BlockoutSuggestionDatabase");
            }

            if(database.Database.Length == 0)
                PreloadAssetData();

            SceneView.onSceneGUIDelegate += OnScene;
        }

        void OnDisable()
        {
            SceneView.onSceneGUIDelegate -= OnScene;
        }

        public override void DrawSection()
        {
            repaint = false;

            using (new GUILayout.HorizontalScope())
            {
                showSection = GUILayout.Toggle(showSection, "", EUIResourceManager.Instance.Skin.button, GUILayout.Height(30), GUILayout.Width(30));
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Suggested Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.MaxWidth(256), GUILayout.MaxHeight(30));
            }

            if (!showSection)
                return;

            // Work out the amount of items that can be dispolayed in the current window state
            amountOfItemsToShow = Mathf.FloorToInt((BlockoutEditorSettings.OneColumnWidth - 55) / currentSize);

            // Draw the suggested assets group of objects

            using (new GUILayout.VerticalScope(GUILayout.MaxWidth(BlockoutEditorSettings.OneColumnWidth)))
            {
                GUILayout.Space(5);

                if(GUILayout.Button("Refresh Suggestions Database"))
                    PreloadAssetData();

                GUILayout.Box(EUIResourceManager.Instance.GetContent("Suggested Assets Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Height(35), GUILayout.MaxWidth(256));
                DrawAssetList(suggestedItems);

                // Draw the scene favourite group of objects
                GUILayout.Box(EUIResourceManager.Instance.GetContent("Favourite Assets Header"), EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Height(35), GUILayout.MaxWidth(256));
                if (favouriteItems != null)
                {
                    if (favouriteItems.Length > 0)
                        DrawAssetList(favouriteItems);
                }
                else
                {
                    GetFavorites();
                    GUILayout.Label("There are no favourite assets!");
                }

            }
            
        }

        void DrawScrollPickerWindow(int windowID)
        {
            if (suggestedItems == null) return;

            using (new HorizontalCenteredScope())
            {
                AssetPreview.SetPreviewTextureCacheSize(suggestedItems.Length + 5);
                for (int i = 0; i < (shownItems.Count >= 5 ? 5 : shownItems.Count); ++i)
                {
                    using (new GUILayout.VerticalScope())
                    {
                        int size = GetSize(i);

                        GUILayout.Space(i == 2 ? 0 : 15);
                        using (new HorizontalCenteredScope())
                        {
                            GUILayout.Box(new GUIContent(suggestedItems[shownItems[i]].previewImage),
                                          EUIResourceManager.Instance.Skin.GetStyle("Texture"), GUILayout.Width(size),
                                          GUILayout.Height(size));
                        }
                        GUILayout.FlexibleSpace();

                        string objectName = suggestedItems[shownItems[i]].name;
                        if (objectName.Length > 15)
                        {
                            string back = objectName.Substring(objectName.Length - 8, 8);
                            string front = objectName.Substring(0, 5);
                            objectName = front + "..." + back;
                        }
                        GUILayout.Label(new GUIContent(objectName));

                    }
                }
            }

        }

        private string searchText = "";
        public Vector2 scroll;
        private BlockoutItemPreview[] searchSuggested;
        public int SuggestedItemCount { get { return searchSuggested.Length; } }

        void DrawQuickPickerWindow(int windowID)
        {
            
            using (new GUILayout.HorizontalScope())
            {
                if (Selection.activeGameObject)
                {
                    quickerMode = GUILayout.SelectionGrid(quickerMode, Styles.TabTypes, Styles.TabTypes.Length,
                                                          EditorStyles.toolbarButton, GUILayout.Width(197));

                    GUILayout.FlexibleSpace();
                }
                GUI.SetNextControlName("Quick Picker Search");

                searchText = GUILayout.TextField(searchText, GUI.skin.FindStyle("ToolbarSeachTextField"), Selection.activeGameObject ? GUILayout.Width(200) : GUILayout.ExpandWidth(true));

                if (previousShowQuickPicker == false)
                {
                    previousShowQuickPicker = true;
                    EditorGUI.FocusTextInControl("Quick Picker Search");
                    searchText = "";
                }
            }

            if (searchText.Trim().Length == 1)
                suggestedItems = GetAssetListFromSelected(searchText.Trim());

            searchSuggested = searchText != "" ? suggestedItems.ToList().Where(x => x.name.Contains(searchText.Trim())).ToArray() : suggestedItems;




            maxPerRow = searchSuggested.Length < 5 ? searchSuggested.Length : 5;
            int rowCount = Mathf.CeilToInt((float)searchSuggested.Length / maxPerRow);

            var e = Event.current;
            

            if (selected < 0)
                selected = 0;
            if (selected >= searchSuggested.Length)
                selected = searchSuggested.Length-1;


            


            using (var svs = new GUILayout.ScrollViewScope(scroll))
            {
                scroll = svs.scrollPosition;

                for (int r = 0; r < rowCount; ++r)
                {
                    using (new HorizontalCenteredScope())
                    {
                        for (int i = 0; i < maxPerRow; ++i)
                        {
                            if (i + r * maxPerRow >= searchSuggested.Length)
                                break;
                            using (new GUILayout.VerticalScope(GUILayout.Width(105)))
                            {

                                using (new HorizontalCenteredScope())
                                {
                                        GUI.SetNextControlName("Suggestions " + (i + r * maxPerRow).ToString());

                                    if (GUILayout.Button(new GUIContent(searchSuggested[i + r * maxPerRow].previewImage),
                                                         EUIResourceManager.Instance.Skin.GetStyle("Texture"),
                                                         GUILayout.Width(75),
                                                         GUILayout.Height(75)))
                                    {
                                        SpawnSelected(i + r * maxPerRow);
                                    }
                                }

                                string objectName = searchSuggested[i + r * maxPerRow].name;
                                if (objectName.Length > 15)
                                {
                                    string back = objectName.Substring(objectName.Length - 8, 8);
                                    string front = objectName.Substring(0, 5);
                                    objectName = front + "..." + back;
                                }

                                if(selected == i + r* maxPerRow)
                                    GUILayout.Label(new GUIContent(objectName), GUI.skin.GetStyle("AssetLabel"), GUILayout.Width(107));
                                else
                                    GUILayout.Label(new GUIContent(objectName), GUILayout.Width(105));


                            }
                        }
                    }
                }
            }

            if (GUI.GetNameOfFocusedControl() == "Quick Picker Search")
            {
                if ((e.keyCode == KeyCode.LeftArrow || e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.DownArrow || e.keyCode == KeyCode.UpArrow))
                {
                    selected = 0;
                    GUI.FocusControl("Suggestions " + selected);
                    if (e.type != EventType.Layout)
                        e.Use();
                }
                else if (e.keyCode == KeyCode.Return)
                {
                    SpawnSelected(selected);
                    if (e.type != EventType.Layout)
                        e.Use();
                }
                else if (e.keyCode == KeyCode.Tab)
                {
                    GUI.FocusControl("Suggestions " + selected);
                    if (e.type != EventType.Layout)
                        e.Use();
                }
                else if (e.keyCode == KeyCode.Escape)
                {
                    CancelQickpicker();
                    if (e.type != EventType.Layout)
                        e.Use();
                }

            }

        }

        public void SpawnSelected(int idx)
        {
            EditorHotkeysTracker.quickPicker = false;
            if (Selection.activeGameObject)
            {
                if (quickerMode == 0)
                {
                    GameObject[] target;

                    BlockoutStaticFunctions
                        .ReplaceObject(new[] { Selection.activeGameObject },
                                       searchSuggested[idx]
                                           .prefab, out target);
                    Selection.activeGameObject = target[0];
                }
                else
                {
                    var pos = BlockoutStaticFunctions.GetSceneViewSpawnPosition();
                    GameObject go = Instantiate(searchSuggested[idx].prefab, pos, searchSuggested[idx].prefab.transform.rotation);
                    Undo.RegisterCreatedObjectUndo(go, "Spawn Asset From Quick Picker");
                    Selection.activeGameObject = go;
                }
            }
            else
            {
                if (idx <= searchSuggested.Length || idx > 0)
                {
                    var pos = BlockoutStaticFunctions.GetSceneViewSpawnPosition();
                    GameObject go = Instantiate(searchSuggested[idx].prefab, pos,
                                                searchSuggested[idx].prefab.transform.rotation);
                    Undo.RegisterCreatedObjectUndo(go, "Spawn Asset From Quick Picker");
                    Selection.activeGameObject = go;
                }
            }
            showQuickPicker = false;
        }


        private int GetSize(int i)
        {
            int size = 0;

            if (shownItems.Count >= 5)
            {
                size = i == 2 ? 90 : 75;
                if (i == 2)
                {
                    if (Selection.activeGameObject.GetComponent<MeshFilter>())
                        Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = suggestedItems[shownItems[i]].prefab.GetComponent<MeshFilter>().sharedMesh;
                    targetPrefabSwap = suggestedItems[shownItems[i]].prefab;
                }
            }
            else
            {
                size = i == Mathf.RoundToInt(shownItems.Count / 2f) ? 90 : 75;
                if (i == Mathf.RoundToInt(shownItems.Count / 2f))
                {
                    if (Selection.activeGameObject.GetComponent<MeshFilter>())
                        Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = suggestedItems[shownItems[i]].prefab.GetComponent<MeshFilter>().sharedMesh;
                    targetPrefabSwap = suggestedItems[shownItems[i]].prefab;
                }
            }

            return size;
        }

        private void OnScene(SceneView sceneview)
        {
            try
            {
                if (sceneview == null)
                    overSceneView = false;
                else
                    overSceneView = EditorWindow.mouseOverWindow != null && EditorWindow.mouseOverWindow.ToString().Contains("SceneView");
            }
            catch (Exception e)
            {
                overSceneView = false;
            }
            
            
            Handles.BeginGUI();
            if (showScrollPicker && Selection.activeGameObject != null)
            {
                RepaintWhileLoadingTextures();


                Vector2 screenSpaceHandlePosition = HandleUtility.WorldToGUIPoint(Tools.handlePosition);


                float width = (shownItems.Count >= 5 ? 5 : shownItems.Count) * 75 + 30;
                float halfWidth = width / 2;
                float hieght = 150f;
                
                Rect rect = new Rect(screenSpaceHandlePosition.x - halfWidth, screenSpaceHandlePosition.y + 40, width, hieght);

                GUILayout.Window(0, rect, DrawScrollPickerWindow, "Scroll Picker", GUI.skin.window);
                
            }
            else if (showQuickPicker)
            {
                RepaintWhileLoadingTextures();



                Vector2 screenSpaceHandlePosition = Selection.activeGameObject ?  HandleUtility.WorldToGUIPoint(Tools.handlePosition) : sceneview.camera.pixelRect.center;


				#if UNITY_EDITOR_OSX
				screenSpaceHandlePosition /= 2;
				#endif
                
                float width = 5 * 125;
                float halfWidth = width / 2;
                float halfHeight = 75f;
                float hieght = 350f;

                Rect rect = new Rect(screenSpaceHandlePosition.x - halfWidth, Selection.activeGameObject ? screenSpaceHandlePosition.y + 40 : screenSpaceHandlePosition.y - halfHeight, width, hieght);

                GUILayout.Window(0, rect, DrawQuickPickerWindow, "Quick Picker", GUI.skin.window);

            }


            Handles.EndGUI();
        }

        public void InitialiseScrollPicker()
        {
            searchText = "";
            originalMesh = null;
            if (Selection.activeGameObject)
            {
                originalMesh = Selection.activeGameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
            }
            suggestedItems = GetAssetListFromSelected(Selection.activeGameObject ? Selection.activeGameObject.name : searchText.Trim());
            showScrollPicker = true;
        }

        private bool previousShowQuickPicker = false;

        public void InitialiseQuickPicker()
        {
            originalMesh = null;
            if (Selection.activeGameObject)
                originalMesh = Selection.activeGameObject.GetComponentInChildren<MeshFilter>().sharedMesh;

            suggestedItems = GetAssetListFromSelected(Selection.activeGameObject ? Selection.activeGameObject.name : searchText.Trim());
            showQuickPicker = true;
            previousShowQuickPicker = false;
            selected = 0;
        }

        public void ConfirmSwap()
        {
            if (showScrollPicker)
            {
                showScrollPicker = false;
                GameObject[] target;
                BlockoutStaticFunctions.ReplaceObject(new[] {Selection.activeGameObject}, targetPrefabSwap, out target);
                Selection.activeGameObject = target[0];
                BlockoutStaticFunctions.ApplyCurrentTheme();
            }
        }

        public void ConfirmSpawn()
        {
            if (!Selection.activeGameObject && !showScrollPicker)
                return;

            showScrollPicker = false;
            if (originalMesh)
            {
                Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = originalMesh;
                Selection.activeGameObject.GetComponent<Collider>().enabled = true;
            }
            else
                Debug.LogError("BLOCKOUT :: INTERNAL :: No original mesh defined");
            GameObject go = PrefabUtility.InstantiatePrefab(targetPrefabSwap) as GameObject;
            go.transform.position = Selection.activeGameObject.transform.position;
            go.transform.rotation = targetPrefabSwap.transform.rotation;
            Selection.activeGameObject = go;
            GetFavorites();
            BlockoutStaticFunctions.ApplyCurrentTheme();
        }

        public void CancelQickpicker()
        {
            showQuickPicker = false;
            EditorHotkeysTracker.quickPicker = false;
            if (originalMesh && Selection.activeGameObject)
                Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = originalMesh;
            BlockoutStaticFunctions.ApplyCurrentTheme();
        }

        public void CancelScrollPicker()
        {
            showScrollPicker = false;
            if (originalMesh)
                Selection.activeGameObject.GetComponent<MeshFilter>().sharedMesh = originalMesh;
            BlockoutStaticFunctions.ApplyCurrentTheme();
        }


        public override void Update()
        {
            
            if (Selection.activeGameObject != selectedObjectCurrent)
            {
                selectedObjectCurrent = Selection.activeGameObject;
                suggestedItems = GetAssetListFromSelected(searchText.Trim());
            }

            // If no assets are selected then default the suggested assets to a selection of Block prefabs
            if (!showQuickPicker && !showScrollPicker && Selection.activeGameObject == null)
            {
                var foundAssets = AssetDatabase.FindAssets("Block t:prefab");

                bool cap = (foundAssets.Length > amountOfItemsToShow);

                suggestedItems = new BlockoutItemPreview[cap ? amountOfItemsToShow : foundAssets.Length];
                for (int i = 0; i < (cap ? amountOfItemsToShow : foundAssets.Length); ++i)
                {
                    var path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
                    suggestedItems[i].prefab =
                        (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                    suggestedItems[i].name = suggestedItems[i].prefab.name;

                    Texture icon_ = AssetPreview.GetAssetPreview(suggestedItems[i].prefab);// An object loaded with Resources.Load.
                    if (icon_ == null)
                        icon_ = AssetDatabase.GetCachedIcon(path);// "Assets/..."
                    suggestedItems[i].previewImage = (Texture2D)icon_;
                }
                repaint = true;
            }

            

        }
        

        void PreloadAssetData()
        {
            EditorUtility.DisplayProgressBar("Asset Suggestions", "Creating Suggestion Database", 0);
            var foundAssets = AssetDatabase.FindAssets("t:prefab");
            
            database.Database = new BlockoutItemPreview[foundAssets.Length];
            for (int i = 0; i < (foundAssets.Length); ++i)
            {
                var path = AssetDatabase.GUIDToAssetPath(foundAssets[i]);
                database.Database[i].prefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                database.Database[i].name = database.Database[i].prefab ? database.Database[i].prefab.name : "";
                database.Database[i].previewImage = null;

                Texture icon_ = UnityEditor.AssetPreview.GetAssetPreview(database.Database[i].prefab);// An object loaded with Resources.Load.
                if (icon_ == null)
                    icon_ = UnityEditor.AssetDatabase.GetCachedIcon(path);// "Assets/..."

                database.Database[i].previewImage = (Texture2D)icon_;
                EditorUtility.DisplayProgressBar("Asset Suggestions", "Creating Suggestion Database", i / (float)foundAssets.Length);

            }

            EditorUtility.ClearProgressBar();
        }

        BlockoutItemPreview[] GetAssetListFromSelected(string searchName = "")
        {
            searchName = searchName.Trim();

            string regex = "(\\[.*\\])|(\".*\")|('.*')|(\\(.*\\))";
            string input = showQuickPicker ? Regex.Replace(Selection.activeGameObject == null ? searchName : searchName == "" ? Selection.activeGameObject.name : searchName, regex, "") : Regex.Replace(Selection.activeGameObject ? Selection.activeGameObject.name : "Block", regex, "");
            string output = Regex.Replace(input, @" ?\(.*?\)", string.Empty).Trim();

            var names = output.Split('_').ToList();
            string targetName = names[0];
            int amount = ((names.Count - 1) > 2 ? 2 : names.Count - 1);
            for (int i = 1; i < amount; ++i)
            {
                targetName += "_" + names[i];
            }

            var foundAssets = database.Database.ToList().Where(x => x.name.Contains(targetName)).ToArray();
            

            if (foundAssets.Length > 4096)
                AssetPreview.SetPreviewTextureCacheSize(foundAssets.Length);
            

            shownItems.Clear();
            for (int i = 0; i < foundAssets.Length; ++i)
            {
                int idx = i;
                shownItems.Add(idx);
            }

            return foundAssets;

        }


        void GetFavorites()
        {
            if (BlockoutEditorSettings.CurrentSceneSetting)
            {
                var dict = BlockoutEditorSettings.CurrentSceneSetting.assetDictionary;


                if (dict != null)
                {
                    var sortedFavourites = dict.ToList().OrderByDescending(x => x.assetQuantity).ToList();
                    favouriteItems = new BlockoutItemPreview[(sortedFavourites.Count > amountOfItemsToShow
                                                                  ? amountOfItemsToShow
                                                                  : sortedFavourites.Count)];

                    for (var i = 0; i < sortedFavourites.Count; ++i)
                    {
                        if (sortedFavourites[i] == null || i >= amountOfItemsToShow)
                        {
                            break;
                        }


                        var fa = AssetDatabase.FindAssets(sortedFavourites[i].assetName + " t:prefab");
                        if (fa.Length != 0)
                        {
                            var path = AssetDatabase.GUIDToAssetPath(fa[0]);
                            favouriteItems[i].prefab =
                                (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
                            favouriteItems[i].name = favouriteItems[i].prefab.name;
                            favouriteItems[i].previewImage = AssetPreview.GetAssetPreview(favouriteItems[i].prefab);
                        }
                    }

                    favouriteItems = favouriteItems.ToList().Distinct().ToArray();
                    var toRemove = favouriteItems.ToList().Where(x => x.previewImage == null).ToList();
                    favouriteItems = favouriteItems.ToList().Except(toRemove).ToArray();
                    if (favouriteItems.Length > 0)
                        repaint = true;
                }
            }

        }

        // Draw a collection of item previews in a similar style to the project window
        protected void DrawAssetList(BlockoutItemPreview[] collection, float space = 10)
        {
            using (new GUILayout.HorizontalScope())
            {
                //GUILayout.Space(space);
                if (collection != null)
                {
                    for (int i = 0;
                         i < (collection.Length < amountOfItemsToShow ? collection.Length : amountOfItemsToShow);
                         ++i)
                    {

                        GUILayout.BeginVertical(GUILayout.MaxHeight(currentSize + EditorGUIUtility.singleLineHeight));

                        if (collection[i].previewImage == null)
                            collection[i].previewImage = AssetPreview.GetAssetPreview(collection[i].prefab);
                        GUILayout.Box(new GUIContent(collection[i].previewImage), EUIResourceManager.Instance.Skin.GetStyle("Texture"),
                                      GUILayout.Width(currentSize), GUILayout.Height(currentSize));

                        DragDropGUI(collection[i], GUILayoutUtility.GetLastRect());

                        GUILayout.Label(collection[i].name, GUILayout.Width(currentSize));

                        GUILayout.EndVertical();

                    }
                }
            }
        }

        // Drag and drop logic
        protected void DragDropGUI(BlockoutItemPreview targetPreview, Rect previewArea)
        {
            // Chache event data
            Event currentEvent = Event.current;
            EventType currentEventType = currentEvent.type;

            // The DragExited event does not have the same mouse position data as the other events,
            // so it must be checked now:
            if (currentEventType == EventType.DragExited) DragAndDrop.PrepareStartDrag();// Clear generic data when user pressed escape. (Unfortunately, DragExited is also called when the mouse leaves the drag area)



            switch (currentEventType)
            {
                case EventType.MouseDown:
                    if (!previewArea.Contains(currentEvent.mousePosition)) return;
                    // Mouse is within the preview area and has been clicked. Reset drag data
                    DragAndDrop.PrepareStartDrag();// reset data
                    dragging = false;
                    currentEvent.Use();
                    break;
                case EventType.MouseDrag:
                    // If drag was started here:
                    if (!previewArea.Contains(currentEvent.mousePosition))
                        return;
                    // Start the drag event with drag references
                    dragging = true;
                    Object[] objectReferences = new Object[1] { targetPreview.prefab };// Careful, null values cause exceptions in existing editor code.
                    DragAndDrop.objectReferences = objectReferences;// Note: this object won't be 'get'-able until the next GUI event.

                    DragAndDrop.StartDrag(targetPreview.name);
                    currentEvent.Use();
                    break;
                case EventType.DragUpdated:
                    // Drag positioning has been updated so check if its valid.
                    if (IsDragTargetValid())
                    {

                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        // Spawn the asset if it doesnt exist
                        if (!spwanedAsset)
                        {
                            spwanedAsset = Instantiate(targetPreview.prefab);
                        }

                        PlaceDraggedAsset();
                    }
                    else
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                    dragging = true;
                    currentEvent.Use();
                    break;
                case EventType.DragPerform:
                    // When the drag event has finished, place it and end the event if its valid. If it isn't valid,
                    // destroy the object
                    if (IsDragTargetValid())
                    {
                        DragAndDrop.AcceptDrag();
                        PlaceDraggedAsset();
                    }
                    else if (spwanedAsset != null)
                    {
                        DestroyImmediate(spwanedAsset);
                    }
                    dragging = true;
                    currentEvent.Use();
                    break;
                case EventType.DragExited:
                    // If the drag event has ben canceled, destroy the spawned asset if its already spawned
                    if (spwanedAsset != null)
                    {
                        DestroyImmediate(spwanedAsset);
                    }
                    dragging = false;
                    break;
                case EventType.MouseUp:
                    // Clean up, in case MouseDrag never occurred:
                    DragAndDrop.PrepareStartDrag();
                    if (!dragging && previewArea.Contains(currentEvent.mousePosition))
                    {
                        // if the mouse is still within the preview area and no drag event has occured, then its only been clicked
                        // So selected it in the project window
                        Selection.activeGameObject = targetPreview.prefab;
                        EditorGUIUtility.PingObject(targetPreview.prefab);

                        repaint = true;
                    }
                    break;
            }

        }

        // Only valid if the mouse is over the scene view
        private bool IsDragTargetValid()
        {
            return overSceneView;
        }

        private void PlaceDraggedAsset()
        {
            Ray mouseRay = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(Event.current.mousePosition.x,
                                                                                             Screen.height - Event.current.mousePosition.y, 0.5f));
            RaycastHit[] hit;
            hit = (Physics.RaycastAll(mouseRay));
            bool placed = false;
            if (hit.Length > 0)
            {
                for (int i = 0; i < hit.Length; ++i)
                {
                    if (!DragAndDrop.objectReferences.Contains(hit[i].collider.gameObject))
                    {
                        spwanedAsset.transform.position = hit[i].point;
                        placed = true;
                        break;
                    }
                }
            }
            if (!placed && spwanedAsset)
            {
                spwanedAsset.transform.position =
                    SceneView.lastActiveSceneView.camera.ScreenToWorldPoint(new Vector3(
                                                                                        Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y, 50f));
            }

            repaint = true;
        }

        void RepaintWhileLoadingTextures()
        {
            if (AssetPreview.IsLoadingAssetPreviews())
            {
                repaint = true;
            }
        }
    }
}
