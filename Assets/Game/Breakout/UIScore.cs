using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIScore : MonoBehaviour
{
    [SerializeField] private Text textScore;
    private RectTransform textScoreTransform;
    private float textPosY;
    private float textPosYbase;

	[SerializeField] private Text scoreMultiplierText;
	[SerializeField] private GameObject scoreMultiplierImageObject;

	void Start()
    {
        GameState.ScoreChanged += OnScoreAdded;

        textScoreTransform = textScore.GetComponent<RectTransform>();
		textPosYbase = textScoreTransform.anchoredPosition.y;
		textPosY = textScoreTransform.anchoredPosition.y;
	}

    void Update()
    {
		HandleTextBounce();
		HandleScoreMultiplier();
	}

    private void HandleTextBounce()
    {
		if (textPosY > textPosYbase)
		{
			textPosY -= 5;

			if (textPosY < textPosYbase)
			{
				textPosY = textPosYbase;
			}
		}

		Vector2 p = textScoreTransform.anchoredPosition;
		p.y = textPosY;
		textScoreTransform.anchoredPosition = p;
	}

	private void HandleScoreMultiplier()
	{
		if (PlayerAdditions.ScoreMultiplier == 1)
		{
			scoreMultiplierText.enabled = false;
			scoreMultiplierImageObject.SetActive(false);
		}
		else
		{
			scoreMultiplierText.enabled = true;
			scoreMultiplierImageObject.SetActive(true);

			scoreMultiplierText.text = $"x{PlayerAdditions.ScoreMultiplier}";

			float ratio = PlayerAdditions.scoreMultiplierTimeLeft / (float)PlayerAdditions.ScoreMultiplierTimeBase;
			scoreMultiplierImageObject.GetComponent<RectTransform>().localScale = new Vector2(ratio, 1);
		}
	}

    private void OnScoreAdded(int newScore, int deltaScore)
    {
        textScore.text = $"{newScore:00000000}";
        textPosY = textPosYbase + 27;
    }
}
