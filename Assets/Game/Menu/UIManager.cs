using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static event Action<string> OnActiveMenuChanged;
	[SerializeField] private List<Transform> textKinectManagerNotFound = new List<Transform>();
	[SerializeField] private Transform backgroundObject;
	[SerializeField] private SpriteMask transitionCircleMask;


	private float backgroundScale = 1f;
	private SpriteRenderer visibleBackgroundImage;

	[SerializeField] private bool StartWithTransitionCircleClosed = true;
	private float transitionCircleMaskOpenSize;
	[SerializeField] private bool isTransitioning = false;
	private Action callableAfterTransition;
	private float callableAfterTransitionHowManySeconds;
	public void TransitionAndThen(Action callableFunction, float afterHowManySeconds)
	{
		isTransitioning = true;
		callableAfterTransitionHowManySeconds = afterHowManySeconds;
		callableAfterTransition = callableFunction;
	}

	private void Start()
	{
		MenuManager.OnSetBackgroundImage += OnSetBackgroundImage;

		transitionCircleMaskOpenSize = transitionCircleMask.transform.localScale.x;
		if (StartWithTransitionCircleClosed)
		{
			transitionCircleMask.transform.localScale = Vector3.zero;
		}
	}

	private void OnDestroy()
	{
		MenuManager.OnSetBackgroundImage -= OnSetBackgroundImage;
	}

	private void OnSetBackgroundImage(SpriteRenderer image)
	{
		visibleBackgroundImage = image;
	}

	void Update()
    {
		HandleBackground();
		HandleTransition();
	}

	private void HandleTransition()
	{
		if (isTransitioning)
		{
			if (transitionCircleMask.transform.localScale.x > 0f)
			{
				transitionCircleMask.transform.localScale -= Vector3.one * 0.135f;
				if (transitionCircleMask.transform.localScale.x <= 0.025f)
				{
					transitionCircleMask.transform.localScale *= 0;
					Invoke(nameof(InvokeCallableAfterTransition), callableAfterTransitionHowManySeconds);
				}
			}
		}
		else
		{
			if (transitionCircleMask.transform.localScale.x < transitionCircleMaskOpenSize)
			{
				transitionCircleMask.transform.localScale += Vector3.one * 0.135f;
			}
		}
	}

	private void InvokeCallableAfterTransition()
	{
		callableAfterTransition();
	}

	private void HandleBackground()
	{
		if (backgroundObject == null)
		{
			return;
		}

		backgroundObject.Rotate(new Vector3(0, 0, 0.1f));
		backgroundScale = Mathf.Sin(Time.time * 0.25f) * 0.5f + 1f;
		backgroundObject.localScale = new Vector3(1, 1, 0) * backgroundScale + Vector3.forward;

		if (visibleBackgroundImage != null)
		{
			foreach (var img in backgroundObject.GetComponentsInChildren<SpriteRenderer>())
			{
				var c = img.color;

				if (img == visibleBackgroundImage)
				{
					c.a = Mathf.Lerp(c.a, 1, 0.2f);
				}
				else
				{
					c.a = Mathf.Lerp(c.a, 0, 0.2f);
				}

				img.color = c;
			}
		}
	}

	public static void SetActiveMenu(string nameID)
	{
		OnActiveMenuChanged.Invoke(nameID);
	}

	private void SetKinectNotConnectedText()
	{
		foreach (var t in textKinectManagerNotFound)
		{
			if (t.GetComponent<Text>() is Text tt)
			{
				if (KinectManager.Instance == null)
				{
					tt.text = "Kinect not connected!";
				}
				else
				{
					tt.text = "";
				}
			}
		}
	}
}
