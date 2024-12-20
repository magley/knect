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
	[SerializeField] private AudioClip SndRefereeWhistle;
	[SerializeField] private AudioClip SndDrumRoll;
	[SerializeField] private AudioClip SndDrumRollEnd;

	[SerializeField] private List<GameObject> PrefabWavesInOrder = new List<GameObject>();
	private int waveIndex = 0;
	private int waveCounter = 0;
	/// <summary>
	/// When the game ends, the visible wave counter drops one by one and gives you points.
	/// We want to keep track of the real counter (i.e. total number of waves) just in case.
	/// </summary>
	private int waveCounterVisible = 0;

	private PauseManager pauseManager;

	private bool IsGameGoing = true;

	void Start()
	{
		destroyedBallsOnLevelEnd = 0;
		sndBallDestroyOnLevelEnd = GetComponent<AudioSource>();

		secondsLeft = seconds;

		GameState.ResetScore();

		pauseManager = FindObjectOfType<PauseManager>();

		Invoke(nameof(SpawnNextWave), 1.5f);

		XMLManager.instance.Load();
	}

	public void SpawnNextWave()
	{
		if (!IsGameGoing)
		{
			return;
		}

		if (PrefabWavesInOrder.Count > 0)
		{
			var obj = PrefabWavesInOrder[waveIndex];
			Instantiate(obj);
			waveIndex = (waveIndex + 1) % (PrefabWavesInOrder.Count);
			waveCounter++;
			waveCounterVisible++;
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
		TimeLeftText.text += $"\nWave {waveCounterVisible}";

		// Tick time.

		if (ShouldTickTimeLeft)
		{
			secondsLeft -= Time.deltaTime;
			if (secondsLeft <= 0 && IsGameGoing)
			{
				TimeIsUp();
			}
		}
	}

	private void TimeIsUp()
	{
		ShouldTickTimeLeft = false;
		IsGameGoing = false;

		DrumRollLoop.clip = SndRefereeWhistle;
		DrumRollLoop.loop = false;
		DrumRollLoop.Play();

		pauseManager.CanPause = false;

		Invoke(nameof(TimeIsUp_01_StopBalls), 0.25f);
	}

	private void TimeIsUp_01_StopBalls()
	{
		foreach(var ball in FindObjectsByType<Ball>(FindObjectsSortMode.None))
		{
			Destroy(ball.GetComponent<Rigidbody>());
		}

		Invoke(nameof(TimeIsUp_02_PlayDrumRoll), 1.473f);
	}

	private void TimeIsUp_02_PlayDrumRoll()
	{
		DrumRollLoop.clip = SndDrumRoll;
		DrumRollLoop.loop = true;
		DrumRollLoop.Play();

		Invoke(nameof(TimeIsUp_03_DestroyRandomBall), 0.4f);
	}

	private void TimeIsUp_03_DestroyRandomBall()
	{
		var balls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID).ToList();
		balls.Sort((Ball b1, Ball b2) => { return b1.transform.position.z.CompareTo(b2.transform.position.z); });
		balls.Reverse();
		if (balls.Count == 0)
		{
			Invoke(nameof(TimeIsUp_04_AllBallsDestroyed), 0.1f);
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
			sndBallDestroyOnLevelEnd.pitch += 0.1f;

			Invoke(nameof(TimeIsUp_03_DestroyRandomBall), 0.25f);
		}
	}

	private void TimeIsUp_04_AllBallsDestroyed()
	{
		Invoke(nameof(TimeIsUp_05_GetPointsForClearedWave), 0.15f);
	}

	private void TimeIsUp_05_GetPointsForClearedWave()
	{
		if (waveCounterVisible > 0)
		{
			GameState.AddScore(1000);

			sndBallDestroyOnLevelEnd.Play();
			sndBallDestroyOnLevelEnd.pitch += 0.1f;

			waveCounterVisible--;

			Invoke(nameof(TimeIsUp_05_GetPointsForClearedWave), 0.25f);
		}
		else
		{
			Invoke(nameof(TimeIsUp_06_DrumRollEnd), 0.67f);
		}
	}

	private void TimeIsUp_06_DrumRollEnd()
	{
		DrumRollLoop.Stop();
		DrumRollLoop.clip = SndDrumRollEnd;
		DrumRollLoop.loop = false;
		DrumRollLoop.Play();

		Invoke(nameof(TimeIsUp_07_ShowResults), 2.5f);
	}

	private void TimeIsUp_07_ShowResults()
	{
		GameState.TotalWaves = waveCounter;

		XMLManager.instance.AddScore(new(GameState.Score, GameState.TotalWaves, GameState.BestCombo));
		XMLManager.instance.Save();

		pauseManager.ShowResultsScreen();
	}
}
