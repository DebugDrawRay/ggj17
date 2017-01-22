using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Text curentHeight;
	public Text currentScore;

	void Update()
	{
		if (WaveStatusController.instance != null)
		{
			curentHeight.text = "HEIGHT: " + Mathf.CeilToInt(WaveStatusController.instance.scale);
		}

		if (GameController.instance != null)
		{
			currentScore.text = GameController.instance.Score.ToString();
		}
	}
}
