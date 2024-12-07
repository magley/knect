using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private Rigidbody rb;
	private bool CanGetHitByBouncePad = true;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.velocity = Vector3.forward * 10f;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("BouncePad"))
		{
			if (CanGetHitByBouncePad)
			{
				OnHitBouncePad();
				CanGetHitByBouncePad = false;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("BouncePad"))
		{
			CanGetHitByBouncePad = true;
		}
	}

	private void OnHitBouncePad()
	{
		float angleOffset = 1f;

		Vector3 forward = transform.forward;
		float xOffset = Random.Range(-angleOffset, angleOffset);
		float yOffset = Random.Range(-angleOffset, angleOffset);
		float zOffset = Random.Range(-angleOffset, angleOffset);

		Vector3 dir = new Vector3(forward.x + xOffset * 10, forward.y + yOffset * 5, forward.z).normalized;
		dir.z = 1;
		rb.velocity = dir * 10f;
		
		Debug.Log("Hit!");
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Brick"))
		{
			Destroy(collision.gameObject);
		}
	}
}
