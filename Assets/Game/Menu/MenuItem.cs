using System.Runtime.CompilerServices;
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

		OpenMenu_Main,
		OpenMenu_Breakout,
		OpenMenu_Switches,
		OpenMenu_Quit,
	}

    public MenuItemType Type;
    [SerializeField] private bool CanBeSelected = true;
    public bool Focused { get; private set; } = false;
    private bool Selected = false;
    private bool HasFocusFromKinect = false;
    private float TimeUntilSelectFromKinect = 120;

    private const float scaleFocus = 1f;
    private const float scaleUnfocus = 0.65f;
    private float scale = 1f;

    [SerializeField] private AudioClip sndFocus;
    [SerializeField] private AudioClip sndSelect;
    private AudioSource audioSource;

    private MenuManager parentMenu; // Set in MenuManager::Start().
    public bool IsActive { get => parentMenu.IsActive; }

    private Vector3 baseScale;

    void Start()
    {
		audioSource = GetComponent<AudioSource>();
        parentMenu = GetComponentInParent<MenuManager>();
        baseScale = transform.localScale;
	}

    void Update()
    {
		HandleScale();

        if (IsActive)
        {
            HandleInteractionThroughKinect();
        }
        else
        {
            if (HasFocusFromKinect)
            {
				LoseFocusFromKinect();
			}
		}
	}

    /// <summary>
    /// Animate the menu item scaling up/down when it's focused or selected.
    /// </summary>
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

		transform.localScale = baseScale * scale;
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
		if (other.CompareTag("KinectPointer") && IsActive)
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
        if (!IsActive)
        {
            return;
        }

		if (other.CompareTag("KinectPointer") && IsActive)
        {
            LoseFocusFromKinect();
		}
	}

    private void LoseFocusFromKinect()
    {
		if (HasFocusFromKinect)
		{
			DoUnfocus();
		}
		HasFocusFromKinect = false;
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
            scale = 2f;
			Selected = true;
			audioSource.clip = sndSelect;
			audioSource.Play();
		}
	}

	private void OnSelect()
    {
        Selected = false;
		transform.localScale = baseScale;

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
                    SceneManager.LoadScene("BreakoutScene");
                }
                break;
            case MenuItemType.PlaySwitches: break;
            case MenuItemType.OpenMenu_Main:
                {
                    UIManager.SetActiveMenu("Menu_Main");
                }
                break;
			case MenuItemType.OpenMenu_Breakout:
				{
					UIManager.SetActiveMenu("Menu_Breakout");
				}
				break;
			case MenuItemType.OpenMenu_Quit:
				{
					UIManager.SetActiveMenu("Menu_Quit");
				}
				break;
		}
    }
}
