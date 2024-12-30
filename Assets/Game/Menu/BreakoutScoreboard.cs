using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BreakoutScoreboard : MonoBehaviour
{
    private Text text;

    void Start()
    {
        text = GetComponent<Text>();
    }

	private void Update()
	{
		// SetText();
	}

	private string RandomizeString(string s)
    {
        string res = "";
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		foreach (char c in s)
        {
            res += $"{chars[Random.Range(0, chars.Length)]}";
        }
        return res;
    }

    private void SetText()
    {
        /*
		High score: 000000000
        ________________________________
        Start with 90 seconds...........100000 ✔
        Start with x2 multiplier..........200000 ✔
        Start with 120 seconds   300000 ✔
        Start with 2 balls           400000 ✔
        Start with x5 multiplier   800000 ✔
        */

        int highScore = XMLManager.instance.data.GetHighScore();

		string s = "";
        s += $"High score: {highScore,8}\n";
        s += $"________________________________\n";

        bool foundFirstNotCompleted = false;
        foreach (var bonus in LevelStartBonus.Bonuses)
        {
            bool hideText = false;
            if (highScore < bonus.HighScoreThreshold && foundFirstNotCompleted)
            {
                hideText = true;
            }

			if (hideText)
			{
				s += $"{RandomizeString(bonus.Description)}    ???\n";
			}
			else
            {
                s += $"{bonus.Description}    {bonus.HighScoreThreshold}";

                if (highScore > bonus.HighScoreThreshold)
                {
                    s += " ✔";
				}
                s += "\n";
            }

			if (highScore < bonus.HighScoreThreshold && !foundFirstNotCompleted)
            {
                foundFirstNotCompleted = true;
            }
        }

        text.text = s;
    }
}
