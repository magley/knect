using UnityEngine;
using UnityEngine.UI;

public class UIBonus : MonoBehaviour
{
	private GameObject MyUIContainer;
	[SerializeField] private Text UIComboText;
	[SerializeField] private float KeepUIOnScreenTimer = -1;

	private void Start()
	{
		Ball.OnComboMade += ShowCombo;
		MyUIContainer = transform.GetChild(0).gameObject;
	}

	private void Update()
	{
		if (KeepUIOnScreenTimer > 0)
		{
			KeepUIOnScreenTimer -= Time.deltaTime;

			if (KeepUIOnScreenTimer <= 0)
			{
				HideUI();
				KeepUIOnScreenTimer = -1;
			}
		}
	}

	private void ShowCombo(int combo)
	{
		ShowUI();
		UIComboText.text = $"{combo}";
		KeepUIOnScreenTimer = 3;
	}

	private void ShowUI()
	{
		MyUIContainer.SetActive(true);
	}

	private void HideUI()
	{
		MyUIContainer.SetActive(false);
	}
}
