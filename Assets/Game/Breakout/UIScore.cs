using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    [SerializeField] private Text textScore;
    private RectTransform textScoreTransform;
    private float textPosY;
    private float textPosYbase;

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

    private void OnScoreAdded(int newScore, int deltaScore)
    {
        textScore.text = $"{newScore:00000000}";
        textPosY += 27;
    }
}
