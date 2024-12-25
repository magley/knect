using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AudienceController : MonoBehaviour
{
    /// <summary>
    /// The attitude determines which set of animation is used for emotes.
    /// </summary>
	public enum Attitudes
	{
		Boy,
		Girl,
	}

    /// <summary>
    /// Predefined animation for an audience member.
    /// </summary>
    private enum Emotes
    {
        Idle,
        Clap,
        Cheer,
    }

	[SerializeField] Attitudes Attitude = Attitudes.Boy;
	[SerializeField] private GameObject boneHead;
	private Animator Animator;

	/// <summary>
	/// I follow only the balls in this list.
	/// The list gets updated every X seconds to improve
	/// performance (because the list of balls is volatile).
	/// </summary>
	private List<Ball> ballsIamAwareOf = new List<Ball>();
    private GameObject focusObject;
    private bool followingBalls = true;
    private float lookAtLerpSpeed = 0.25f;

    /// <summary>
    /// The combo you're currently cheering for.
    /// If -1, then no combo.
    /// </summary>
    private int cheeringForCombo = -1;

    /// <summary>
    /// Time (in seconds) until the emote is set to idle again.
    /// </summary>
    private float setEmoteToIdleTimer = 0f;

    private AudienceGroupController audienceGroup;

	void Start()
    {
        UpdateBallsList();
        Animator = GetComponent<Animator>();

        SetEmote(Emotes.Idle);
        Ball.OnComboMade += OnComboMade;

        focusObject = FindObjectsOfType<Transform>(false).Where(o => o.tag == "Player").First().gameObject;
		audienceGroup = transform.parent.GetComponent<AudienceGroupController>();
	}

	private void OnDestroy()
	{
        Ball.OnComboMade -= OnComboMade;
	}

	void OnComboMade(int combo)
    {
        if (combo < 0)
        {
            return;
        }

        cheeringForCombo = Mathf.Max(cheeringForCombo, combo);

		if (cheeringForCombo >= 3 && cheeringForCombo < 10)
        {
            SetEmote(Emotes.Clap);
            setEmoteToIdleTimer = Random.Range(1f, 2f) + combo / 4.5f;
            audienceGroup.PlayClap();
		}
        else if (cheeringForCombo >= 10)
		{
			SetEmote(Emotes.Cheer);
			setEmoteToIdleTimer = Random.Range(1f, 2f) + (combo - 5) / 6.5f;
			audienceGroup.PlayCheer();
		}
	}

	private void Update()
	{
        if (setEmoteToIdleTimer > 0f)
        {
            setEmoteToIdleTimer -= Time.deltaTime;
            if (setEmoteToIdleTimer <= 0)
            {
                setEmoteToIdleTimer = 0f;
                SetEmote(Emotes.Idle);
                cheeringForCombo = -1;
            }
        }
	}

	void LateUpdate()
    {
        FollowBalls();
	}

    private void UpdateBallsList()
    {
        ballsIamAwareOf = FindObjectsByType<Ball>(FindObjectsSortMode.None).ToList();
        if (ballsIamAwareOf.Count > 0)
        {
			focusObject = ballsIamAwareOf[Random.Range(0, ballsIamAwareOf.Count)].gameObject;
        }

        lookAtLerpSpeed = Random.Range(0.15f, 0.35f);
		if (followingBalls)
        {
            Invoke(nameof(UpdateBallsList), 3f);
        }
    }

    private void FollowBalls()
    {
        if (ballsIamAwareOf.Count == 0)
        {
			return;
		}

        // Ugly ugly ugly code.

        if (focusObject.IsDestroyed()) 
        {
			ballsIamAwareOf = FindObjectsByType<Ball>(FindObjectsSortMode.None).ToList();
			if (ballsIamAwareOf.Count > 0)
			{
				focusObject = ballsIamAwareOf[Random.Range(0, ballsIamAwareOf.Count)].gameObject;

				Vector3 focusPoint = focusObject.transform.position;
				Vector3 relativePos = focusPoint - boneHead.transform.position;
				Quaternion toRotation = Quaternion.LookRotation(relativePos);
				boneHead.transform.rotation = Quaternion.Lerp(boneHead.transform.rotation, toRotation, lookAtLerpSpeed);
			}
		} 
        else
        {
			Vector3 focusPoint = focusObject.transform.position;
			Vector3 relativePos = focusPoint - boneHead.transform.position;
			Quaternion toRotation = Quaternion.LookRotation(relativePos);
			boneHead.transform.rotation = Quaternion.Lerp(boneHead.transform.rotation, toRotation, lookAtLerpSpeed);

		}
	}

    private void SetEmote(Emotes emote)
    {
        switch (emote)
        {
            case Emotes.Idle:
                {
                    if (Attitude == Attitudes.Boy) Animator.CrossFade("Base Layer.Idle01", 0.85f);
					if (Attitude == Attitudes.Girl) Animator.CrossFade("Base Layer.Idle02", 0.85f);
					break;
                }
			case Emotes.Clap:
				{
					if (Attitude == Attitudes.Boy) Animator.CrossFade("Base Layer.Clap01", 0.5f);
					if (Attitude == Attitudes.Girl) Animator.CrossFade("Base Layer.Clap02", 0.5f);
					break;
				}
			case Emotes.Cheer:
				{
					if (Attitude == Attitudes.Boy) Animator.CrossFade("Base Layer.Cheer01", 0.35f);
					if (Attitude == Attitudes.Girl) Animator.CrossFade("Base Layer.Cheer02", 0.35f);
					break;
				}
		}
    }
}
