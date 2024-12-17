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

        Toggle,

        ResumeGame,
        QuitToMainMenu,
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

    private MenuItemToggle toggle;

    private float focusRotation = 0f;

    private UIManager UIManager;

    void Start()
    {
		audioSource = GetComponent<AudioSource>();
        parentMenu = GetComponentInParent<MenuManager>();
        baseScale = transform.localScale;
        UIManager = FindFirstObjectByType<UIManager>();

        toggle = GetComponent<MenuItemToggle>();
        if (toggle == null && Type == MenuItemType.Toggle)
        {
            Debug.LogError("A 'Toggle' menu item requires a 'MenuItemToggle' component!");
        }

		transform.localScale = baseScale * scaleUnfocus;
	}

    void Update()
    {
		HandleScale();
        HandleRotation();

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

    private void HandleRotation()
    {
        float targetRot = 0f;
        if (Focused)
        {
			targetRot = Mathf.Sin(Time.time * 1.75f) * 15f;
		}

        focusRotation = Mathf.Lerp(focusRotation, targetRot, 0.35f);


		transform.rotation = Quaternion.Euler(new Vector3(0, 0, focusRotation));
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
                    UIManager.TransitionAndThen(() =>
                    {
#if UNITY_EDITOR
						UnityEditor.EditorApplication.isPlaying = false;
#else
                    Application.Quit();
#endif
					}, 0);
				}
				break;
            case MenuItemType.PlayBreakout:
                {
                    UIManager.TransitionAndThen(() =>
                    {
                        SceneManager.LoadScene("BreakoutScene");
                    }, 1);
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
            case MenuItemType.Toggle:
                {
                    toggle.Toggle();
				}
                break;
            case MenuItemType.ResumeGame:
                {
                    if (FindFirstObjectByType<PauseManager>() is PauseManager pauseManager)
                    {
                        pauseManager.ResumeGame();
					}
                    else
                    {
                        Debug.LogError("No gameobject with 'PauseManager' component found! You need a 'PauseManager' to unpause the game.");
                    }
                }
                break;
            case MenuItemType.QuitToMainMenu:
                {
					UIManager.TransitionAndThen(() =>
					{
						SceneManager.LoadScene("MenuScene");
					}, 1);
				}
                break;
		}
    }
}
