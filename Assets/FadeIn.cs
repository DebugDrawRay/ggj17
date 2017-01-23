using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class FadeIn : MonoBehaviour
{
    public Image target;
    void Start()
    {
        Color clear = Color.white;
        clear.a = 0;
        target.DOColor(Color.white, 3).OnComplete(() => target.DOColor(clear, 3).OnComplete(() => SceneManager.LoadScene("test")));
    }
}
