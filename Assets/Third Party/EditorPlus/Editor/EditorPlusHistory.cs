
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

#pragma warning disable 618
#pragma warning disable 168

namespace EditorPlus
{
    public class EditorPlusHistory : EditorWindow, IHasCustomMenu
    {
        private static EditorPlusHistory window;

        private static Vector2 StartMarginFavs = new Vector2(0f, 10f);

        private static Vector2 StartMarginHist = new Vector2(0f, 5f);

        private static Vector2 ButtonMargin = new Vector2(5f, 3f);

        private static Vector2 ButtonSize = new Vector2(130f, 20f);

        private static bool showLabels = true;

        private static bool repaintNextUpdate = false;

        private static bool highlightInFavs = false;

        private static GUISkin IconsSkin;
        private static GUISkin IconsSkinFav;
        private static GUISkin IconsSkinLock;

        private static Texture LockOpen;
        private static Texture LockClosed;

        private static Texture Next;
        private static Texture Prev;

        private static Texture2D GameObjectLabel;

        private static bool init = false;

        public static int historySize = 25;

        private static Object selectedObj;

        private static WidgetHost widgetHostFavs;
        
        private static Rect favsView;

        private static WidgetHost widgetHostHistory;

        private static Vector2 historyViewPos = Vector2.zero;

        private static Rect historyView;


        void OnEnable()
        {
            Load();
        }


        [MenuItem("Window/EditorPlus/History")]
        static public void Init()
        {
            init = true;
            window = (EditorPlusHistory)EditorWindow.GetWindow(typeof(EditorPlusHistory));
            window.title = "History & Favs";
            window.minSize = new Vector2(window.minSize.x, 57);


            Load();

            float sc = EditorPrefs.GetFloat("EditorPlusHistory.Scale", 1f);
            showLabels = EditorPrefs.GetBool("EditorPlusHistory.ShowLabels", true);
            historySize = EditorPrefs.GetInt("EditorPlusHistory.HistorySize", 25);

            //EditorPlusHistoryInternal.ForceDataUpdate();

            widgetHostFavs = new WidgetHost(window.position, StartMarginFavs, ButtonMargin + new Vector2(LockClosed.width, 0), ButtonSize, EditorPlusHistoryInternal.favorites.Count, sc, true);
            widgetHostHistory = new WidgetHost(window.position, StartMarginHist, ButtonMargin + new Vector2(LockClosed.width, 0), ButtonSize, EditorPlusHistoryInternal.favorites.Count, sc, true);

            EditorPlus.OnSkinSwitched += Load;
        }



        static public void Load()
        {
            IconsSkin = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "UnityPlusSkinHistory.guiskin") as GUISkin;
            IconsSkinFav = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "UnityPlusSkinHistoryFav.guiskin") as GUISkin;
            IconsSkinLock = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "UnityPlusSkinHistoryLock.guiskin") as GUISkin;


            LockOpen = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "EditorPlusLockOpen.png") as Texture;
            LockClosed = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath + EditorPlus.SelectedSkinFolder + "EditorPlusLockClosed.png") as Texture;
            Next = AssetDatabase.LoadMainAssetAtPath("Assets/EditorPlus/Content/EditorPlus_Arrow_Next.png") as Texture;
            Prev = AssetDatabase.LoadMainAssetAtPath("Assets/EditorPlus/Content/EditorPlus_Arrow_Prev.png") as Texture;

            GameObjectLabel = AssetDatabase.LoadMainAssetAtPath(EditorPlus.PlusPath +  "EditorPlus_GameobjectIcon.png") as Texture2D;

            if (window != null)
                window.Repaint();

        }

        void IHasCustomMenu.AddItemsToMenu(GenericMenu menu)
        {
        }

        void ShowButton(Rect position)
        {
            Event e = Event.current;
            if (GUI.Button(position, EditorPlus.EditorPlusIcon, new GUIStyle(GUI.skin.label)))
            {
                if (e.button != 0)
                    return;
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Scale/1.0"), widgetHostFavs.WidgetScale == 1f, ChangeScale, 1f);
                menu.AddItem(new GUIContent("Scale/1.25"), widgetHostFavs.WidgetScale == 1.25f, ChangeScale, 1.25f);
                menu.AddItem(new GUIContent("Scale/1.5"), widgetHostFavs.WidgetScale == 1.5f, ChangeScale, 1.5f);
                menu.AddItem(new GUIContent("Scale/1.75"), widgetHostFavs.WidgetScale == 1.75f, ChangeScale, 1.75f);
                menu.AddItem(new GUIContent("Scale/2"), widgetHostFavs.WidgetScale == 2f, ChangeScale, 2f);

                menu.AddItem(new GUIContent("Icons/Show"), showLabels, ChangeLabelMode, true);
                menu.AddItem(new GUIContent("Icons/Hide"), !showLabels, ChangeLabelMode, false);

                menu.AddItem(new GUIContent("HistorySize/10"), historySize == 10, SetHistoryMax, 10);
                menu.AddItem(new GUIContent("HistorySize/25"), historySize == 25, SetHistoryMax, 25);
                menu.AddItem(new GUIContent("HistorySize/50"), historySize == 50, SetHistoryMax, 50);
                menu.AddItem(new GUIContent("HistorySize/75"), historySize == 75, SetHistoryMax, 75);
                menu.AddItem(new GUIContent("HistorySize/100"), historySize == 100, SetHistoryMax, 100);
                menu.AddItem(new GUIContent("HistorySize/Unlimited"), historySize == 1500, SetHistoryMax, 1500);    //not really unlimited, prevent lags :)

                menu.AddItem(new GUIContent("Clear/Clear History"), false, EditorPlusHistoryInternal.ClearHistory);
                menu.AddItem(new GUIContent("Clear/Clear All"), false, EditorPlusHistoryInternal.ClearData);

                menu.ShowAsContext();
            }
        }


        void OnSelectionChange()
        {
            if (Selection.activeObject != null)
            {
                if(focusedWindow != this)
                {
                    selectedObj = null;
                }

                repaintNextUpdate = true;
                Repaint();
            }
        }


        void OnInspectorUpdate()
        {
            if(repaintNextUpdate)
            {
                repaintNextUpdate = false;
                Repaint();
            }
        }


        void OnHierarchyChange()
        {
            Repaint();
        }


        public void ChangeScale(object scale)
        {
            widgetHostFavs.WidgetScale = (float)scale;
            widgetHostHistory.WidgetScale = (float)scale;
            EditorPrefs.SetFloat("EditorPlusHistory.Scale", (float)scale);
        }


        public void ChangeLabelMode(object labels)
        {
            EditorPrefs.SetBool("EditorPlusHistory.ShowLabels", (bool)labels);
            showLabels = (bool)labels;
        }


        public void SetHistoryMax(object count)
        {
            historySize = (int)count;
            EditorPrefs.SetInt("EditorPlusHistory.HistorySize", historySize);
        }


        void OnGUI()
        {
            if (!init)
                Init();

            bool hoverSelected = false;
            Event e = Event.current;

            EditorPlusHistoryInternal.CheckConsistency();

            widgetHostFavs.Area = position;
            widgetHostFavs.WidgetsCount = EditorPlusHistoryInternal.favorites.Count + 1;
            widgetHostFavs.Update();

            favsView = new Rect(0, 0, position.width, widgetHostFavs.GetYMax());

            historyView = new Rect(0, widgetHostFavs.GetYMax(), position.width, position.height - favsView.height);

            widgetHostHistory.Area = position;
            widgetHostHistory.WidgetsCount = EditorPlusHistoryInternal.history.Count;

            widgetHostHistory.Update();

            //drag and drop sorting
            if (favsView.Contains(e.mousePosition) && DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] == selectedObj && (EditorPlusHistoryInternal.favorites.Contains(selectedObj) || EditorPlusHistoryInternal.history.Contains(selectedObj)))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                if (e.rawType == EventType.DragPerform)
                {
                    int index = widgetHostFavs.FindClosestWidget(e.mousePosition);

                    bool abort = false;

                    if (index - 1 == EditorPlusHistoryInternal.favorites.IndexOf(selectedObj))
                    {
                        DragAndDrop.PrepareStartDrag();
                        abort = true;
                    }
                    else if(index - 1 <= 0)
                    {
                        if (!EditorPlusHistoryInternal.favorites.Contains(selectedObj))
                        {
                            EditorPlusHistoryInternal.AddToFavorites(selectedObj);
                            widgetHostFavs.WidgetsCount = EditorPlusHistoryInternal.favorites.Count + 1;
                            widgetHostFavs.Update();
                        }
                        EditorPlusHistoryInternal.SetFavoritePosition(0, selectedObj);
                        abort = true;
                    }

                    //check if widget + margin.x contains mouse position and insert there
                    if (!abort && e.mousePosition.x > widgetHostFavs.Widgets[index].x && e.mousePosition.x < widgetHostFavs.Widgets[index].x + widgetHostFavs.Widgets[index].width + widgetHostFavs.WidgetMargin.x && e.mousePosition.y > widgetHostFavs.Widgets[index].y && e.mousePosition.y < widgetHostFavs.Widgets[index].y + widgetHostFavs.Widgets[index].height)
                    {
                        //left or right
                        if (e.mousePosition.x < widgetHostFavs.Widgets[index].x + widgetHostFavs.Widgets[index].width / 2)
                        {
                            if (!EditorPlusHistoryInternal.favorites.Contains(selectedObj))
                            {
                                EditorPlusHistoryInternal.AddToFavorites(selectedObj);
                                widgetHostFavs.WidgetsCount = EditorPlusHistoryInternal.favorites.Count + 1;
                                widgetHostFavs.Update();
                            }
                            EditorPlusHistoryInternal.SetFavoritePosition(index - 1, selectedObj);
                        }
                        else
                        {
                            if (!EditorPlusHistoryInternal.favorites.Contains(selectedObj))
                            {
                                EditorPlusHistoryInternal.AddToFavorites(selectedObj);
                                widgetHostFavs.WidgetsCount = EditorPlusHistoryInternal.favorites.Count + 1;
                                widgetHostFavs.Update();
                            }
                            EditorPlusHistoryInternal.SetFavoritePosition(index, selectedObj);
                        }
                    }
                    else if (!abort)
                    {
                        if (!EditorPlusHistoryInternal.favorites.Contains(selectedObj))
                        {
                            EditorPlusHistoryInternal.AddToFavorites(selectedObj);
                            widgetHostFavs.WidgetsCount = EditorPlusHistoryInternal.favorites.Count + 1;
                            widgetHostFavs.Update();
                        }
                        EditorPlusHistoryInternal.SetFavoritePosition(EditorPlusHistoryInternal.favorites.Count, selectedObj);
                    }
                }
            }


            float scrollViewFavsHeight = 0f;
            if (widgetHostFavs.Widgets.Count > 0)
            {
                scrollViewFavsHeight = widgetHostFavs.Widgets[widgetHostFavs.Widgets.Count - 1].yMax;
            }


            float scrollViewHistHeight = 0f;
            if (widgetHostHistory.Widgets.Count > 0)
            {
                scrollViewHistHeight = widgetHostHistory.Widgets[widgetHostHistory.Widgets.Count - 1].yMax;
            }

            if (position.height < scrollViewFavsHeight + scrollViewHistHeight)
            {
                widgetHostFavs.Area = new Rect(0, 0, favsView.width - GUI.skin.verticalScrollbar.normal.background.width, favsView.height);
                widgetHostFavs.Update();

                widgetHostHistory.Area = new Rect(0, historyView.y, historyView.width - GUI.skin.verticalScrollbar.normal.background.width, historyView.height);
                widgetHostHistory.Update();
            }

            GUISkin old = GUI.skin;
            GUI.skin = EditorPlus.ScrollviewSkin;
            historyViewPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height), historyViewPos, new Rect(0, 0, position.width - 30, scrollViewFavsHeight + scrollViewHistHeight));
            GUI.skin = old;

            if (GUI.skin != IconsSkinFav)
                    GUI.skin = IconsSkinFav;

            //Navigation Arrows
            Rect r = widgetHostFavs.Widgets[0];

            if (GUI.Button(new Rect(r.x, r.y, r.width / 2 - ButtonMargin.x - LockClosed.width, r.height), new GUIContent("Prev", Prev)) || (e.isMouse && e.button == 3))
            {
                EditorPlusHistoryInternal.HistoryNavigationBack();
            }

            if (GUI.Button(new Rect(r.x + r.width / 2, r.y, r.width / 2, r.height), new GUIContent("Next", Next)))
            {
                EditorPlusHistoryInternal.HistoryNavigationForward();
            }


            //-----------------------------------------------
            //draw favorites
            for (int i = 0; i < EditorPlusHistoryInternal.favorites.Count; ++i)
            {
                if (GUI.skin != IconsSkinFav)
                    GUI.skin = IconsSkinFav;

                
                r = widgetHostFavs.Widgets[i + 1];

                if (e.rawType == EventType.MouseDown)
                {
                    if (r.Contains(e.mousePosition))
                    {
                        //Drag object
                        if (e.button == 0)
                        {
                            highlightInFavs = true;
                            if (e.clickCount != 2)
                            {
                                selectedObj = EditorPlusHistoryInternal.favorites[i];

                                DragAndDrop.PrepareStartDrag();
                                if (selectedObj.GetType() != typeof(UnityEngine.GameObject))
                                    DragAndDrop.paths = new string[] { AssetDatabase.GetAssetPath(selectedObj) };
                                DragAndDrop.objectReferences = new Object[] { selectedObj };

                                DragAndDrop.StartDrag(selectedObj.ToString());
                            }
                            else if (e.clickCount == 2)
                            {
                                Selection.activeObject = EditorPlusHistoryInternal.favorites[i];

                                if (Selection.activeObject.GetType() != typeof(UnityEngine.GameObject))
                                {
                                    if (Selection.activeObject.GetType() == EditorPlus.EPlusDefaultType)
                                    {
                                        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                                        if (Directory.Exists(path))
                                        {
                                            string[] subFolders = Directory.GetDirectories(path);
                                            if (subFolders.Length != 0)
                                            {
                                                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(subFolders[0]));
                                                break;
                                            }

                                            string[] content = Directory.GetFiles(path);
                                            if (content.Length == 0)
                                            {
                                                EditorGUIUtility.PingObject(Selection.activeObject);
                                            }
                                            else
                                            {
                                                foreach (string file in content)
                                                {
                                                    Object asset = AssetDatabase.LoadMainAssetAtPath(file);
                                                    if (asset != null)
                                                    {
                                                        EditorGUIUtility.PingObject(asset);
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        EditorGUIUtility.PingObject(Selection.activeObject);
                                    }
                                }
                            }
                        }
                        hoverSelected = true;
                    }
                }
                else if (e.rawType == EventType.MouseUp && r.Contains(e.mousePosition))
                    hoverSelected = true;


                GUIContent cnt = new GUIContent(EditorPlusHistoryInternal.favorites[i].name);

                if (cnt.text == "")
                {
                    cnt.text = "Noname";
                }

                if (showLabels)
                {
                    Texture2D prev = AssetPreview.GetAssetPreview(EditorPlusHistoryInternal.favorites[i]);
                    if (prev == null)
                    {
                        prev = (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(EditorPlusHistoryInternal.favorites[i]));
                        if (EditorPlusHistoryInternal.favorites[i].GetType() == typeof(UnityEngine.GameObject))
                        {
                            prev = GameObjectLabel;
                        }
                    }
                    GUI.Label(new Rect(r.x, r.y, 32 * widgetHostFavs.WidgetScale, r.height), prev, ((selectedObj == EditorPlusHistoryInternal.favorites[i] && highlightInFavs)) ? new GUIStyle(GUI.skin.box) : new GUIStyle(GUI.skin.button));
                    r.x += 20 * widgetHostFavs.WidgetScale;
                    r.width -= 20 * widgetHostFavs.WidgetScale;
                }
                GUI.Label(r, cnt, ((selectedObj == EditorPlusHistoryInternal.favorites[i] && highlightInFavs)) ? new GUIStyle(GUI.skin.box) : new GUIStyle(GUI.skin.button));

                Rect lockButton = new Rect(r.x + r.width, r.y + 5, LockClosed.width, LockClosed.height);

                if (GUI.skin != IconsSkinLock)
                    GUI.skin = IconsSkinLock;

                if (GUI.Button(lockButton, LockClosed))
                {
                    EditorPlusHistoryInternal.RemoveFromFavorites(EditorPlusHistoryInternal.favorites[i]);
                    --i;
                    hoverSelected = true;
                    repaintNextUpdate = true;
                    continue;
                }
            }

            if (GUI.skin != IconsSkin)
                GUI.skin = IconsSkin;

            
            //historyViewPos = GUI.BeginScrollView(historyView, historyViewPos, new Rect(0, 0, position.width - 30, scrollViewHistHeight));
            historyView.height = scrollViewHistHeight;
            GUI.BeginGroup(historyView);

            //-----------------------------------------------
            //draw history
            for (int i = 0; i < EditorPlusHistoryInternal.history.Count; ++i)
            {
                if (i == historySize)
                    break;

                if (GUI.skin != IconsSkin)
                    GUI.skin = IconsSkin;

                if (EditorPlusHistoryInternal.history[i] == null)
                {
                    EditorPlusHistoryInternal.RemoveFromHistory(EditorPlusHistoryInternal.history[i], true);
                    --i;
                    continue;
                }

                r = widgetHostHistory.Widgets[i];

                if (e.rawType == EventType.MouseDown)
                {
                    if (r.Contains(e.mousePosition))
                    {
                        //Drag object
                        if (e.button == 0)
                        {
                            highlightInFavs = false;
                            if (e.clickCount != 2)
                            {
                                selectedObj = EditorPlusHistoryInternal.history[i];

                                DragAndDrop.PrepareStartDrag();
                                string path = AssetDatabase.GetAssetPath(selectedObj);
                                if (path != "")
                                    DragAndDrop.paths = new string[] { path };
                                if (selectedObj != null)
                                    DragAndDrop.objectReferences = new Object[] { selectedObj };

                                DragAndDrop.StartDrag(selectedObj.ToString());

                            }
                            else if (e.clickCount == 2)
                            {
                                Selection.activeObject = EditorPlusHistoryInternal.history[i];

                                if (Selection.activeObject.GetType() != typeof(UnityEngine.GameObject))
                                {
                                    if (Selection.activeObject.GetType() == EditorPlus.EPlusDefaultType)
                                    {
                                        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                                        if (Directory.Exists(path))
                                        {

                                            string[] subFolders = Directory.GetDirectories(path);
                                            if (subFolders.Length != 0)
                                            {
                                                AssetDatabase.OpenAsset(AssetDatabase.LoadMainAssetAtPath(subFolders[0]));
                                                break;
                                            }

                                            string[] content = Directory.GetFiles(path);
                                            if (content.Length == 0)
                                            {
                                                EditorGUIUtility.PingObject(Selection.activeObject);
                                            }
                                            else
                                            {
                                                foreach (string file in content)
                                                {
                                                    Object asset = AssetDatabase.LoadMainAssetAtPath(file);
                                                    if (asset != null)
                                                    {
                                                        EditorGUIUtility.PingObject(asset);
                                                        break;
                                                    }
                                                }

                                            }
                                        }
                                    }
                                    else
                                    {
                                        EditorGUIUtility.PingObject(Selection.activeObject);
                                    }
                                }
                            }
                        }
                        hoverSelected = true;
                    }
                }
                else if(e.rawType == EventType.MouseUp && r.Contains(e.mousePosition))
                    hoverSelected = true;


                GUIContent cnt = new GUIContent(EditorPlusHistoryInternal.history[i].name);

                if(cnt.text == "")
                {
                    cnt.text = "Noname";
                }

                //Debug.Log(GUI.skin.box.CalcSize(new GUIContent(cnt.text)).x + " " + GUI.skin.box.padding.left + " " + GUI.skin.box.padding.right);

                if (showLabels)
                {
                    Texture2D prev = AssetPreview.GetAssetPreview(EditorPlusHistoryInternal.history[i]);
                    if (prev == null)
                    {
                        prev = (Texture2D)AssetDatabase.GetCachedIcon(AssetDatabase.GetAssetPath(EditorPlusHistoryInternal.history[i]));
                        if (EditorPlusHistoryInternal.history[i].GetType() == typeof(UnityEngine.GameObject))
                        {
                            prev = GameObjectLabel;
                        }
                    }
                    GUI.Label(new Rect(r.x, r.y, 32 * widgetHostHistory.WidgetScale, r.height), prev, ((selectedObj == EditorPlusHistoryInternal.history[i] && !highlightInFavs)) ? new GUIStyle(GUI.skin.box) : new GUIStyle(GUI.skin.button));
                    r.x += 20 * widgetHostHistory.WidgetScale;
                    r.width -= 20 * widgetHostHistory.WidgetScale;
                }

                GUI.Label(r, cnt, ((selectedObj == EditorPlusHistoryInternal.history[i] && !highlightInFavs)) ?  new GUIStyle(GUI.skin.box) : new GUIStyle(GUI.skin.button));

                Rect lockButton = new Rect(r.x + r.width, r.y + 5, LockOpen.width, LockOpen.height);

                if (GUI.skin != IconsSkinLock)
                    GUI.skin = IconsSkinLock;

                if (!EditorPlusHistoryInternal.favorites.Contains(EditorPlusHistoryInternal.history[i]) && GUI.Button(lockButton, LockOpen))
                {
                    EditorPlusHistoryInternal.AddToFavorites(EditorPlusHistoryInternal.history[i]);
                    --i;
                    hoverSelected = true;
                    repaintNextUpdate = true;
                    continue;
                }
            }
            GUI.EndGroup();
            GUI.EndScrollView();



            //Reset Inspector if mouse up inside the window but not on button

            bool inside = false;
            if (e.mousePosition.x > 0f && e.mousePosition.x < window.position.width && e.mousePosition.y > 0f && e.mousePosition.y < window.position.height)
            {
                inside = true;
            }
            if (inside && !hoverSelected && e.rawType == EventType.MouseUp)
            {
                selectedObj = null;
                Selection.activeObject = null;
            }

            if ((e.rawType == EventType.MouseUp || e.rawType == EventType.dragPerform) && DragAndDrop.objectReferences != null)
            {
                DragAndDrop.PrepareStartDrag();
            }
        }
    }
}