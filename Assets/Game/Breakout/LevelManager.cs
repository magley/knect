using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	private int destroyedBallsOnLevelEnd = 0;
	[SerializeField] private GameObject PrefabWorldSpaceTextForScore;
	private AudioSource sndBallDestroyOnLevelEnd;

	private bool ShouldTickTimeLeft = true;
	[SerializeField] float seconds = 60;
	[SerializeField] private Text TimeLeftText;
	private float secondsLeft = 60;
	[SerializeField] private AudioSource DrumRollLoop;
	[SerializeField] private AudioClip SndDrumRollEnd;

	private bool ShouldAddPointsForRemainingTime = false;

	[SerializeField] private List<GameObject> PrefabWavesInOrder = new List<GameObject>();
	private int waveIndex = 0;
	private int waveCounter = 0;

	void Start()
	{
		destroyedBallsOnLevelEnd = 0;
		sndBallDestroyOnLevelEnd = GetComponent<AudioSource>();

		secondsLeft = seconds;

		GameState.ResetScore();

		Invoke(nameof(SpawnNextWave), 1.5f);
	}

	public void SpawnNextWave()
	{
		if (PrefabWavesInOrder.Count > 0)
		{
			var obj = PrefabWavesInOrder[waveIndex];
			Instantiate(obj);
			waveIndex = (waveIndex + 1) % (PrefabWavesInOrder.Count);
			waveCounter++;
		}
		else
		{
			Debug.LogError("No wave prefabs found");
		}
	}

	void Update()
	{
		UpdateTimeLeft();
	}

	private void UpdateTimeLeft()
	{
		// Display time left.

		int minutes = (int)secondsLeft / 60;
		int seconds = (int)secondsLeft % 60;
		TimeLeftText.text = $"{minutes.ToString("D2")}:{seconds.ToString("D2")}";
		TimeLeftText.text += $"\nWave {waveCounter}";

		// Tick time.

		if (ShouldTickTimeLeft)
		{
			secondsLeft -= Time.deltaTime;
			if (secondsLeft <= 0)
			{
				TimeIsUp();
			}
		}

		if (ShouldAddPointsForRemainingTime)
		{
			if (secondsLeft > 0)
			{
				secondsLeft -= 1;
				GameState.AddScore(100);

				if (secondsLeft <= 0)
				{
					DrumRollLoop.Stop();
					DrumRollLoop.clip = SndDrumRollEnd;
					DrumRollLoop.loop = false;
					DrumRollLoop.Play();

					secondsLeft = 0;
					Invoke(nameof(PromptForNextLevel), 0.75f);
				}
			}
		}
	}

	private void TimeIsUp()
	{

	}

	public void OnLevelComplete()
	{
		ShouldTickTimeLeft = false;
		Invoke(nameof(DestoryRandomBall), 1f);
	}

	private void DestoryRandomBall()
	{
		var balls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID).ToList();
		balls.Sort((Ball b1, Ball b2) => { return b1.transform.position.z.CompareTo(b2.transform.position.z); });
		balls.Reverse();
		if (balls.Count == 0)
		{
			Invoke(nameof(OnAllBallsDestroyed), 1f);
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
		ShouldAddPointsForRemainingTime = true;
		DrumRollLoop.Play();
	}

	private void PromptForNextLevel()
	{

	}
}
