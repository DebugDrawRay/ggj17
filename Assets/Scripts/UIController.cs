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
    public Text subtitle;
    public Button startButton;

    [Header("In Game")]
    public Text curentHeight;
	public Text currentCollected;
    public Text score;

    [Header("Results")]
    public Text[] results;
	public Text sizeResult;
	public Text gatheredResult;
	public Text destroyedResult;
	public Text totalPointsResult;
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
        subtitle.color = clear;
        subtitle.DOColor(Color.white, 2f).SetEase(Ease.Linear);
    }
    void Update()
	{
		if (WaveStatusController.instance != null)
		{
			curentHeight.text = "HEIGHT: " + Mathf.CeilToInt(WaveStatusController.instance.scale) + "m";
			//if (GameController.instance != null)
			//	currentCollected.text = GameController.instance.Score.ToString();
		}
	}

    public void StartGame()
    {
        title.DOKill(true);
        subtitle.DOKill(true);
        CameraController.instance.MoveToPlayer(() => GameController.instance.currentState = GameController.State.InGame);
        curentHeight.DOColor(Color.white, 1f);
        score.DOColor(Color.white, 1f);
        startButton.gameObject.SetActive(false);
        Color clear = title.color;
        clear.a = 0;
        title.DOColor(clear, 1f);
        subtitle.DOColor(clear, 1f);

    }

    public void DisplayResults()
    {
        GameController.instance.currentState = GameController.State.End;

		//Fill in information
		if (GameController.instance != null)
		{
			sizeResult.text = GameController.instance.FinalWaveHeight.ToString();
			gatheredResult.text = GameController.instance.NumberOfFloatsam.ToString();
			destroyedResult.text = GameController.instance.NumberOfDestroyedObjects.ToString();
			totalPointsResult.text = GameController.instance.Score.ToString();
		}

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
