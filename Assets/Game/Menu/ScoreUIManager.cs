using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private Sprite sprHead;
    [SerializeField] private Sprite sprBody;
    [SerializeField] private Sprite sprTail;
    [SerializeField] private Sprite sprUnchecked;
    [SerializeField] private Sprite sprChecked;
    [SerializeField] private Font font;

    private bool created = false;

    void Awake()
    {
        if (!created)
        {
            CreateSprites();
            created = true;
        }
    }

    private void CreateSprites()
    {
		int highScore = XMLManager.instance.data.GetHighScore();
		var numberFormat = new NumberFormatInfo();
		numberFormat.CurrencyGroupSeparator = ".";

        {
			SpawnText("High Score", new(432, 96), Color.white);
			string formattedNumber = highScore.ToString("#,0", numberFormat);
			SpawnText($"{formattedNumber}", new(220, 96), Color.white, TextAnchor.MiddleRight);
		}

		for (int i = 0; i < LevelStartBonus.Bonuses.Count; i++)
        {
            Sprite spr = sprBody;
            if (i == 0)
            {
                spr = sprHead;

			}
            else if (i == LevelStartBonus.Bonuses.Count - 1)
            {
                spr = sprTail;
			}
            SpawnSprite(spr, new(0, -64 * i));

            var bonus = LevelStartBonus.Bonuses[i];
			if (highScore < bonus.HighScoreThreshold)
			{
				SpawnSprite(sprUnchecked, new(0, -64 * i));
			}
            else
            {
				SpawnSprite(sprChecked, new(0, -64 * i));
			}

            Color color = (highScore < bonus.HighScoreThreshold) ? Color.gray : Color.white;

            SpawnText(bonus.Description, new(500, -64 * i), color);

			string formattedNumber = bonus.HighScoreThreshold.ToString("#,0", numberFormat);
			SpawnText($"{formattedNumber}", new(220, -64 * i), color, TextAnchor.MiddleRight);
		}
	}

    private void SpawnSprite(Sprite spr, Vector2 pos)
    {
		GameObject imageObject = new GameObject("ScoreUIImage");
        imageObject.transform.SetParent(transform, false);

		Image image = imageObject.AddComponent<Image>();
		image.sprite = spr;

		image.rectTransform.sizeDelta = new Vector2(64, 64);
		image.rectTransform.localScale = Vector2.one;
        image.rectTransform.localPosition = pos;
	}

    private void SpawnText(string text, Vector2 pos, Color color, TextAnchor align = TextAnchor.MiddleLeft)
    {
		GameObject imageObject = new GameObject("ScoreUIText");
		imageObject.transform.SetParent(transform, false);

		Text txt = imageObject.AddComponent<Text>();
        txt.text = text;
        txt.fontSize = 32;
        txt.color = color;
		txt.rectTransform.sizeDelta = new Vector2(900, 64);
		txt.rectTransform.localScale = Vector2.one;
		txt.rectTransform.localPosition = pos;
        txt.alignment = align;
        txt.font = font;

        Outline outline = imageObject.AddComponent<Outline>();
        outline.effectColor = Color.black;
        outline.effectDistance = Vector2.one * 2;

	}
}