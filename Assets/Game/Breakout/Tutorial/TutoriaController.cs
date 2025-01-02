using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoriaController : MonoBehaviour
{
    [SerializeField] private GameObject clearArea;
    [SerializeField] private Image playerIcon;
	[SerializeField] private Image sofa;
	[SerializeField] private Image chair;
    private float clearArea_timeUntilMove = 2f;
    private float clearArea_timeUntilMoveStop = 1.5f;
	[SerializeField] List<Sprite> sprPlayerIdleFrames;

	[SerializeField] private GameObject hitBall;
    [SerializeField] private Image playerIcon02;
	[SerializeField] private Image wall;
	[SerializeField] private Image ball;
	[SerializeField] List<Sprite> sprPlayerHitBallFrames;
	[SerializeField] List<Sprite> sprWallFrames;

	private float hitBall_timeUntilMove = 2f;
	private float hitBall_timeUntilHitHand = 0.7f;
	private float hitBall_timeUntilHitWall = 0.7f;

	private void AnimatePlayerIcon()
    {
        playerIcon.sprite = sprPlayerIdleFrames[(int)(Time.time * 1.5f) % sprPlayerIdleFrames.Count];
    }

    void Start()
    {
        
    }

    private void HandleClearArea()
    {
        if (!clearArea.activeSelf)
        {
            return;
        }

        clearArea_timeUntilMove -= Time.deltaTime;

        if (clearArea_timeUntilMove <= 0f)
        {
			if (clearArea_timeUntilMoveStop > 0f)
            {
                clearArea_timeUntilMoveStop -= Time.deltaTime;

				chair.rectTransform.position += new Vector3(-38, 38, 0) * Time.deltaTime;
				sofa.rectTransform.position += new Vector3(38, 48, 0) * Time.deltaTime;
			}
		}
	}

	private void HandleHitBall()
	{
		if (!hitBall.activeSelf)
		{
			return;
		}

		hitBall_timeUntilMove -= Time.deltaTime;

		if (hitBall_timeUntilMove <= 0f)
		{
			if (hitBall_timeUntilHitHand > 0f)
			{
				hitBall_timeUntilHitHand -= Time.deltaTime;
				ball.rectTransform.position += new Vector3(180, -42, 0) * Time.deltaTime;
			}
			if (hitBall_timeUntilHitHand <= 0f)
			{
				playerIcon02.sprite = sprPlayerHitBallFrames[1];

				if (hitBall_timeUntilHitWall > 0f)
				{
					hitBall_timeUntilHitWall -= Time.deltaTime;
					ball.rectTransform.position += new Vector3(-200, 70, 0) * Time.deltaTime;
				}

				if (hitBall_timeUntilHitWall <= 0f)
				{
					wall.sprite = sprWallFrames[1];
				}
			}
		}
	}

	void Update()
    {
        AnimatePlayerIcon();
        HandleClearArea();
		HandleHitBall();

	}
}
