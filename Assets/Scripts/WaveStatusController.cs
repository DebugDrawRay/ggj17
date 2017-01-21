using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveStatusController : MonoBehaviour
{
	public GameObject[] waveDisplays;
	public float[] waveChangeThresholds;
	protected int currentWaveThreshold = 0;

	[HideInInspector]
	public float scale = 1f;

	protected float scaleSpeed = 1;
	protected float negateAmount = 0.5f;
	protected float scaleDecayRate = 0.05f;


	void Update()
	{
		//Scale Down over time
		scale -= scaleDecayRate * Time.deltaTime;

		//Move scale of wave toward desired scale
		transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(scale, scale, scale), scaleSpeed * Time.deltaTime);

		if (currentWaveThreshold < waveChangeThresholds.Length - 1 && scale > waveChangeThresholds[currentWaveThreshold + 1])
		{
			waveDisplays[currentWaveThreshold + 1].SetActive(true);
			waveDisplays[currentWaveThreshold].SetActive(false);
			currentWaveThreshold++;
		}
		else if(currentWaveThreshold > 0 && scale < waveChangeThresholds[currentWaveThreshold])
		{
			waveDisplays[currentWaveThreshold - 1].SetActive(true);
			waveDisplays[currentWaveThreshold].SetActive(false);
			currentWaveThreshold--;
		}

		if (scale <= 0)
		{
			//Death
		}
	}

	void OnTriggerEnter(Collider collider)
	{
		OceanBodyData data = collider.gameObject.GetComponent<OceanBodyData>();
		if (data != null)
		{
			if (data.type == OceanBodyData.OceanBodyType.EnemyWave)
			{
				//If enemy wave is bigger than you
				if (data.scale > scale)
				{
					//take damage based on wave scale
					scale -= data.scale * negateAmount;
				}
				else
				{
					//Eat enemy wave
					scale += data.scale;
				}
			}

			//Call for enemy wave destruction
			//TEMP CODE
			Destroy(data.gameObject);
		}
	}
}
