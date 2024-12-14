using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static event Action<string> OnActiveMenuChanged;

	[SerializeField] private List<Transform> textKinectManagerNotFound = new List<Transform>();

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

	[SerializeField] private Transform backgroundObject;
	private float backgroundScale = 1f;

	void Start()
    {
        
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
	}
}
