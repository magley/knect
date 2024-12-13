using UnityEngine;

public class LevelManager : MonoBehaviour
{
	private int destroyedBallsOnLevelEnd = 0;
	[SerializeField] private GameObject PrefabWorldSpaceTextForScore;

    void Start()
    {
		destroyedBallsOnLevelEnd = 0;
	}

    void Update()
    {
        
    }

	public void OnLevelComplete()
	{
		Invoke(nameof(DestoryRandomBall), 1f);
	}

	private void DestoryRandomBall()
	{
		var balls = FindObjectsByType<Ball>(FindObjectsSortMode.InstanceID);
		if (balls.Length == 0)
		{
			Invoke(nameof(OnAllBallsDestroyed), 2f);
		}
		else
		{
			destroyedBallsOnLevelEnd++;
			int points = (destroyedBallsOnLevelEnd * destroyedBallsOnLevelEnd) * 500;
			int i = Random.Range(0, balls.Length);

			balls[i].DestroyAndGivePoints(points);
			var pointsText = Instantiate(PrefabWorldSpaceTextForScore).GetComponent<WorldSpaceText>();
			pointsText.SetText($"{points}");
			pointsText.SetSize(5 + 3 * destroyedBallsOnLevelEnd);
			pointsText.gameObject.transform.position = balls[i].transform.position;

			Invoke(nameof(DestoryRandomBall), 0.5f);
		}
	}

	private void OnAllBallsDestroyed()
	{

	}
}
