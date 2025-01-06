using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoriaController : MonoBehaviour
{
	private float beforeShowTutorialTime = 1f;

    [SerializeField] private GameObject clearArea;
    [SerializeField] private Image playerIcon;
	[SerializeField] private Image sofa;
	[SerializeField] private Image chair;
	[SerializeField] List<Sprite> sprPlayerIdleFrames;
    private float clearArea_timeUntilMove = 1.5f;
    private float clearArea_timeUntilMoveStop = 1.5f;
	private CanvasGroup clearAreaGroup;

	[SerializeField] private GameObject hitBall;
    [SerializeField] private Image playerIcon02;
	[SerializeField] private Image wall;
	[SerializeField] private Image ball;
	[SerializeField] List<Sprite> sprPlayerHitBallFrames;
	[SerializeField] List<Sprite> sprWallFrames;
	private float hitBall_timeUntilMove = 1f;
	private float hitBall_timeUntilHitHand = 0.5f;
	private float hitBall_timeUntilHitWall = 0.5f;
	private float hitBall_timeUntilEnd = 1f;
	private CanvasGroup hitBallGroup;

	[SerializeField] private GameObject pause;
	[SerializeField] private Image playerIconPause;
	[SerializeField] List<Sprite> sprPlayerPauseFrames;
	private float pause_timeUntilEnd = 3.5f;
	private CanvasGroup pauseGroup;

	private void AnimatePlayerIcon()
    {
		if (clearArea.activeInHierarchy)
		{
			playerIcon.sprite = sprPlayerIdleFrames[(int)(Time.time * 1.5f) % sprPlayerIdleFrames.Count];
		}

		if (pause.activeInHierarchy)
		{
			playerIconPause.sprite = sprPlayerPauseFrames[(int)(Time.time * 1.25f) % sprPlayerPauseFrames.Count];
		}

	}

	void Start()
    {
		clearAreaGroup = clearArea.GetComponentInChildren<CanvasGroup>();
		hitBallGroup = hitBall.GetComponentInChildren<CanvasGroup>();
		pauseGroup = pause.GetComponentInChildren<CanvasGroup>();

		clearArea.SetActive(false);
		hitBall.SetActive(false);
		pause.SetActive(false);

		clearAreaGroup.alpha = 0f;
		hitBallGroup.alpha = 0f;
		pauseGroup.alpha = 0f;
	}

	private void HandleBeforeTutorialBegins()
	{
		beforeShowTutorialTime -= Time.deltaTime;
		if (beforeShowTutorialTime <= 0f)
		{
			clearArea.SetActive(true);
		}
	}

	private void HandleClearArea()
    {
        if (!clearArea.activeInHierarchy)
        {
            return;
        }
		clearAreaGroup.alpha += Time.deltaTime * 3f;
		if (clearAreaGroup.alpha >= 1f)
		{
			clearAreaGroup.alpha = 1f;
		}
		else
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
			else
			{
				hitBall.SetActive(true);
			}
		}
	}

	private void HandleHitBall()
	{
		if (!hitBall.activeInHierarchy)
		{
			return;
		}
		if (hitBallGroup.alpha < 1f)
		{
			hitBallGroup.alpha += Time.deltaTime * 3f;
			if (hitBallGroup.alpha < 1f)
			{
				return;
			}
		}

		hitBall_timeUntilMove -= Time.deltaTime;

		if (hitBall_timeUntilMove <= 0f)
		{
			if (hitBall_timeUntilHitHand > 0f)
			{
				hitBall_timeUntilHitHand -= Time.deltaTime;
				ball.rectTransform.position += new Vector3(220, -59, 0) * Time.deltaTime;
			}
			if (hitBall_timeUntilHitHand <= 0f)
			{
				playerIcon02.sprite = sprPlayerHitBallFrames[1];

				if (hitBall_timeUntilHitWall > 0f)
				{
					hitBall_timeUntilHitWall -= Time.deltaTime;
					ball.rectTransform.position += new Vector3(-240, 84, 0) * Time.deltaTime;
				}

				if (hitBall_timeUntilHitWall <= 0f)
				{
					wall.sprite = sprWallFrames[1];	
					hitBall_timeUntilEnd -= Time.deltaTime;

					if (hitBall_timeUntilEnd <= 0f)
					{
						pause.SetActive(true);
					}
				}
			}
		}
	}

	private void HandleItems()
	{
		if (!pause.activeInHierarchy)
		{
			return;
		}
		if (pauseGroup.alpha < 1f)
		{
			pauseGroup.alpha += Time.deltaTime * 3f;
			if (pauseGroup.alpha < 1f)
			{
				return;
			}
		}

		pause_timeUntilEnd -= Time.deltaTime;
		if (pause_timeUntilEnd <= 0f)
		{
			gameObject.SetActive(false);
		}
	}

	void Update()
    {
        AnimatePlayerIcon();
		HandleBeforeTutorialBegins();
		HandleClearArea();
		HandleHitBall();
		HandleItems();
	}
}
