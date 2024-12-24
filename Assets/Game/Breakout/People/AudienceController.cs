using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private bool followingBalls = true;
    private float lookAtLerpSpeed = 0.25f;

    /// <summary>
    /// Time (in seconds) until the emote is set to idle again.
    /// </summary>
    private float setEmoteToIdleTimer = 0f;

    void Start()
    {
        UpdateBallsList();
        Animator = GetComponent<Animator>();

        SetEmote(Emotes.Idle);
        Ball.OnComboMade += OnComboMade;
	}

	private void OnDestroy()
	{
        Ball.OnComboMade -= OnComboMade;
	}

	void OnComboMade(int combo)
    {
        if (combo >= 3 && combo < 10)
        {
            SetEmote(Emotes.Clap);
            setEmoteToIdleTimer = Random.Range(1f, 2f) + combo / 4.5f;

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
        lookAtLerpSpeed = Random.Range(0.15f, 0.65f);

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

        // Find the centroid of all balls.

        Vector3 ballCentroid = Vector3.zero;
        foreach (var ball in ballsIamAwareOf)
        {
            ballCentroid += ball.transform.position;
        }
        ballCentroid /= ballsIamAwareOf.Count;

        // Look at centroid, with lerp.

		Vector3 relativePos = ballCentroid - boneHead.transform.position;
		Quaternion toRotation = Quaternion.LookRotation(relativePos);
		boneHead.transform.rotation = Quaternion.Lerp(boneHead.transform.rotation, toRotation, lookAtLerpSpeed);
    }

    private void SetEmote(Emotes emote)
    {
        switch (emote)
        {
            case Emotes.Idle:
                {
                    if (Attitude == Attitudes.Boy) Animator.CrossFade("Base Layer.Idle01", 0.5f);
					if (Attitude == Attitudes.Girl) Animator.CrossFade("Base Layer.Idle02", 0.5f);
					break;
                }
			case Emotes.Clap:
				{
					if (Attitude == Attitudes.Boy) Animator.CrossFade("Base Layer.Clap01", 0.5f);
					if (Attitude == Attitudes.Girl) Animator.CrossFade("Base Layer.Clap02", 0.5f);
					break;
				}
		}
    }
}
