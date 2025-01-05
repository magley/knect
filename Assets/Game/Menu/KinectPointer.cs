using UnityEngine;
using UnityEngine.UI;

public class KinectPointer : MonoBehaviour
{
    public bool CanInteract { get; private set; } = true;
    private float CanInteractTimer = 0f;

    [SerializeField] private Image Icon;
    [SerializeField] private Text Hint;

    private float handIsntMovingTimer = 7f;
    private float handIsMovingThreshold = 0.5f;
	private float lastTime;
	[SerializeField] private float handMotion = 0f;
	private Vector3 handPosition = Vector3.zero;

	private void Start()
	{
        HideHint();
        handPosition = Icon.rectTransform.position;
	}

	public void OnInteract()
    {
        CanInteract = false;
        CanInteractTimer = 1f;
    }

	void Update()
    {
		if (CanInteractTimer > 0f) {
            CanInteractTimer -= Time.deltaTime;
            if (CanInteractTimer < 0f)
            {
                CanInteract = true;
                CanInteractTimer = 0f;
			}
        }

		HandleHandMoveHint();
	}

	private void HandleHandMoveHint()
    {
		Vector3 newHandPos = Icon.rectTransform.position;
		
		// Check if hand has moved far enough in the last N seconds.

		float deltaTime = Time.time - lastTime;
		handMotion += (newHandPos - handPosition).magnitude;

		if (deltaTime >= 1f * 0.1f)
		{
			if (handMotion > handIsMovingThreshold)
			{
				handIsntMovingTimer = 3f;
				HideHint();
				handMotion = 0f;
				lastTime = Time.time;
			}
			else
			{
				if (deltaTime >= 1f)
				{
					handMotion = 0f;
					lastTime = Time.time;
				}
			}
		}

		handPosition = newHandPos;

		// Update timer for showing the hint.

		if (handIsntMovingTimer > 0f)
		{
			handIsntMovingTimer -= Time.deltaTime;
			if (handIsntMovingTimer <= 0f)
			{
				ShowHint("Move your right hand");
			}
		}

		// Animate hint.
		
		if (handIsntMovingTimer <= 0f)
		{
			AnimateHint();
		}
	}

    private void ShowHint(string text)
    {
        Hint.text = text;
        Hint.enabled = true;
	}

    private void HideHint()
    {
		Hint.text = "";
		Hint.enabled = false;
		Hint.color = new Color(1, 1, 1, 0);
		Hint.GetComponent<Outline>().effectColor = new Color(
			1, 1, 1, Mathf.Lerp(Hint.color.a, 1, 5 * Time.deltaTime)
		);
	}

	private void AnimateHint()
    {
		Hint.transform.localRotation = Quaternion.Euler(
			0, 
			0, 
			Mathf.Cos(0.17f + Time.time * 2.5f) * 7
		);

		Outline outline = Hint.GetComponent<Outline>();  // Eh... this is expensive.
		outline.effectDistance = new Vector2(
			Mathf.Sin(Time.time * 2.35f) * 4,
			Mathf.Cos(Mathf.PI + Time.time * 2.63f) * 4
		);

		Hint.color = new Color(
			Mathf.Lerp(Hint.color.r, 0, 5 * Time.deltaTime),
			Mathf.Lerp(Hint.color.g, 0, 5 * Time.deltaTime),
			Mathf.Lerp(Hint.color.b, 0, 5 * Time.deltaTime), 
			Mathf.Lerp(Hint.color.a, 1, 5 * Time.deltaTime)
		);
		outline.effectColor = new Color(
			1,
			1,
			1,
			Mathf.Lerp(Hint.color.a, 1, 5 * Time.deltaTime)
		);
	}
}
