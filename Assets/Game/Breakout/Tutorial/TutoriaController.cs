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
	private float hitBall_timeUntilHitHand = 0.7f;
	private float hitBall_timeUntilHitWall = 0.7f;
	private float hitBall_timeUntilEnd = 1f;
	private CanvasGroup hitBallGroup;

	[SerializeField] private GameObject items;
	private float items_timeUntilEnd = 4f;
	private CanvasGroup itemsGroup;

	private void AnimatePlayerIcon()
    {
        playerIcon.sprite = sprPlayerIdleFrames[(int)(Time.time * 1.5f) % sprPlayerIdleFrames.Count];
    }

    void Start()
    {
		clearAreaGroup = clearArea.GetComponentInChildren<CanvasGroup>();
		hitBallGroup = hitBall.GetComponentInChildren<CanvasGroup>();
		itemsGroup = items.GetComponentInChildren<CanvasGroup>();

		clearArea.SetActive(true);
		hitBall.SetActive(false);
		items.SetActive(false);

		clearAreaGroup.alpha = 0f;
		hitBallGroup.alpha = 0f;
		itemsGroup.alpha = 0f;
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
					hitBall_timeUntilEnd -= Time.deltaTime;

					if (hitBall_timeUntilEnd <= 0f)
					{
						items.SetActive(true);
					}
				}
			}
		}
	}

	private void HandleItems()
	{
		if (!items.activeInHierarchy)
		{
			return;
		}
		if (itemsGroup.alpha < 1f)
		{
			itemsGroup.alpha += Time.deltaTime * 3f;
			if (itemsGroup.alpha < 1f)
			{
				return;
			}
		}

		items_timeUntilEnd -= Time.deltaTime;
		if (items_timeUntilEnd <= 0f)
		{
			Destroy(gameObject);
		}
	}

	void Update()
    {
        AnimatePlayerIcon();
        HandleClearArea();
		HandleHitBall();
		HandleItems();
	}
}
