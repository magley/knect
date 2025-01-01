using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAdditions : MonoBehaviour
{
    public static float scoreMultiplierTimeLeft = 0;
	public static int ScoreMultiplier = 1;
	public static int ScoreMultiplierTimeBase = 10;
	public static bool ForceScoreMultiplier5 = false;
	public static bool UnlockScoreMultiplier10 = false;

	private void Awake()
	{
		ForceScoreMultiplier5 = false;
		UnlockScoreMultiplier10 = false;
	}

	void Update()
    {
		HandleScoreMultiplierDepletion();
	}

	public static void SetScoreMultiplier(int multiplier)
	{
		ScoreMultiplier = multiplier;
		scoreMultiplierTimeLeft = ScoreMultiplierTimeBase;
	}

    private void HandleScoreMultiplierDepletion()
    {
		if (scoreMultiplierTimeLeft > 0)
		{
			scoreMultiplierTimeLeft -= 1 * Time.deltaTime;
			if (scoreMultiplierTimeLeft < 0)
			{
				scoreMultiplierTimeLeft = 0;
				ScoreMultiplier = 1;
			}
		}
	}
}
