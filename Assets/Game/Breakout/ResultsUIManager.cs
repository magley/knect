using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUIManager : MonoBehaviour
{
    [SerializeField] private Text textScoreBoard;

    private int animScore = 0;
    private int animWaves = 0;
    private int animCombo = 0;
    private bool animRunning = false;

    private AudioSource audioSource;
    [SerializeField] private AudioClip sndBlip;
	[SerializeField] private AudioClip sndBloop;

    private void SetScoreBoardText(int score, int waves, int bestCombo)
    {
        /*
        Score: 00000000
        New High Score!
        _____________________

        Waves Cleared: 00
        _____________________

        Best Combo: 00
        */

        string s = "";
        bool isNewHighScore = true;
        s += $"Score:       {score.ToString().PadLeft(8, '0')}\n";
        s += isNewHighScore ? $"New High Score!\n" : "\n";
        s += "_____________________\n\n";
        s += $"Waves Cleared: {waves,0}\n";
        s += "_____________________\n\n";
        s += $"Best Combo: {bestCombo,0}\n";

        textScoreBoard.text = s;
	}

	private void Start()
	{
        audioSource = GetComponent<AudioSource>();
        SetScoreBoardText(0, 0, 0);
	}

	void Update()
    {
        if (animRunning)
        {
            HandleAnim();
        }
    }

    private void HandleAnim()
    {
        if (animScore < GameState.Score)
        {
            MaybeStartPlayingBlip();

			int diff = GameState.Score - animScore;
            if (diff > 10000)
            {
				animScore += 1000;
            }
			if (diff > 5000)
			{
				animScore += 500;
			}
			else
            {
                animScore += 100;
			}

            if (animScore >= GameState.Score)
            {
				StopAnim();
				Invoke(nameof(StartAnim), 1f);
				animScore = GameState.Score;
            }
        }
        else if (animWaves < GameState.TotalWaves)
        {
            MaybeStartPlayingBlip();

			animWaves++;

			if (animWaves >= GameState.TotalWaves)
			{
				StopAnim();
				Invoke(nameof(StartAnim), 1f);
				animWaves = GameState.TotalWaves;
			}
		}
        else if (animCombo < GameState.BestCombo)
        {
            MaybeStartPlayingBlip();

			animCombo++;

			if (animCombo >= GameState.BestCombo)
			{
                StopAnim();
				Invoke(nameof(StartAnim), 1f);
				animCombo = GameState.BestCombo;
			}
		}
        else
        {
			audioSource.Stop();
			animRunning = false;
		}

		SetScoreBoardText(animScore, animWaves, animCombo);
	}

    private void StartAnim()
    {
		animRunning = true;
    }

    private void MaybeStartPlayingBlip()
    {
		if (!audioSource.isPlaying)
		{
			audioSource.clip = sndBlip;
			audioSource.loop = true;
			audioSource.Play();
		}

	}

	private void StopAnim()
    {
		audioSource.Stop();
		audioSource.clip = sndBloop;
		audioSource.loop = false;
		audioSource.Play();
		animRunning = false;
	}

	public void Ready()
	{
        Invoke(nameof(StartAnim), 1f);
	}
}
