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

		shard.transform.position += vel / 10f;
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
						int i = Random.Range(0, GenericDrops.Length);
						var prefab = GenericDrops[i];

						GameObject o = Instantiate(prefab, transform.parent);
						o.transform.position = transform.position;
					}
				}
				break;
			case DropOnBreak.Miniballs:
				{
					for (int i = 0; i < 5; i++)
					{
						Vector3 rand = new(
							Random.Range(-1, 1),
							Random.Range(-1, 1),
							Random.Range(-1, 1)
						);
						GameObject o = Instantiate(MiniballPrefab, transform.parent);
						o.transform.position = transform.position + rand;
						o.transform.rotation = Quaternion.Euler(new(
							Random.Range(-90, 90),
							Random.Range(-90, 90),
							Random.Range(-90, 90)
						));
					}
				}
				break;
			default: return;
        }
	}
}
