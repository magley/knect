using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static KinectGestures;

public class PauseManager : MonoBehaviour, GestureListenerInterface
{
	private Texture2D screenshot;
	public Image pauseImage;
	bool isPaused = false;
	private List<Pauseable> wasInactiveBeforePausing = new List<Pauseable>();

	[SerializeField] private Camera cameraMain;
	[SerializeField] private Camera cameraPause;
	[SerializeField] private GameObject uiTutorial;

	[SerializeField] private AudioClip sndPause;
	[SerializeField] private AudioClip sndUnpause;

	[SerializeField] private GameObject objectTreePause;
	[SerializeField] private GameObject objectTreeResults;

	[SerializeField] private GameObject alwaysActive;

	public bool CanPause = true;
	private List<CanvasGroup> uiObjectsToHandleOpacity = new List<CanvasGroup>();
	private AudioSource objMusic;

	private void Start()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}
		uiTutorial.SetActive(true);

		objMusic = FindObjectsOfType<AudioSource>().Where(o => o.CompareTag("Music")).First();

		uiObjectsToHandleOpacity = transform
			.GetComponentsInChildren<CanvasGroup>(true)
			.Where(o => o.gameObject.CompareTag("GameUiTop"))
			.ToList();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && CanPause)
		{
			StartCoroutine(PauseOnEndOfFrame());
		}

		HandleUIOpacity();
	}

	private void HandleUIOpacity()
	{
		float uiOpacity = 0;
		if (isPaused)
		{
			uiOpacity = 1;
		}

		foreach (var canvasGroup in uiObjectsToHandleOpacity)
		{
			canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, uiOpacity, Time.deltaTime * 8);
		}
		uiTutorial.GetComponentInChildren<CanvasGroup>().alpha = 1.0f;
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
	}

	void TogglePause()
	{
		if (CanPause)
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
	}

	public void PauseGame()
	{
		isPaused = true;
		cameraPause.enabled = true;
		cameraMain.enabled = false;

		wasInactiveBeforePausing = FindObjectsOfType<Pauseable>(true).Where(p => !p.gameObject.activeInHierarchy).ToList();
		foreach (var pauseable in FindObjectsOfType<Pauseable>(true))
		{
			pauseable.gameObject.SetActive(false);
		}

		CaptureScreenshot();
		ShowScreenshot();

		objectTreePause.SetActive(true);
		alwaysActive.SetActive(true);
		for (int i = 0; i < objectTreePause.transform.childCount; i++)
		{
			objectTreePause.transform.GetChild(i).gameObject.SetActive(true);
		}

		AudioSource.PlayClipAtPoint(sndPause, cameraMain.transform.position);

		objMusic.volume = 0.4f;
	}

	public void ResumeGame()
	{
		isPaused = false;
		cameraMain.enabled = true;
		cameraPause.enabled = false;

		foreach (var pauseable in FindObjectsOfType<Pauseable>(true).Except(wasInactiveBeforePausing))
		{
			pauseable.gameObject.SetActive(true);
		}
		wasInactiveBeforePausing.Clear();

		objectTreePause.SetActive(false);
		for (int i = 0; i < objectTreePause.transform.childCount; i++)
		{
			objectTreePause.transform.GetChild(i).gameObject.SetActive(false);
		}

		AudioSource.PlayClipAtPoint(sndUnpause, cameraMain.transform.position);

		objMusic.volume = 1.0f;
	}

	public void ShowResultsScreen()
	{
		StartCoroutine(ShowResultsScreenOnEndOfFrame());
	}

	IEnumerator ShowResultsScreenOnEndOfFrame()
	{
		yield return new WaitForEndOfFrame();
		CaptureScreenshot();
		ShowScreenshot();

		wasInactiveBeforePausing = FindObjectsOfType<Pauseable>(true).Where(p => !p.gameObject.activeInHierarchy).ToList();
		foreach (var pauseable in FindObjectsOfType<Pauseable>(true))
		{
			pauseable.gameObject.SetActive(false);
		}

		CanPause = false;
		isPaused = true;
		cameraPause.enabled = true;
		cameraMain.enabled = false;

		alwaysActive.SetActive(true);

		objectTreePause.SetActive(false);
		for (int i = 0; i < objectTreePause.transform.childCount; i++)
		{
			objectTreePause.transform.GetChild(i).gameObject.SetActive(false);
		}

		objectTreeResults.SetActive(true);
		for (int i = 0; i < objectTreeResults.transform.childCount; i++)
		{
			objectTreeResults.transform.GetChild(i).gameObject.SetActive(true);
		}

		if (objectTreeResults.transform.GetComponentInChildren<ResultsUIManager>() is ResultsUIManager resultsUI)
		{
			resultsUI.Ready();
		}
	}

	public void UserDetected(uint userId, int userIndex)
	{
		KinectManager manager = KinectManager.Instance;
		manager.DetectGesture(userId, Gestures.RaiseRightHand);
	}

	public void UserLost(uint userId, int userIndex)
	{
		// Do nothing.
	}

	public void GestureInProgress(uint userId, int userIndex, Gestures gesture, float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		// Do nothing.
	}

	public bool GestureCompleted(uint userId, int userIndex, Gestures gesture, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		if (gesture == Gestures.RaiseRightHand)
		{
			if (!isPaused)  // Pause, but don't unpause.
			{
				TogglePause();
			}
		}

		return true;
	}

	public bool GestureCancelled(uint userId, int userIndex, Gestures gesture, KinectWrapper.NuiSkeletonPositionIndex joint)
	{
		return true;
	}
}
