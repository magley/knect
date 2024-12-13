using UnityEngine;
using UnityEngine.UI;

public class WorldSpaceText : MonoBehaviour
{
    [SerializeField] private Text uiText;
    private int lifeTime = 160;
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
			sizeCurrent -= 1.15f;
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
			lifeTime--;
		}

		if (lifeTime == 0)
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

    public void SetSize(int size)
    {
        size = 20;
        sizeCurrent = ((size + 5) * 1.1f);
    }
}
