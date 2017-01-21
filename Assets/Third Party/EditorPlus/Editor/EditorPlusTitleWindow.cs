using UnityEngine;
using UnityEditor;
using System.Collections;


#pragma warning disable 618

namespace EditorPlus
{
    public class EditorPlusTitleWindow : EditorWindow
    {
        public EditorWindow tgt;

        public static void Init(object targetWindow)
        {
            EditorPlusTitleWindow window = (EditorPlusTitleWindow)ScriptableObject.CreateInstance("EditorPlusTitleWindow");

            window.tgt = (EditorWindow)targetWindow;
            window.ShowAsDropDown(new Rect(window.tgt.position.x, window.tgt.position.y, 0, 0), new Vector2(200, 25f));
        }

        void OnGUI()
        {
            Event e = Event.current;
            string newTitle = tgt.title;
            newTitle = EditorGUILayout.TextField(tgt.title);

            if (tgt.title != newTitle)
            {
                if (tgt.GetType() == typeof(EditorPlusHotbar))
                {
                    EditorPlusHotbar casted = ((EditorPlusHotbar)tgt);
                    if (casted.useAutoSave)
                    {
                        casted.ClearSave(tgt.title);
                        tgt.title = newTitle;
                        casted.SaveHotbar();
                    }
                }
                tgt.title = newTitle;
                tgt.Repaint();
            }

            if (e.isKey)
            {
                if (e.keyCode == KeyCode.Return)
                {
                    Close();
                }
            }
        }
    }
}