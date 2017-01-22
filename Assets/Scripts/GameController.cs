using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	public enum State
	{
		Start,
		Pause,
		InGame,
		End
	}
	public State currentState;

	void RunStates()
	{
		switch (currentState)
		{
			case State.Start:
				break;
			case State.Pause:
				break;
			case State.InGame:
				break;
			case State.End:
				break;
		}
	}

	protected int score = 0;
	public int Score { get { return score; } }

	protected int numberOfFloatsam = 0;
	public int NumberOfFloatsam { get { return numberOfFloatsam; } }

	protected int numberOfDestroyedObjects = 0;
	public int NumberOfDestroyedObjects { get { return numberOfDestroyedObjects; } }

	protected float finalWaveHeight = 0;
	public float FinalWaveHeight { get { return finalWaveHeight; } }

	void Awake()
	{
		instance = this;
	}

	void Update()
	{
		RunStates();
	}

	public void AddToScore(int scoreToAdd)
	{
		score += scoreToAdd;

		Debug.Log("Added " + scoreToAdd + " to Score:" + score);
	}

	public void RemoveFromScore(int scoreToRemove)
	{
		score -= scoreToRemove;
	}

	public void ResetScore()
	{
		score = 0;
	}

	public void IncramentFloatsam(int numberOfObjects = 1)
	{
		numberOfFloatsam += numberOfObjects;
	}

	public void DecramentFloatsam(int numberOfObjects = 1)
	{
		numberOfFloatsam -= numberOfObjects;
	}

	public void IncramentDestroyedObjects(int numberOfObjects = 1)
	{
		numberOfDestroyedObjects += numberOfObjects;
	}

	public void DecramentDestroyedObjects(int numberOfObjects = 1)
	{
		numberOfDestroyedObjects -= numberOfObjects;
	}

	public void SetFinalWaveHeight(float height)
	{
		finalWaveHeight = height;
	}
}
