using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private Rigidbody rb;
	private TrailRenderer trailRenderer;
	private bool CanGetHitByBouncePad = true;

	public static event Action<int> OnComboMade;

	/// <summary>
	/// Combo is increased for each breakable that the ball destroys.
	/// When the ball hits the player, the combo resets.
	/// </summary>
	private int Combo { get => _combo; set => _combo = value; }
	[SerializeField] private int _combo = 0;

	/// <summary>
	/// Bonus is increased for each breakable that the ball destroys.
	/// When the ball hits anything else, the bonus is reduced.
	/// </summary>
	private int Bonus
	{
		get => _bonus;
		set
		{
			_bonus = value;
			StandardSpeed = 14f + _bonus / 2f;
		}
	}
	[SerializeField] private int _bonus = 0;

	/// <summary>
	/// Velocity magnitude at level start and when the ball is hit by the player.
	/// </summary>
	[SerializeField] private float StandardSpeed = 14f;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		trailRenderer = GetComponent<TrailRenderer>();
		rb.velocity = transform.forward * StandardSpeed;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("BouncePad"))
		{
			if (CanGetHitByBouncePad)
			{
				BounceOffBouncePad();
				CanGetHitByBouncePad = false;
				ResetCombo();
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

	private void Update()
	{
		HandleTrailLength();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Brick"))
		{
			Destroy(collision.gameObject);
			IncreaseComboAndBonus();
		}
		else
		{
			DecreaseBonus();
		}
	}

	private void HandleTrailLength()
	{
		float trailRenderTargetTime = 0.075f + _combo / 4f;
		trailRenderer.time = Mathf.Lerp(trailRenderer.time, trailRenderTargetTime, 0.25f);
	}

	private void BounceOffBouncePad()
	{
		float angleOffset = 1f;

		Vector3 forward = transform.forward;
		float xOffset = UnityEngine.Random.Range(-angleOffset, angleOffset);
		float yOffset = UnityEngine.Random.Range(-angleOffset, angleOffset);
		float zOffset = UnityEngine.Random.Range(-angleOffset, angleOffset);

		Vector3 dir = new Vector3(forward.x + xOffset * 10, forward.y + yOffset * 5, forward.z).normalized;
		dir.z = 1;
		rb.velocity = dir * StandardSpeed;
	}


	private void IncreaseComboAndBonus()
	{
		Combo++;
		Bonus++;
	}

	private void DecreaseBonus()
	{
		Bonus--;
		if (Bonus < 0)
		{
			Bonus = 0;
		}
	}

	private void ResetCombo()
	{
		if (Combo >= 3)
		{
			OnComboMade?.Invoke(Combo);
		}

		Combo = 0;
	}
}
