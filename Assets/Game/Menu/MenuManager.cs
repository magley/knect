using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[SerializeField] private Transform backgroundObject;
	private float backgroundScale = 1f;

    [SerializeField] int focusIndex = -1;
    List<MenuItem> menuItems = new List<MenuItem>();
	private void Start()
	{
		menuItems = GameObject.FindObjectsOfType<MenuItem>().ToList();
        focusIndex = 0;
		menuItems[focusIndex].DoFocus();
	}

	void Update()
    {
		HandleBackground();

		menuItems = GameObject.FindObjectsOfType<MenuItem>().ToList();
		int inputY = (Input.GetKeyDown(KeyCode.DownArrow) ? 1 : 0) - (Input.GetKeyDown(KeyCode.UpArrow) ? 1 : 0);


		if (inputY == 1)
        {
			menuItems[focusIndex].DoUnfocus();
			focusIndex++;
			if (focusIndex > menuItems.Count - 1)
			{
				focusIndex = 0;
			}
			menuItems[focusIndex].DoFocus();
		}
        if (inputY == -1)
        {
			menuItems[focusIndex].DoUnfocus();
			focusIndex--;
			if (focusIndex < 0)
			{
				focusIndex = menuItems.Count - 1;
			}
			menuItems[focusIndex].DoFocus();
		}
    }

	private void HandleBackground()
	{
		backgroundObject.Rotate(new Vector3(0, 0, 0.1f));
		backgroundScale = Mathf.Sin(Time.time * 0.25f) * 0.5f + 1f;
		backgroundObject.localScale = new Vector3(1, 1, 0) * backgroundScale + Vector3.forward;
	}
}
