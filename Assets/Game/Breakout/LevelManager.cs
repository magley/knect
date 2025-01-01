using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

class LevelStartBonus
{
	public string Description;
	public int HighScoreThreshold;
	public Action<LevelManager> Function;

	public LevelStartBonus(string description, int highScoreThreshold, Action<LevelManager> function)
	{
		this.Description = description;
		this.HighScoreThreshold = highScoreThreshold;
		this.Function = function;

	}

	public static List<LevelStartBonus> Bonuses = new()
	{
		new LevelStartBonus("90 seconds", 100_000, (LevelManager self) =>
		{
			self.SetStartingTime(90);
		}),
		new LevelStartBonus("Start with x2", 200_000, (LevelManager self) =>
		{
			PlayerAdditions.SetScoreMultiplier(2);
		}),
		new LevelStartBonus("120 seconds", 300_000, (LevelManager self) =>
		{
			self.SetStartingTime(120);
		}),
		new LevelStartBonus("2 balls", 500_000, (LevelManager self) =>
		{
			self.AddBall();
		}),
		new LevelStartBonus("Start with x5", 800_000, (LevelManager self) =>
		{
			PlayerAdditions.SetScoreMultiplier(5);
		}),
		new LevelStartBonus("Force x5", 1_000_000, (LevelManager self) =>
		{
			PlayerAdditions.ForceScoreMultiplier5 = true;
		}),
		new LevelStartBonus("3 balls", 1_500_000, (LevelManager self) =>
		{
			self.AddBall();
		}),
		new LevelStartBonus("Unlock x10", 2_250_000, (LevelManager self) =>
		{
			PlayerAdditions.UnlockScoreMultiplier10 = true;
		}),
	};
}

public class LevelManager : MonoBehaviour
{
	private int destroyedBallsOnLevelEnd = 0;
	[SerializeField] private GameObject PrefabWorldSpaceTextForScore;
	private AudioSource audioSource;

	private bool ShouldTickTimeLeft = false;
	float seconds = 60;
	[SerializeField] private Text TimeLeftText;
	private float secondsLeft = 60;
	[SerializeField] private AudioSource DrumRollLoop;
	[SerializeField] private AudioClip SndRefereeWhistle;
	[SerializeField] private AudioClip SndDrumRoll;
	[SerializeField] private AudioClip SndDrumRollEnd;

	[SerializeField] private AudioClip sndLevelEndBonusBall;
	[SerializeField] private GameObject PrefabBall;

	[SerializeField] private List<GameObject> PrefabWavesInOrder = new List<GameObject>();
	private int waveIndex = 0;
	private int waveCounter = 0;
	/// <summary>
	/// When the game ends, the visible wave counter drops one by one and gives you points.
	/// We want to keep track of the real counter (i.e. total number of waves) just in case.
	/// </summary>
	private int waveCounterVisible = 0;

	private PauseManager pauseManager;

	[SerializeField] private Text textCountdown;
	private float textCountdownScaleStart = 4;
	private float textCountdownScaleEnd = 1;
	private float textCountdownTimeUntilNext = 1f;
	private int textCountdownCounter = 3;
	[SerializeField] private AudioClip sndCountdown;
	[SerializeField] private AudioClip sndCountdownEnd;

	[SerializeField] private Text textFinished;
	private float textFinishedScaleStart = 4;
	private float textFinishedScaleMid = 0.5f;
	private float textFinishedScaleEnd = 0.3f;
	private int textFinishedScaleState = 0;
	private bool textFinishedScaleRunning = false;

	private AudioSource objMusic;
	[SerializeField] private AudioClip mus;
	[SerializeField] private AudioClip musHurryUp;

	[SerializeField] private GameObject PlayerKeyboard;
	[SerializeField] private GameObject PlayerKinect;

	private void HandleTextFinishedSize()
	{
		if (!textFinishedScaleRunning)
		{
			return;
		}
		float textFinishedScale = textFinished.transform.localScale.x;
		
		float textFinishedScaleChangeSpeed = 0.5f;
		if (textFinishedScale <= textFinishedScaleMid)
		{
			textFinishedScaleChangeSpeed = 0.01f;

			if (textFinishedScaleState == 0)
			{
				textFinishedScale = textFinishedScaleMid;
				textFinishedScaleState++;
			}
		}
		if (textFinishedScale <= textFinishedScaleEnd)
		{
			textFinishedScaleChangeSpeed = 0.1f;

			if (textFinishedScaleState == 1)
			{
				textFinishedScale = textFinishedScaleEnd;
				textFinishedScaleState++;
			}
		}

		textFinishedScale -= textFinishedScaleChangeSpeed * Time.deltaTime * 15;
		if (textFinishedScale < 0)
		{
			textFinishedScale = 0;
			textFinishedScaleRunning = false;
		}
		textFinished.transform.localScale = Vector3.one * textFinishedScale;
	}

	public void TextCountdownStart()
	{
		Invoke(nameof(TextCountdownTick), 2);
		ShouldTickTimeLeft = false;
	}

	private void TextCountdownTick()
	{
		if (textCountdownCounter > 0)
		{
			textCountdown.text = $"{textCountdownCounter}";
			textCountdown.transform.localScale = Vector3.one * textCountdownScaleStart;
			textCountdownCounter--;

			Invoke(nameof(TextCountdownTick), textCountdownTimeUntilNext);

			audioSource.clip = sndCountdown;
			audioSource.Play();
			audioSource.loop = false;
			audioSource.pitch = 1;
		}
		else
		{
			textCountdown.text = "GO!";
			textCountdown.transform.localScale = Vector3.one * textCountdownScaleStart;

			Invoke(nameof(TextCountdownFinish), textCountdownTimeUntilNext);

			audioSource.clip = sndCountdownEnd;
			audioSource.Play();
			audioSource.loop = false;
			audioSource.pitch = 1f;
		}
	}

	private void TextCountdownFinish()
	{
		foreach (var item in FindObjectsByType<Ball>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			item.gameObject.SetActive(true);
		}

		textCountdown.text = "";

		Invoke(nameof(SpawnNextWave), 0.25f);
		ShouldTickTimeLeft = true;
		ApplyBonuses();
	}

	private void HandleTextCountdown()
	{
		if (textCountdown.transform.localScale.x > textCountdownScaleEnd)
		{
			textCountdown.transform.localScale -= Vector3.one * 0.5f * Time.deltaTime * 30;

			if (textCountdown.transform.localScale.x < textCountdownScaleEnd)
			{
				textCountdown.transform.localScale = Vector3.one * textCountdownScaleEnd;
			}
		}
	}

	public bool IsGameGoing = true;

	internal void SetStartingTime(int newSeconds)
	{
		seconds = newSeconds;
		secondsLeft = seconds;
	}

	internal void AddBall()
	{
		Vector3 offset = new(
			UnityEngine.Random.Range(-0.25f, 0.25f),
			UnityEngine.Random.Range(-0.25f, 0.25f),
			UnityEngine.Random.Range(-0.25f, 0.25f)
		);
		var ball = Instantiate(PrefabBall, FindFirstObjectByType<Ball>().transform.position + offset, UnityEngine.Random.rotation);
	}

	void Start()
	{
		destroyedBallsOnLevelEnd = 0;
		audioSource = GetComponent<AudioSource>();
		pauseManager = FindObjectOfType<PauseManager>();

		if (GameState.PlayingAsKinect)
		{
			PlayerKeyboard.SetActive(false);
			PlayerKinect.SetActive(true);
		}
		else
		{
			PlayerKeyboard.SetActive(true);
			PlayerKinect.SetActive(false);
		}

		GameState.ResetScore();
		XMLManager.instance.Load();


		secondsLeft = seconds;

		foreach (var item in FindObjectsByType<Ball>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			item.gameObject.SetActive(false);
		}

		// TextCountdownStart(); <-- Called by IntroPanCameraController.

		objMusic = FindObjectsOfType<AudioSource>().Where(o => o.CompareTag("Music")).First();
	}

	private void ApplyBonuses()
	{

		foreach (var bonus in LevelStartBonus.Bonuses)
		{
			if (bonus.HighScoreThreshold > XMLManager.instance.data.GetHighScore())
			{
				Debug.Log($"Need {bonus.HighScoreThreshold}, have {XMLManager.instance.data.GetHighScore()}");
				break;
			}
			Debug.Log($"Applying {bonus.Description}");
			bonus.Function.Invoke(this);
		}
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
		HandleTextCountdown();
		HandleTextFinishedSize();
	}

	private void UpdateTimeLeft()
	{
		// Display time left.

		int minutesShow = (int)(secondsLeft / 60);
		int secondsShow = (int)(secondsLeft % 60);
		TimeLeftText.text = $"{minutesShow.ToString("D2")}:{secondsShow.ToString("D2")}";
		TimeLeftText.text += $"\nWave {waveCounterVisible}";

		// Tick time.

		if (ShouldTickTimeLeft)
		{
			secondsLeft -= Time.deltaTime;
			if (secondsLeft <= 0 && IsGameGoing)
			{
				TimeIsUp();
			}

			// Hurry up music.

			if (secondsLeft <= seconds / 6)
			{
				if (objMusic.clip != musHurryUp)
				{
					objMusic.clip = musHurryUp;
					objMusic.Play();
				}
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

		textFinishedScaleRunning = true;
		textFinished.transform.localScale = Vector3.one * textFinishedScaleStart;

		pauseManager.CanPause = false;

		Invoke(nameof(TimeIsUp_01_StopBalls), 0.25f);

		audioSource.clip = sndLevelEndBonusBall;

		objMusic.Stop();
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

			audioSource.Play();
			audioSource.pitch += 0.1f;

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

			audioSource.Play();
			audioSource.pitch += 0.1f;

			waveCounterVisible--;

			Invoke(nameof(TimeIsUp_05_GetPointsForClearedWave), 0.25f);
		}
		else
		{
			Invoke(nameof(TimeIsUp_06_DrumRollEnd), 0.67f);
			ConfettiBlower.StartSequenceEmit();
		}
	}

	private void TimeIsUp_06_DrumRollEnd()
	{
		DrumRollLoop.Stop();
		DrumRollLoop.clip = SndDrumRollEnd;
		DrumRollLoop.loop = false;
		DrumRollLoop.Play();

		Invoke(nameof(TimeIsUp_07_ShowResults), 4.5f);
		AudienceController.CheerEndlessly();
	}

	private void TimeIsUp_07_ShowResults()
	{
		GameState.TotalWaves = waveCounter;

		XMLManager.instance.AddScore(new(GameState.Score, GameState.TotalWaves, GameState.BestCombo));
		XMLManager.instance.Save();

		pauseManager.ShowResultsScreen();

		objMusic.clip = mus;
		objMusic.Play();
	}
}
