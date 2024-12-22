using UnityEngine;

public class BrickShard : MonoBehaviour
{
    private Rigidbody rb;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
	}
	void FixedUpdate()
    {
        rb.AddForce(Physics.gravity * rb.mass);

        if (transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }
}
