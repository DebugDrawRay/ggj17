using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusClearScenes
    {
        private static List<string> Scenes = new List<string>();

        static EditorPlusClearScenes()
        {

        }

        public static void Run()
        {
            
            Scenes.Clear();
            FindScenes("Assets");
            EditorPlusHistoryInternal.historyUseSceneSaving = false;
            EditorApplication.update += Update;
        }


        public static void FindScenes(string dir)
        {
            try
            {
                Scenes.AddRange(Directory.GetFiles(dir, "*.unity", SearchOption.AllDirectories));
            }
            catch (System.Exception excpt)
            {
                Debug.LogError(excpt.Message);
            }
        }
       

        static void Update()
        {
            EditorPlusHistorySceneObjects[] objs = MonoBehaviour.FindObjectsOfType<EditorPlusHistorySceneObjects>();
            if (objs.Length > 0)
            {
                foreach (EditorPlusHistorySceneObjects ob in objs)
                {
                    MonoBehaviour.DestroyImmediate(ob.gameObject);
                }
                EditorApplication.SaveScene();

            }
            if (Scenes.Count > 0)
            {
                EditorApplication.OpenScene(Scenes[0]);
                Scenes.RemoveAt(0);
            }
            else
            {
                EditorApplication.update -= Update;
            }
        }
    }
}