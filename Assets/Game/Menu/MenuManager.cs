using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] int focusIndex = 0;
    private List<MenuItem> menuItems = new List<MenuItem>();

	[SerializeField] private string NameID = "";
	[SerializeField] public bool IsActive = false;

	private void Start()
	{
		menuItems = GetComponentsInChildren<MenuItem>(includeInactive:true).ToList();
		UIManager.OnActiveMenuChanged += OnActiveMenuChanged;

		focusIndex = 0;
		menuItems[focusIndex].DoFocus();
	}

	private void OnActiveMenuChanged(string nameID)
	{
		IsActive = (NameID == nameID);

		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(IsActive);
		}
	}

	void Update()
    {
		if (IsActive)
		{
			HandleInput();
		}
	}

	private void HandleInput()
	{
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

		bool enterPressed = Input.GetKeyDown(KeyCode.Return);

		if (enterPressed)
		{
			foreach (var item in menuItems)
			{
				if (item.Focused)
				{
					item.DoSelect();
				}
			}
		}
	}
}
