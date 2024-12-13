using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuItem : MonoBehaviour
{
    public enum MenuItemType
    {
        Unknown,
        PlayBreakout,
        Quit,
    }

    public MenuItemType Type;
    [SerializeField] private bool Focused = false;
    private bool Selected = false;
    private bool HasFocusFromKinect = false;
    private float TimeUntilSelectFromKinect = 120;

    private const float scaleFocus = 2f;
    private const float scaleUnfocus = 1f;
    private float scale = 1f;

    void Start()
    {

    }

    void Update()
    {
        HandleVisualsWhenFocused();
        HandleInteractionThroughKinect();
        HandleInteractionThroughKeyboard();
	}

    private void HandleVisualsWhenFocused()
    {
		if (Focused)
		{
			scale = Mathf.Lerp(scale, scaleFocus, 0.2f);
            transform.localScale = Vector3.one * scale;
		}
		else
		{			
            scale = Mathf.Lerp(scale, scaleUnfocus, 0.2f);
			transform.localScale = Vector3.one * scale;
		}
	}

    private void HandleInteractionThroughKeyboard()
    {
		if (Selected)
		{
			return;
		}

        if (Focused)
        {
		    if (Input.GetKeyDown(KeyCode.Return))
            {
                DoSelect();
            }
        }
    }

    private void HandleInteractionThroughKinect()
    {
        if (Selected)
        {
            return;
        }

        if (HasFocusFromKinect)
        {
            TimeUntilSelectFromKinect -= 1;

            if (TimeUntilSelectFromKinect <= 0)
            {
                DoSelect();
            }
        }
        else
        {
            if (TimeUntilSelectFromKinect < 120)
            {
                TimeUntilSelectFromKinect += 5;
            }
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("KinectPointer"))
        {
			if (!HasFocusFromKinect)
            {
                DoFocus();
            }
			HasFocusFromKinect = true;
        }
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("KinectPointer"))
        {
            if (HasFocusFromKinect)
            {
                DoUnfocus();
			}
            HasFocusFromKinect = false;
        }
	}

	public void DoFocus()
    {
        Focused = true;
    }

    public void DoUnfocus()
    {
        Focused = false;
    }

	public void DoSelect()
	{
        Selected = true;

        OnSelect();
	}

	private void OnSelect()
    {
        switch (Type)
        {
            case MenuItemType.Unknown: default: break;
            case MenuItemType.Quit:
                { 
#if UNITY_EDITOR
				    UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
				}
				break;
            case MenuItemType.PlayBreakout:
                {
                    Debug.Log("Breakout!");
                }
                break;
        }
    }
}
