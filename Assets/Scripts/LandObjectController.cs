using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandObjectController : MonoBehaviour
{
	public int pickupScore;

	void OnCollisionEnter(Collision collision)
	{
		if (GameController.instance != null)
		{
			GameController.instance.AddToScore(pickupScore);
			GameController.instance.IncramentDestroyedObjects();
		}
	}
}
