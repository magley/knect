using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UIElements.Image;

public class UIBonus : MonoBehaviour
{
	[SerializeField] private Text UIComboText;
	[SerializeField] private GameObject Background;
	[SerializeField] private Text Title;
	[SerializeField] private float KeepUIOnScreenTimer = -1;
	private AudioSource sourceBonus;

	private bool IsBeingShown = false;

	private float uiComboTextSizeCurrent = 157;
	private int uiComboTextSizeDesired = 157;

	private void Start()
	{
		Ball.OnComboMade += ShowCombo;
		UIComboText.text = "";
		sourceBonus = GetComponent<AudioSource>();
	}

	private void OnDestroy()
	{
		Ball.OnComboMade -= ShowCombo;
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

		{
			Color col = Background.GetComponent<RawImage>().color;
			float targetAlpha = IsBeingShown ? 1f : 0f;
			col.a = Mathf.Lerp(col.a, targetAlpha, 0.15f);
			Background.GetComponent<RawImage>().color = col;
		}
		{
			Color col = UIComboText.color;
			float targetAlpha = IsBeingShown ? 1f : 0f;
			col.a = Mathf.Lerp(col.a, targetAlpha, 0.15f);
			UIComboText.color = col;
		}
		{
			Color col = Title.color;
			float targetAlpha = IsBeingShown ? 1f : 0f;
			col.a = Mathf.Lerp(col.a, targetAlpha, 0.15f);
			Title.color = col;
		}
		{
			if (uiComboTextSizeCurrent > uiComboTextSizeDesired)
			{
				uiComboTextSizeCurrent -= 4 * Time.deltaTime * 60f;
			}

			UIComboText.fontSize = (int)uiComboTextSizeCurrent;
		}
	}

	private void ShowCombo(int combo)
	{
		ShowUI();
		UIComboText.text = $"{combo}";
		KeepUIOnScreenTimer = 3;

		// Play combo ding
		sourceBonus.pitch = 1 + ((Mathf.Max(combo, 3) - 3) / 7f);
		sourceBonus.Play();
	}

	private void ShowUI()
	{
		IsBeingShown = true;
		uiComboTextSizeCurrent = uiComboTextSizeDesired + 100;
		UIComboText.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, Random.Range(-5f, 5f) * 2.5f);
	}

	private void HideUI()
	{
		IsBeingShown = false;
	}
}
