using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public Text curentHeight;

	void Update()
	{
		if (WaveStatusController.instance != null)
		{
			curentHeight.text = "HEIGHT: " + Mathf.CeilToInt(WaveStatusController.instance.scale);
		}
	}
}
