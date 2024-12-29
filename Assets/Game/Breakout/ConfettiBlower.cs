using UnityEngine;

public class ConfettiBlower : MonoBehaviour
{
    private ParticleSystem ps;
    private AudioSource sndConfettiPop;

    /// <summary>
    /// Not all blowers emit confetti at the same time.
    /// We have a 2-by-2 sequence.
    /// Order determines which sequence the blower is a part of.
    /// </summary>
    [SerializeField] private int order = 0;
    /// <summary>
    /// When the timer reaches 0, the blower will try to emit
    /// confetti for the next sequence of blowers.
    /// </summary>
    private float emitNextInOrderTimer = 0f;

	/// <summary>
	/// HACK: Emitting through sequences is only possible when this is true.
	/// I added this to prevent duplicates: if there are K confetti blowers
	/// in sequence N, then EmitIfOrderIs(N+1) will be called K times instead
    /// of just once.
	/// </summary>
	private bool canEmit = true;
    private float canEmitAgainTimer = 0f;

    void Start()
    {
		ps = GetComponentInChildren<ParticleSystem>();
		sndConfettiPop = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (emitNextInOrderTimer > 0)
        {
            emitNextInOrderTimer -= Time.deltaTime;

            if (emitNextInOrderTimer <= 0f)
            {
				EmitForSequence(order + 1);
			}
        }

        if (canEmitAgainTimer > 0)
        {
            canEmitAgainTimer -= Time.deltaTime;
            if (canEmitAgainTimer <= 0f)
            {
                canEmit = true;
			}
		}
	}

    private void Emit()
    {
        ps.Play();
        sndConfettiPop.Play();
    }

	private void EmitIfOrderIs(int theOrder)
	{
        if (order == theOrder)
        {
            if (canEmit)
            {
				canEmit = false;
                canEmitAgainTimer = 0.1f;

				Emit();
			    emitNextInOrderTimer = 0.7f;
            }
        }
	}

	private void EmitForSequence(int order)
	{
		foreach (var confettiBlower in FindObjectsOfType<ConfettiBlower>())
		{
			confettiBlower.EmitIfOrderIs(order);
		}
	}

	public static void StartSequenceEmit()
    {
        foreach (var confettiBlower in FindObjectsOfType<ConfettiBlower>())
        {
            confettiBlower.EmitIfOrderIs(0);
        }
    }
}
