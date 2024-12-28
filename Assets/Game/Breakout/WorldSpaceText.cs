using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceText : MonoBehaviour
{
    [SerializeField] private Text uiText;
    private float lifeTime = 2.2f;
    private int size = 20;
    private float sizeCurrent = 20;

	void Update()
    {
		HandleSize();
		HandleLifeTime();
	}

    private void HandleSize()
    {
		if (sizeCurrent > size)
		{
			sizeCurrent -= 1.15f * Time.deltaTime * 60f;
		}
		if (sizeCurrent < size)
		{
			sizeCurrent = size;
		}
		uiText.fontSize = (int)sizeCurrent;
	}

	private void HandleLifeTime()
    {
		if (lifeTime > 0)
		{
			lifeTime -= Time.deltaTime;
		}

		if (lifeTime <= 0)
		{
			var col = uiText.color;
			col.a -= 0.075f;

			uiText.color = col;
			if (col.a <= 0)
			{
				Destroy(gameObject);
			}
		}
	}

	public void SetText(string text)
	{
        uiText.text = text;
	}

    public void SetSize(int sizeNew)
    {
		sizeNew = size;
        sizeCurrent = size * 2;
    }

	public void SetLifetime(int ticks)
	{
		lifeTime = ticks / 60f;
	}
}
