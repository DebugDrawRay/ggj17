#if (UNITY_4_2 || UNITY_4_3 || UNITY_4_5)
#define EPlus_4
#else
#define EPlus_5
#endif

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
#pragma warning disable 618

namespace EditorPlus
{
    [InitializeOnLoad]
    public static class EditorPlusHistoryInternal
    {
        private static Object oldSelection = null;

        private static int historySelectionIndex = 0;

        private static string level = "";

        private static EditorPlusHistorySceneObjects sceneObjectsSave;

        private static EditorPlusHistoryProjectObjects projectObjectsSave;

        private static List<int> historyIDs = new List<int>();

        public static List<Object> history = new List<Object>();

        public static List<int> historyNavigationIDs = new List<int>();

        private static List<int> favoritesIDs = new List<int>();

        public static List<Object> favorites = new List<Object>();

        private static int currentID;

        private static bool sceneObjWaitingForSerializer = false;

        public static bool historyUseSceneSaving = true;

        public static List<string> savedScenes = new List<string>();


        static EditorPlusHistoryInternal()
        {
            currentID = EditorPrefs.GetInt("EditorPlusHistory.curID", 0);
            historyUseSceneSaving = EditorPrefs.GetBool("EditorPlusUseSceneSaving", true);

            level = Application.loadedLevelName;

            projectObjectsSave = new EditorPlusHistoryProjectObjects();
            HandleSceneSaveObject();

            LoadData();
            ForceDataUpdate();

            EditorApplication.update += Update;
            EditorApplication.playmodeStateChanged += PlayModeChanged;
        }

        public static void PlayModeChanged()
        {
            ForceDataUpdate();
        }
        

        public static void ClearData()
        {
            EditorPrefs.SetString("EditorPlusHistory.History", "");
            EditorPrefs.SetString("EditorPlusHistory.HistoryNavigation", "");
            EditorPrefs.SetString("EditorPlusHistory.Favorites", "");
            EditorPrefs.SetInt("EditorPlusHistory.curID", 0);
            EditorPrefs.SetInt("EditorPlusHistory.clearID", Random.Range(0, int.MaxValue));

            

            currentID = 0;

            historyIDs.Clear();
            history.Clear();
            historyNavigationIDs.Clear();
            historySelectionIndex = 0;
            favoritesIDs.Clear();
            favorites.Clear();

            if (projectObjectsSave != null)
            {
                projectObjectsSave.ClearData();
                projectObjectsSave.SaveData();
            }

            if (sceneObjectsSave != null)
            {
                ScriptableObject.DestroyImmediate(sceneObjectsSave.gameObject);
            }

            HandleSceneSaveObject();
        }


        public static void ClearHistory()
        {
            historyIDs.Clear();
            history.Clear();
            historyNavigationIDs.Clear();
        }


        private static void LoadData()
        {
            string loaded = EditorPrefs.GetString("EditorPlusHistory.History", "");
            if (loaded != "")
            {
                string[] loadedSplit = loaded.Split(';');
                foreach (string ld in loadedSplit)
                {
                    if (ld.Length == 0)
                        continue;

                    historyIDs.Add(int.Parse(ld));
                }
            }

            loaded = EditorPrefs.GetString("EditorPlusHistory.HistoryNavigation", "");
            historySelectionIndex = EditorPrefs.GetInt("EditorPlusHistory.HistoryNavigationIndex", 0);
            if (loaded != "")
            {
                string[] loadedSplit = loaded.Split(';');
                foreach (string ld in loadedSplit)
                {
                    if (ld.Length == 0)
                        continue;

                    historyNavigationIDs.Add(int.Parse(ld));
                }
            }

            loaded = EditorPrefs.GetString("EditorPlusHistory.Favorites", "");
            if (loaded != "")
            {
                string[] loadedSplit = loaded.Split(';');
                foreach (string ld in loadedSplit)
                {
                    if (ld.Length == 0)
                        continue;

                    favoritesIDs.Add(int.Parse(ld));
                }
            }
        }


        private static void SaveAll()
        {
            SaveID();
            SaveHistory();
            SaveHistoryNavigation();
            SaveFavorites();
            projectObjectsSave.SaveData();
        }


        private static void SaveID()
        {
            EditorPrefs.SetInt("EditorPlusHistory.curID", currentID);
        }


        private static void SaveHistory()
        {
            string save = "";

            foreach (int entry in historyIDs)
            {
                save += entry.ToString() + ";";
            }
            EditorPrefs.SetString("EditorPlusHistory.History", save);
        }


        private static void SaveHistoryNavigation()
        {
            string save = "";

            foreach (int entry in historyNavigationIDs)
            {
                save += entry.ToString() + ";";
            }
            EditorPrefs.SetString("EditorPlusHistory.HistoryNavigation", save);
            EditorPrefs.SetInt("EditorPlusHistory.HistoryNavigationIndex", historySelectionIndex);
        }


        private static void SaveFavorites()
        {
            string save = "";

            foreach (int entry in favoritesIDs)
            {
                save += entry.ToString() + ";";
            }
            EditorPrefs.SetString("EditorPlusHistory.Favorites", save);
        }


        private static int GetNewID()
        {
            ++currentID;
            if (currentID == int.MaxValue)
                currentID = 0;      //undefined behavior; congrats if you reach int.Max :)
            SaveID();
            return currentID;
        }


        public static int FindID(Object obj)
        {
            int prID = projectObjectsSave.GetID(obj);
            if (prID != -1)
            {
                return prID;
            }
            int sceneID = sceneObjectsSave.GetID(obj);
            if (sceneID != -1)
            {
                return sceneID;
            }

            return -1;
        }


        private static int FindIDOrGetNew(Object obj)
        {
            int prID = projectObjectsSave.GetID(obj);
            if (prID != -1)
            {
                return prID;
            }
            int sceneID = sceneObjectsSave.GetID(obj);
            if (sceneID != -1)
            {
                return sceneID;
            }

            int id = GetNewID();

            if (obj.GetType() == typeof(UnityEngine.GameObject) && AssetDatabase.GetAssetPath(obj) == "")
            {
                sceneObjectsSave.Set(id, obj);
                EditorUtility.SetDirty(sceneObjectsSave.gameObject);
            }
            else
            {
                projectObjectsSave.Set(id, obj);
            }

            return id;
        }


        public static Object FindObject(int id)
        {
            Object cur = projectObjectsSave.GetOBJ(id);
            if (cur != null)
                return cur;

            return sceneObjectsSave.GetOBJ(id);
        }


        public static void AddToFavorites(Object obj)
        {
            int id = FindIDOrGetNew(obj);

            if (favoritesIDs.Contains(id))
                return;

            favoritesIDs.Add(id);
            SetFavorites();
            SaveAll();
        }


        public static void RemoveFromFavorites(Object obj)
        {
            int id = FindIDOrGetNew(obj);

            if (!favoritesIDs.Contains(id))
                return;

            favoritesIDs.Remove(id);
            SetFavorites();
            SaveAll();
        }


        public static void SetFavoritePosition(int index, Object obj)
        {
            int id = FindID(obj);
            int indexCur = favoritesIDs.IndexOf(id);
            favoritesIDs.RemoveAt(indexCur);

            //we have to take care of other scenes (hidden for favorites but not favoritesIDs) objects
            if (index == 0)
            {
                favoritesIDs.Insert(0, id);
            }
            else if (index >= favoritesIDs.Count)
            {
                favoritesIDs.Add(id);
            }
            else
            {
                int idLeft = FindID(favorites[index - 1]);
                if (idLeft == id)    //case at the end of favorites
                {
                    favoritesIDs.Insert(indexCur, id);
                }
                else
                {
                    favoritesIDs.Insert(favoritesIDs.IndexOf(idLeft) + 1, id);
                }
            }
            SaveFavorites();
            SetFavorites();
        }


        public static void HistoryNavigationForward()
        {
            if (historySelectionIndex <= 0)
                return;
            historySelectionIndex--;
            Selection.activeObject = FindObject(historyNavigationIDs[historySelectionIndex]);
            SaveHistoryNavigation();
        }


        public static void HistoryNavigationBack()
        {
            if (historySelectionIndex >= historyNavigationIDs.Count - 1)
                return;
            historySelectionIndex++;
            Selection.activeObject = FindObject(historyNavigationIDs[historySelectionIndex]);
            SaveHistoryNavigation();
        }


        public static void AddToHistory(int id)
        {
            if (historyIDs.Contains(id))
                historyIDs.Remove(id);

            historyIDs.Insert(0, id);
            if (historySelectionIndex != 0 && historyNavigationIDs.Count != 0)
            {
                historyNavigationIDs.RemoveRange(0, historySelectionIndex);
                historySelectionIndex = 0;
                
            }
            historyNavigationIDs.Insert(0, id);
            
            while(historyIDs.Count > EditorPlusHistory.historySize)
            {
                historyIDs.RemoveAt(historyIDs.Count - 1);
            }

            while (historyNavigationIDs.Count > EditorPlusHistory.historySize)
            {
                historyNavigationIDs.RemoveAt(historyNavigationIDs.Count - 1);
            }


            SetHistory();
            SaveHistory();
            SaveHistoryNavigation();
        }


        public static void RemoveFromHistory(Object obj, bool removeFromSelection)
        {
            int index = FindID(obj);
            if (index != -1)
                RemoveFromHistory(index, removeFromSelection);
        }


        public static void RemoveFromHistory(int id, bool removeFromSelection)
        {
            if (id == -1)
                return;

            if (removeFromSelection)
            {
                int index = historyNavigationIDs.IndexOf(id);
                if (index < historySelectionIndex)
                {
                    historySelectionIndex--;
                }
            }

            historyNavigationIDs.Remove(id);
            historyIDs.Remove(id);
            SetHistory();
            SaveHistory();
        }


        public static void CheckConsistency()
        {
            sceneObjectsSave.CheckConsistency();

            bool dirty = false;
            for (int i = 0; i < history.Count; ++i)
            {
                if (history[i] == null)
                {
                    history.RemoveAt(i);
                    --i;
                    dirty = true;
                }
            }
            if (dirty)
            {
                SetHistory();
            }

            dirty = false;
            for (int i = 0; i < favorites.Count; ++i)
            {
                if (favorites[i] == null)
                {
                    favorites.RemoveAt(i);
                    --i;
                    dirty = true;
                }
            }
            if (dirty)
            {
                SetFavorites();
            }
        }


        public static void ForceDataUpdate()
        {
            SetHistory();
            SetFavorites();
        }


        public static void Update()
        {
            
            if (level != Application.loadedLevelName || sceneObjectsSave == null || sceneObjWaitingForSerializer)
            {
                HandleSceneSaveObject();
            }

            //selection
            if (oldSelection != Selection.activeObject)
            {
                if (Selection.activeObject != null)
                {
                    if (historyNavigationIDs.Count == 0 || (historyNavigationIDs.Count != 0 && historySelectionIndex < historyNavigationIDs.Count && Selection.activeObject != FindObject(historyNavigationIDs[historySelectionIndex])))
                    {
                        int id = FindIDOrGetNew(Selection.activeObject);

                        AddToHistory(id);
                    }
                }
                oldSelection = Selection.activeObject;
            }
        }


        private static void SetHistory()
        {
            history.Clear();

            if (sceneObjectsSave == null)
            {
                HandleSceneSaveObject();
            }
            
            foreach (int id in historyIDs)
            {
                Object cur = FindObject(id);

                if (cur == null)
                    continue;
                history.Add(cur);
            }
        }


        private static void SetFavorites()
        {
            favorites.Clear();

            foreach (int id in favoritesIDs)
            {
                Object cur = FindObject(id);
                if (cur == null)
                    continue;
                favorites.Add(cur);
            }
        }


        private static void HandleSceneSaveObject()
        {
            GameObject save = GameObject.Find("EditorPlusSceneObjectsSave." + SystemInfo.deviceUniqueIdentifier);
            
            if (save == null)
            {
                save = new GameObject("EditorPlusSceneObjectsSave." + SystemInfo.deviceUniqueIdentifier);
                sceneObjectsSave = save.AddComponent<EditorPlusHistorySceneObjects>();
                sceneObjectsSave.ClearID = EditorPrefs.GetInt("EditorPlusHistory.clearID", 0);
                EditorUtility.SetDirty(sceneObjectsSave);
                if (historyUseSceneSaving)
                    save.hideFlags = HideFlags.HideInHierarchy;
                else
                    save.hideFlags = HideFlags.HideAndDontSave;
                
                sceneObjWaitingForSerializer = false;
            }
            else
            {
                if (historyUseSceneSaving)
                    save.hideFlags = HideFlags.HideInHierarchy;
                else
                    save.hideFlags = HideFlags.HideAndDontSave;

                sceneObjectsSave = save.GetComponent<EditorPlusHistorySceneObjects>();
                if (sceneObjectsSave.ClearID != -1 && sceneObjectsSave.ClearID != EditorPrefs.GetInt("EditorPlusHistory.clearID", 0))
                {
                    sceneObjectsSave.Clear();
                    sceneObjectsSave.ClearID = EditorPrefs.GetInt("EditorPlusHistory.clearID", 0);
                    EditorUtility.SetDirty(sceneObjectsSave);
                }
                if(sceneObjectsSave.ClearID == -1)
                {
                    sceneObjWaitingForSerializer = true;
                }
                else
                {
                    sceneObjWaitingForSerializer = false;
                }
            }
            
        }
    }

    public class EditorPlusHistoryProjectObjects
    {
        public struct HPOEntry
        {
            public int id;
            public Object obj;

            public HPOEntry(int objId, Object ob)
            {
                id = objId;
                obj = ob;
            }


            #region operators
            public override bool Equals(System.Object ob)
            {
                return ob is HPOEntry && this == (HPOEntry)ob;
            }
            public override int GetHashCode()
            {
                return id.GetHashCode() ^ obj.GetHashCode();
            }
            public static bool operator ==(HPOEntry x, HPOEntry y)
            {
                return x.id == y.id && x.obj == y.obj;
            }
            public static bool operator !=(HPOEntry x, HPOEntry y)
            {
                return !(x == y);
            }

            #endregion
        }

        private List<HPOEntry> Data;


        public EditorPlusHistoryProjectObjects()
        {
            if (Data == null)
            {
                Data = new List<HPOEntry>();
            }
            else
            {
                Data.Clear();
            }
            LoadData();
        }


        public void Set(int objId, Object go)
        {
            HPOEntry entr = new HPOEntry(objId, go);
            if (!Data.Contains(entr))
            {
                Data.Add(entr);
                SaveData();

            }
        }


        public int GetID(Object go)
        {
            foreach (HPOEntry entr in Data)
            {
                if (entr.obj == go)
                    return entr.id;
            }
            return -1;
        }


        public Object GetOBJ(int id)
        {
            foreach (HPOEntry entr in Data)
            {
                if (entr.id == id)
                    return entr.obj;
            }
            return null;

        }

        public void ClearData()
        {
            Data.Clear();
        }


        public void LoadData()
        {
            string loaded = EditorPrefs.GetString("EditorPlusHistory.ProjectObjs", "");
            if (loaded == "")
                return;
            string[] loadedSplit = loaded.Split(';');
            foreach (string ld in loadedSplit)
            {
                if (ld.Length == 0)
                    continue;
                string[] entry = ld.Split(',');

                Object obj = AssetDatabase.LoadMainAssetAtPath(entry[1]);
                if (obj == null)
                    continue;
                Data.Add(new HPOEntry(int.Parse(entry[0]), obj));
            }
        }


        public void SaveData()
        {
            string save = "";

            int count = 0;

            foreach (HPOEntry entry in Data)
            {
                if (entry.obj == null || !(EditorPlusHistoryInternal.favorites.Contains(entry.obj) || EditorPlusHistoryInternal.history.Contains(entry.obj)))
                    continue;
                save += entry.id.ToString() + "," + AssetDatabase.GetAssetPath(entry.obj.GetInstanceID()) + ";";
                count++;
            }
            EditorPrefs.SetString("EditorPlusHistory.ProjectObjs", save);
        }
    }
}