using System.Linq;
using UnityEngine;

public class ResultsUIBrick : MonoBehaviour
{
	[SerializeField] private GameObject BrickShard;
	private AudioSource AudioSource;
	private float speed = 8f;
	private bool destroyed = false;

	private void Start()
	{
		AudioSource = GetComponent<AudioSource>();
		transform.Rotate(new Vector3(
			Random.Range(0,360),
			Random.Range(0,360),
			Random.Range(0,360)
		));
	}

	private void Update()
	{
		transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
		transform.Rotate(transform.up * 120 * Time.deltaTime);
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (destroyed)
		{
			return;
		}
		if (collision.gameObject.CompareTag("ResultsUI Brick Breaker"))
		{
			Break();
		}
	}

	private void Break()
	{
		AudioSource.Play();
		for (int i = 0; i < 4; i++)
		{
			SpawnBrickShard(i);
		}
		destroyed = true;
		GetComponent<MeshRenderer>().enabled = false;
		GetComponent<Collider>().enabled = false;
	}

	private void SpawnBrickShard(int i)
	{
		GameObject shard = Instantiate(BrickShard, transform.parent);
		shard.transform.position = transform.position;
		shard.transform.localScale = transform.localScale;
		shard.transform.localRotation = transform.localRotation;

		var rb = shard.GetComponent<Rigidbody>();
		Vector3 vel = Vector3.zero;

		vel.x = 5 * (i % 2 == 0 ? -1 : 1);
		vel.y = (6 + i >= 2 ? 8 : 0);
		vel.z = 5 * (i % 2 == 0 ? -1 : 1);

		vel.x += Random.Range(-3, 3);
		vel.y += Random.Range(-3, 3);
		vel.z += Random.Range(-3, 3);

		shard.transform.position += new Vector3(vel.x / Random.Range(4, 5), vel.y / Random.Range(4, 5), vel.z / Random.Range(5f, 10f)) / 4f;
		rb.velocity = vel;

		rb.angularVelocity = new Vector3(
			Random.Range(20f, 35f),
			Random.Range(20f, 35f),
			Random.Range(20f, 35f)
		);
	}
}
