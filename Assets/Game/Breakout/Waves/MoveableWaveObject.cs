using UnityEngine;

public class MoveableWaveObject : MonoBehaviour
{
	private Vector3 PointA;
	private Vector3 PointB => PointA + PointBOffset;
    [SerializeField] Vector3 PointBOffset = Vector3.up;
    [SerializeField] private float WaitingTime = 1f;
    [SerializeField] private float MovingTime = 1f;

    enum Modes {
        MoveToB,
        WaitAtB,
        MoveToA,
        WaitAtA,
    }
    private Modes Mode = Modes.WaitAtA;

    private float timer = 0f;

	private void Start()
	{
		PointA = transform.position;
        timer = WaitingTime;
	}

	void Update()
    {
		timer -= Time.deltaTime;

		switch (Mode) {
            case Modes.WaitAtA:
                if (timer <= 0)
                {
                    timer = MovingTime;
                    Mode = Modes.MoveToB;
				}
                break;
			case Modes.MoveToB:
				transform.position = PointB - (timer / MovingTime) * (PointB - PointA);
				if (timer <= 0)
				{
					timer = WaitingTime;
					Mode = Modes.WaitAtB;
				}
				break;
			case Modes.WaitAtB:
				if (timer <= 0)
				{
					timer = MovingTime;
					Mode = Modes.MoveToA;
				}
				break;
			case Modes.MoveToA:
				transform.position = PointA - (timer / MovingTime) * (PointA - PointB);
				if (timer <= 0)
				{
					timer = WaitingTime;
					Mode = Modes.WaitAtA;
				}
				break;
		}
    }
}
