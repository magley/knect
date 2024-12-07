using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] private GameObject BrickShard;
    [SerializeField] private AudioClip SndBrickBreak;
    
	private void OnDestroy()
	{
		AudioSource.PlayClipAtPoint(SndBrickBreak, Camera.main.transform.position);

		for (int i = 0; i < 4; i++)
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
	}
}
