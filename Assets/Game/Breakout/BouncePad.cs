using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    // TODO: Extract somewhere else
    public static bool IsKinect = false;

    private Rigidbody rb;

	private void Start()
	{
        rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
    {
        if (IsKinect)
        {
            Control_Kinect();
        }
        else
        {
            Control_KeyboardMouse();
        }
    }

    private void Control_Kinect()
    {

    }

    private void Control_KeyboardMouse()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        bool inputPunch = Input.GetButton("Fire1");

        float speed = 10f;

		Vector3 tempVect = new Vector3(inputX, inputY, 0);
		tempVect = tempVect.normalized * speed * Time.deltaTime;
		rb.MovePosition(transform.position + tempVect);

		//rb.velocity = new Vector3(inputX, inputY, 0f) * 5f;
	}
}
