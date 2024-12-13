using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour
{
	private int destroyedBallsOnLevelEnd = 0;
	[SerializeField] private GameObject PrefabWorldSpaceTextForScore;
	private AudioSource sndBallDestroyOnLevelEnd;

    void Start()
    {
		destroyedBallsOnLevelEnd = 0;
		sndBallDestroyOnLevelEnd = GetComponent<AudioSource>();
	}

    void Update()
    {
        
    }

	public void OnLevelComplete()
	{
		Invoke(nameof(DestoryRandomBall), 1f);
	}

	private void DestoryRandomBall()
	{
		var balls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID).ToList();
		balls.Sort((Ball b1, Ball b2) => { return b1.transform.position.z.CompareTo(b2.transform.position.z); });
		balls.Reverse();
		if (balls.Count == 0)
		{
			Invoke(nameof(OnAllBallsDestroyed), 2f);
		}
		else
		{
			destroyedBallsOnLevelEnd++;
			int points = (destroyedBallsOnLevelEnd * destroyedBallsOnLevelEnd) * 500;
			int i = 0;
			GameObject ball = balls[i].gameObject;

			int pointsAfterMultiplier = GameState.AddScore(points);
			WorldSpaceText pointsText = Instantiate(PrefabWorldSpaceTextForScore).GetComponent<WorldSpaceText>();
			pointsText.SetText($"{pointsAfterMultiplier}");
			pointsText.SetSize(5 + 3 * destroyedBallsOnLevelEnd);
			pointsText.gameObject.transform.position = ball.transform.position;
			Destroy(ball);

			sndBallDestroyOnLevelEnd.Play();
			sndBallDestroyOnLevelEnd.pitch += 0.15f;

			Invoke(nameof(DestoryRandomBall), 0.25f);
		}
	}

	private void OnAllBallsDestroyed()
	{

	}
}
