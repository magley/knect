using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuItem : MonoBehaviour
{
    public enum MenuItemType
    {
        Unknown,
        PlayBreakout,
        PlaySwitches,
        Quit,
    }

    public MenuItemType Type;
    [SerializeField] private bool CanBeSelected = true;
    private bool Focused = false;
    private bool Selected = false;
    private bool HasFocusFromKinect = false;
    private float TimeUntilSelectFromKinect = 120;

    private const float scaleFocus = 2f;
    private const float scaleUnfocus = 1.5f;
    private float scale = 1.5f;


    [SerializeField] private AudioClip sndFocus;
    [SerializeField] private AudioClip sndSelect;
    private AudioSource audioSource;

    void Start()
    {
		audioSource = GetComponent<AudioSource>();
	}

    void Update()
    {
		HandleScale();
        HandleInteractionThroughKinect();
        HandleInteractionThroughKeyboard();
	}

    private void HandleScale()
    {
        if (!Selected)
        {
		    if (Focused)
		    {
			    scale = Mathf.Lerp(scale, scaleFocus, 0.2f);
            
		    }
		    else
		    {			
                scale = Mathf.Lerp(scale, scaleUnfocus, 0.2f);
		    }
        }
        else
        {
			scale = Mathf.Lerp(scale, scaleFocus, 0.2f);

            if (Mathf.Abs(scale - scaleFocus) <= 0.01f)
            {
                OnSelect();
            }
		}

		transform.localScale = Vector3.one * scale;
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
        if (!Focused)
        {
            Focused = true;
            audioSource.clip = sndFocus;
            audioSource.Play();
        }
	}

    public void DoUnfocus()
    {
        if (Focused)
        {
            Focused = false;
        }
    }

	public void DoSelect()
	{
        if (!CanBeSelected)
        {
            return;
        }

        if (!Selected)
        {
            scale = 3;
			Selected = true;
			audioSource.clip = sndSelect;
			audioSource.Play();
		}
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
                    SceneManager.LoadScene("BreakoutScene");
                }
                break;
            case MenuItemType.PlaySwitches: break;
        }
    }
}
