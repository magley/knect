using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
	private Texture2D screenshot;
	public Image pauseImage;
	bool isPaused = false;
	private List<Pauseable> wasInactiveBeforePausing = new List<Pauseable>();

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			StartCoroutine(PauseOnEndOfFrame());
		}
	}

	IEnumerator PauseOnEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		TogglePause();
	}


	void CaptureScreenshot()
	{
		screenshot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
		screenshot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
		screenshot.Apply();
	}

	private void ShowScreenshot()
	{
		Sprite sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
		pauseImage.sprite = sprite;
		pauseImage.gameObject.SetActive(true);
	}

	void TogglePause()
	{
		isPaused = !isPaused;
		if (isPaused)
		{
			PauseGame();
		}
		else
		{
			ResumeGame();
		}
	}

	private void PauseGame()
	{
		wasInactiveBeforePausing = FindObjectsOfType<Pauseable>(true).Where(p => !p.gameObject.activeInHierarchy).ToList();
		foreach (var pauseable in FindObjectsOfType<Pauseable>(true))
		{
			pauseable.gameObject.SetActive(false);
		}

		CaptureScreenshot();
		ShowScreenshot();
	}

	private void ResumeGame()
	{
		foreach (var pauseable in FindObjectsOfType<Pauseable>(true).Except(wasInactiveBeforePausing))
		{
			pauseable.gameObject.SetActive(true);
		}
		wasInactiveBeforePausing.Clear();

		pauseImage.gameObject.SetActive(false);
	}
}
