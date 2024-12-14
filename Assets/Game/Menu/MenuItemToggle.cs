using UnityEngine;
using UnityEngine.UI;

public class MenuItemToggle : MonoBehaviour
{
    public bool IsToggled { get; private set; } = false;

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
    }

    [SerializeField] private Image imageIndicator;
	[SerializeField] private Sprite sprNegative;
	[SerializeField] private Sprite sprPositive;
}
