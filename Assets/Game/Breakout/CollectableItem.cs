using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public enum Type
    {
        TimesTwo,
        TimesFive,
        Gem,
    }

    [SerializeField] public Type CollectableType;
    [SerializeField] private int ScoreBonus = 1000;
    [SerializeField] private AudioClip sndCollect = null;
    [SerializeField] private float MinSpeed = 7;
    [SerializeField] private float MaxSpeed = 17;
	[SerializeField] private GameObject PrefabWorldSpaceTextForScore = null;

	private float speed = 7;

	private void Start()
	{
        speed = Random.Range(MinSpeed, MaxSpeed);
	}

	private void OnCollect()
    {
        if (ScoreBonus > 0)
        {
            int points = GameState.AddScore(ScoreBonus);

            if (PrefabWorldSpaceTextForScore != null)
            {
				WorldSpaceText pointsText = Instantiate(PrefabWorldSpaceTextForScore).GetComponent<WorldSpaceText>();
				pointsText.SetText($"{points}");
				pointsText.SetSize(1);
				pointsText.SetLifetime(50);
                pointsText.gameObject.transform.localScale = Vector3.one * 0.25f;
				pointsText.gameObject.transform.position = transform.position;
            }
        }

        switch (CollectableType)
        {
            case Type.TimesTwo:
                PlayerAdditions.SetScoreMultiplier(2);
				break;
            case Type.TimesFive:
				PlayerAdditions.SetScoreMultiplier(5);
				break;
            case Type.Gem:
                break;
			default: break;
        }

        if (sndCollect != null)
        {
		    AudioSource.PlayClipAtPoint(sndCollect, Camera.main.transform.position);
        }

        Destroy(gameObject);
    }

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime, Space.World);
        transform.Rotate(transform.up * 6);

        if (transform.position.z < -100)
        {
            Destroy(gameObject);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("BouncePad"))
        {
            OnCollect();
        }
	}
}
