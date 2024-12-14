using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static event Action<string> OnActiveMenuChanged;
	[SerializeField] private List<Transform> textKinectManagerNotFound = new List<Transform>();
	[SerializeField] private Transform backgroundObject;

	private float backgroundScale = 1f;
	private SpriteRenderer visibleBackgroundImage;

	private void Start()
	{
		MenuManager.OnSetBackgroundImage += OnSetBackgroundImage;
	}

	private void OnSetBackgroundImage(SpriteRenderer image)
	{
		visibleBackgroundImage = image;
	}

	void Update()
    {
		HandleBackground();
	}

	private void HandleBackground()
	{
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
