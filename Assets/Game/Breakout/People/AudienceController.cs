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

    void Start()
    {
        UpdateBallsList();
        Animator = GetComponent<Animator>();

        SetEmote(Emotes.Idle);
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
                    if (Attitude == Attitudes.Boy) Animator.Play("Base Layer.Idle01");
					if (Attitude == Attitudes.Girl) Animator.Play("Base Layer.Idle02");
					break;
                }
        }
    }
}
