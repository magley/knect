using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Brick : MonoBehaviour
{
    [SerializeField] private GameObject BrickShard;
	[SerializeField] private GameObject BallPrefab;
	[SerializeField] private GameObject MiniballPrefab;
	[SerializeField] private AudioClip SndBrickBreak;

	[SerializeField] private int Score = 100;

    public enum DropOnBreak
    {
        Nothing,
        ExtraBall,
		Miniballs,
    }
    [SerializeField] private DropOnBreak Drop;

	[SerializeField] private GameObject[] GenericDrops = { };

	private void OnDestroy()
	{
		AudioSource.PlayClipAtPoint(SndBrickBreak, Camera.main.transform.position);
        
		HandleDropItem();

		for (int i = 0; i < 4; i++)
        {
			SpawnBrickShard(i);
		}

		GameState.AddScore((int)(Score));
	}

	private void SpawnBrickShard(int i)
    {
		GameObject shard = Instantiate(BrickShard, transform.parent);
		shard.transform.position = transform.position;

		var rb = shard.GetComponent<Rigidbody>();
		Vector3 vel = Vector3.zero;

		vel.x = 5 * (i % 2 == 0 ? -1 : 1);
		vel.y = (6 + i >= 2 ? 8 : 0);
		vel.z = 5 * (i % 2 == 0 ? -1 : 1);

		vel.x += Random.Range(-3, 3);
		vel.y += Random.Range(-3, 3);
		vel.z += Random.Range(-3, 3);

		shard.transform.position += new Vector3(vel.x / Random.Range(4, 5), vel.y / Random.Range(4, 5), vel.z / Random.Range(5f, 10f));
		rb.velocity = vel;

		rb.angularVelocity = new Vector3(
			Random.Range(20f, 35f),
			Random.Range(20f, 35f),
			Random.Range(20f, 35f)
		);
	}

	private void HandleDropItem()
    {
		switch (Drop)
        {
			case DropOnBreak.ExtraBall:
				{
					GameObject o = Instantiate(BallPrefab, transform.parent);
					o.transform.position = transform.position;
				}
				break;
			case DropOnBreak.Nothing:
				{
					if (Random.Range(0, 8) == 0)
					{
						var drops = new List<GameObject>();
						foreach (var item in GenericDrops.Select(item => item.GetComponent<CollectableItem>()).Where(item => item != null))
						{
							bool canBeDropped = true;

							if (item.CollectableType == CollectableItem.Type.TimesTwo && PlayerAdditions.ForceScoreMultiplier5)
							{
								canBeDropped = false;
							}
							if (item.CollectableType == CollectableItem.Type.TimesTen && !PlayerAdditions.UnlockScoreMultiplier10)
							{
								canBeDropped = false;
							}

							if (canBeDropped)
							{
								drops.Add(item.gameObject);
							}
						}

						if (drops.Count > 0)
						{
							int i = Random.Range(0, drops.Count);
							var prefab = drops[i];

							GameObject o = Instantiate(prefab, transform.parent);
							o.transform.position = transform.position;
						}
					}
				}
				break;
			case DropOnBreak.Miniballs:
				{
					int N = 5;

					Vector3 forwardDirection = transform.forward;
					Vector3 downwardDirection = Vector3.down;
					float angleOffset = Vector3.SignedAngle(forwardDirection, downwardDirection, Vector3.right);

					for (int i = 0; i < N; i++)
					{
						float angle = Mathf.Lerp(angleOffset - 90f, angleOffset + 90f, (float)i / (N - 1));
						int[] dirY = new int[2] { -1, 1 };
						Vector3 dir = new Vector3(Mathf.Cos(angle), dirY[Random.Range(0, dirY.Length)], Mathf.Sin(angle));

						GameObject o = Instantiate(MiniballPrefab, transform.parent);
						o.transform.position = transform.position + dir * 0.2f;
						o.transform.rotation = Quaternion.LookRotation(dir);
					}
				}
				break;
			default: return;
        }
	}
}
