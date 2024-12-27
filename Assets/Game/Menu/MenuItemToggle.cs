using UnityEngine;
using UnityEngine.UI;

public class MenuItemToggle : MonoBehaviour
{
    public bool IsToggled { get; private set; } = false;

    public enum VariableToToggle
    {
        None,
        PlayAsKinect,
    }

	[SerializeField] private VariableToToggle Variable = VariableToToggle.None;

	private void Start()
	{
        SyncWithVariable();
	}

	public void Toggle()
    {
        SetToggle(!IsToggled);
	}

    public void SetToggle(bool isToggled)
    {
        IsToggled = isToggled;
        if (imageIndicator != null)
        {
            imageIndicator.sprite = IsToggled ? sprPositive : sprNegative;
        }

        ToggleVariable();
	}

    private void ToggleVariable()
    {
        switch (Variable)
        {
            case VariableToToggle.PlayAsKinect:
                GameState.PlayingAsKinect = IsToggled;
                break;
            default:
                break;
        }
    }

    private void SyncWithVariable()
    {
		switch (Variable)
		{
			case VariableToToggle.PlayAsKinect:
				SetToggle(GameState.PlayingAsKinect);
				break;
			default:
				break;
		}
	}

    [SerializeField] private Image imageIndicator;
	[SerializeField] private Sprite sprNegative;
	[SerializeField] private Sprite sprPositive;
}
