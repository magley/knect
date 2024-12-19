using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static int Score = 0;
    public static event Action<int, int> ScoreChanged;

    public static int BestCombo = 0;
    public static int TotalWaves = 0;

    public static void ResetScore()
    {
        Score = 0;
        BestCombo = 0;
        TotalWaves = 0;
    }

    public static int AddScore(int added)
    {
        Score += added * PlayerAdditions.ScoreMultiplier;
        ScoreChanged(Score, added * PlayerAdditions.ScoreMultiplier);
        return added * PlayerAdditions.ScoreMultiplier;
	}
}
