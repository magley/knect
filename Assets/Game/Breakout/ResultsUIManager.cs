using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ResultsUIManager : MonoBehaviour
{
	private AudioSource AudioSource;

	[SerializeField] private AudioClip SndBlip;
	[SerializeField] private AudioClip SndBloop;
	[SerializeField] private AudioClip SndSuccess;

	private float TimeUntilBeforeAllIsHeld = 2f;
	private bool BeforeAllIsUpdating = false;

	[SerializeField] private Text Score;
	[SerializeField] private Text NewHighScore;
	private int DisplayScore = 0;
	private float TimeUntilScoreIsHeld = 1f;
	private bool ScoreIsUpdating = false;

	[SerializeField] private Text WavesCleared;
	private int DisplayWavesCleared = 0;
	private float TimeUntilWaveCounterTicks = 0.175f;
	private float TimeUntilWavesClearedIsHeld = 1f;
	private bool WavesClearedIsUpdating = false;

	private void Start()
	{
		AudioSource = GetComponent<AudioSource>();
	}

	private void Update()
	{
		HandleScore();
		HandleBeforeAll();
		HandleWavesCleared();
	}

	public void Ready()
	{
		BeforeAllIsUpdating = true;
	}

	private void HandleBeforeAll()
	{
		if (!BeforeAllIsUpdating)
		{
			return;
		}

		if (UpdateTimer(ref TimeUntilBeforeAllIsHeld).Item1)
		{
			BeforeAllIsUpdating = false;
			ScoreIsUpdating = true;
		}
	}

	private void HandleScore()
	{
		if (!ScoreIsUpdating)
		{
			return;
		}

		if (DisplayScore < GameState.Score)
		{
			LoopSoundBlip();
			IncrementScore();

			if (DisplayScore == GameState.Score)
			{
				AudioSource.Stop();
				AudioSource.clip = SndBloop;
				AudioSource.loop = false;
				AudioSource.Play();
			}
		}

		Score.text = DisplayScore.ToString().PadLeft(8, '0');

		if (DisplayScore == GameState.Score)
		{
			if (GameState.Score > XMLManager.instance.data.GetHighScore())
			{
				if (!NewHighScore.enabled)
				{
					AudioSource.Stop();
					AudioSource.clip = SndSuccess;
					AudioSource.loop = false;
					AudioSource.Play();

					NewHighScore.enabled = true;
					NewHighScore.text = "New High Score!";
					NewHighScore.rectTransform.localRotation = Quaternion.Euler(new(0, 0, Random.Range(-8, 8)));
					NewHighScore.rectTransform.localScale = Vector3.one * 1.25f;
				} else
				{
					float scale = Mathf.Lerp(NewHighScore.rectTransform.localScale.x, 1f, 11f * Time.deltaTime);
					NewHighScore.rectTransform.localScale = Vector3.one * scale;
				}
			}

			if (UpdateTimer(ref TimeUntilScoreIsHeld).Item1)
			{
				ScoreIsUpdating = false;
				WavesClearedIsUpdating = true;
			}
		}
	}

	private void HandleWavesCleared()
	{
		if (!WavesClearedIsUpdating)
		{
			return;
		}

		if (DisplayWavesCleared < GameState.TotalWaves)
		{
			if (UpdateTimer(ref TimeUntilWaveCounterTicks).Item2)
			{
				TimeUntilWaveCounterTicks = 0.175f;
				DisplayWavesCleared += 1;

				AudioSource.Stop();
				AudioSource.clip = SndBlip;
				AudioSource.loop = false;
				AudioSource.Play();

			}

			if (DisplayWavesCleared == GameState.TotalWaves)
			{
				AudioSource.Stop();
				AudioSource.clip = SndBloop;
				AudioSource.loop = false;
				AudioSource.Play();
			}
		}

		WavesCleared.text = DisplayWavesCleared.ToString().PadLeft(2, '0');

		if (DisplayWavesCleared == GameState.TotalWaves)
		{
			if (UpdateTimer(ref TimeUntilWavesClearedIsHeld).Item1)
			{
				WavesClearedIsUpdating = false;
			}
		}
	}

	private void IncrementScore()
	{
		DisplayScore = (int)Mathf.Lerp(DisplayScore, GameState.Score, 3 * Time.deltaTime);
		if (Mathf.Abs(DisplayScore - GameState.Score) < 100)
		{
			DisplayScore = GameState.Score;
		}
	}

	private void LoopSoundBlip()
	{
		if (!AudioSource.isPlaying)
		{
			AudioSource.clip = SndBlip;
			AudioSource.loop = true;
			AudioSource.Play();
		}
	}

	/// <returns>(timer ended, timer just ended)</returns>
	private (bool, bool) UpdateTimer(ref float timer)
	{
		if (timer > 0)
		{
			timer -= Time.deltaTime;

			if (timer <= 0)
			{
				return (true, true);
			}
		}
		else
		{
			return (true, false);
		}

		return (false, false);
	}
}