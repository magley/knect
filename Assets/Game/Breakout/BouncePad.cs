using UnityEngine;

public class BouncePad : MonoBehaviour
{
    private Vector3 posOffset = Vector3.zero;
    private Vector3 startPos;

    private Rigidbody rb;

	private void Start()
	{
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
	}

	void FixedUpdate()
    {
        Control_KeyboardMouse();

        Vector3 offsetWithDistance = new(posOffset.x * 3.5f, posOffset.y * 2f, 0);
		transform.position = Vector3.Lerp(transform.position, startPos + offsetWithDistance, 0.15f);
	}

    private void Control_KeyboardMouse()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        bool inputPunch = Input.GetButton("Fire1");

        posOffset = new Vector3(inputX, inputY, 0);
	}
}
