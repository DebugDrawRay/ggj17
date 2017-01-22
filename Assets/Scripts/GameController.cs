﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController instance;

	protected int score;
	public int Score { get { return score; } }

	protected int numberOfFloatsam;
	public int NumberOfFloatsam { get { return numberOfFloatsam; } }

	protected int numberOfDestroyedObjects;
	public int NumberOfDestroyedObjects { get { return numberOfDestroyedObjects; } }

	protected float finalWaveHeight;
	public float FinalWaveHeight { get { return finalWaveHeight; } }

	void Awake()
	{
		instance = this;
	}

	public void AddToScore(int scoreToAdd)
	{
		score += scoreToAdd;
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
