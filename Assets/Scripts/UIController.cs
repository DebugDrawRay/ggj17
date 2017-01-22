using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
public class UIController : MonoBehaviour
{
    [Header("Title Screen")]
    public Text title;
    public Button startButton;
    [Header("In Game")]
    public Text curentHeight;
    public Text score;
    [Header("Results")]
    public Text[] results;
    public Button continueButton;

    public static UIController instance;

    void Awake()
    {
        instance = this;
        foreach (Text text in results)
        {
            Color full = text.color;
            full.a = 0;
            text.color = full;
        }
        continueButton.gameObject.SetActive(false);

        Color clear = Color.white;
        clear.a = 0;

        title.color = clear;
        title.DOColor(Color.white, 2f).SetEase(Ease.Linear);
    }
    void Update()
	{
		if (WaveStatusController.instance != null)
		{
			curentHeight.text = "HEIGHT: " + Mathf.CeilToInt(WaveStatusController.instance.scale) + "m";
		}
	}

    public void StartGame()
    {
        CameraController.instance.MoveToPlayer(() => GameController.instance.currentState = GameController.State.InGame);
        curentHeight.DOColor(Color.white, 1f);
        score.DOColor(Color.white, 1f);
        startButton.gameObject.SetActive(false);
        Color clear = title.color;
        clear.a = 0;
        title.DOColor(clear, 1f);
    }

    public void DisplayResults()
    {
        GameController.instance.currentState = GameController.State.End;
        foreach(Text text in results)
        {
            Color full = text.color;
            full.a = 1;
            text.DOColor(full, 1f);
        }
        continueButton.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
