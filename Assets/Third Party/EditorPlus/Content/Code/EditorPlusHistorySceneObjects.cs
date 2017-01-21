using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EditorPlus
{
    [System.Serializable]
    /// <summary>
    /// Saved data for scene objects
    /// </summary>
    public class EditorPlusHistorySceneObjects : MonoBehaviour
    {
        [SerializeField]
        public int ClearID = -1;
        [SerializeField]
        public List<int> IDs;
        [SerializeField]
        public List<GameObject> OBJs;

        void Start()
        {
            if (IDs == null)
            {
                IDs = new List<int>();
            }

            if (OBJs == null)
            {
                OBJs = new List<GameObject>();
            }
            if(!Application.isEditor)
            {
                Destroy(this);
            }
        }


        public void Clear()
        {
            IDs.Clear();
            OBJs.Clear();
        }


        public void CheckConsistency()
        {
            if (IDs == null)
                IDs = new List<int>();
            if (OBJs == null)
                OBJs = new List<GameObject>();

            //if count of lists don't match - discard everything
            if(IDs.Count != OBJs.Count)
            {
                Debug.LogWarning("Scene object data corrupted - discarding");
                Clear();
                return;
            }

            for (int i = 0; i < OBJs.Count; ++i)
            {
                if(OBJs[i] == null)
                {
                    OBJs.RemoveAt(i);
                    IDs.RemoveAt(i);
                    --i;
                }
            }
        }


        public void Set(int objId, Object go)
        {
            if (IDs == null)
                IDs = new List<int>();
            if (OBJs == null)
                OBJs = new List<GameObject>();

            if (!OBJs.Contains((GameObject)go) && !IDs.Contains(objId))
            {
                OBJs.Add((GameObject)go);
                IDs.Add(objId);
            }
        }


        public int GetID(Object go)
        {
            if (IDs == null)
                IDs = new List<int>();
            if (OBJs == null)
                OBJs = new List<GameObject>();


            for (int i = 0; i < OBJs.Count; ++i )
            {
                if (OBJs[i] != null && OBJs[i] == go)
                {
                    return IDs[i];
                }
            }
            return -1;
        }


        public Object GetOBJ(int id)
        {
            if (IDs == null)
                IDs = new List<int>();
            if (OBJs == null)
                OBJs = new List<GameObject>();

            int index = IDs.IndexOf(id);
            if(index != -1)
            {
                return OBJs[index];
            }
            return null;
        }
    }
}