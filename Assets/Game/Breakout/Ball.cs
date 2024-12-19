using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Ball : MonoBehaviour
{
	private Rigidbody rb;
	private TrailRenderer trailRenderer;
	private ParticleSystem particles;
	private bool CanGetHitByBouncePad = true;
	[SerializeField] public bool DestroyOnImpact = false;

	private AudioSource SndHitWallSource;

	public static event Action<int> OnComboMade;

	[SerializeField] private AudioClip[] sndBallHit = { };
	[SerializeField] private AudioClip[] sndBallHitHard = { };

	[SerializeField] private Material[] PossibleMaterial = { };

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
			StandardSpeed = 18f + _bonus / 2f;
		}
	}
	[SerializeField] private int _bonus = 0;

	/// <summary>
	/// Velocity magnitude at level start and when the ball is hit by the player.
	/// </summary>
	[SerializeField] private float StandardSpeed = 18f;
	/// <summary>
	/// Minimum speed in any axis. This is to prevent the ball from moving too slowly
	/// on the z-axis which means it takes a long time for the ball to move across the arena.
	/// </summary>
	private float MinimumSpeedOnAxis = 9;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		trailRenderer = GetComponent<TrailRenderer>();
		SndHitWallSource = GetComponent<AudioSource>();
		particles = GetComponent<ParticleSystem>();
		rb.velocity = transform.forward * StandardSpeed;

		GetComponent<Renderer>().material = PossibleMaterial[UnityEngine.Random.Range(0, PossibleMaterial.Length)];
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
				particles.Emit(1);

				if (other.GetComponent<BouncePad>() is BouncePad bouncePad)
				{
					bouncePad.SpinPaddleRing();
				}

				var clip = sndBallHitHard[UnityEngine.Random.Range(0, sndBallHitHard.Length)];
				SndHitWallSource.clip = clip;
				SndHitWallSource.Play();
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
		HandleVelocityDirection();
	}

	private void HandleVelocityDirection()
	{
		if (Math.Abs(rb.velocity.z) < MinimumSpeedOnAxis)
		{
			rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Math.Sign(rb.velocity.z) * MinimumSpeedOnAxis);
		}
		if (Math.Abs(rb.velocity.y) < MinimumSpeedOnAxis)
		{
			rb.velocity = new Vector3(rb.velocity.x, Math.Sign(rb.velocity.y) * MinimumSpeedOnAxis, rb.velocity.z);
		}
	}

	private void SlowTheBallDownABit()
	{
		float currentSpeed = rb.velocity.magnitude;
		if (currentSpeed < 13f)
		{
			currentSpeed -= 0.2f;
		}
		rb.velocity = rb.velocity.normalized * currentSpeed;
	}

	void OnCollisionEnter(Collision collision)
	{
		SlowTheBallDownABit();

		if (collision.gameObject.CompareTag("Brick"))
		{
			Destroy(collision.gameObject);
			IncreaseComboAndBonus();
			GameState.AddScore(Bonus * 50);

			if (DestroyOnImpact)
			{
				Destroy(gameObject);
			}
		}
		else
		{
			DecreaseBonus();

			var clip = sndBallHit[UnityEngine.Random.Range(0, sndBallHit.Length)];
			SndHitWallSource.clip = clip;
			SndHitWallSource.Play();
		}
	}

	private void HandleTrailLength()
	{
		float trailRenderTargetTime = 0.025f + _combo / 50f;
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
			GameState.AddScore(Combo * 1000);

			if (Combo > GameState.BestCombo)
			{
				GameState.BestCombo = Combo;
			}
		}

		Combo = 0;
	}
}
