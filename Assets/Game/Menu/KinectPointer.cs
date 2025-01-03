using UnityEngine;

public class KinectPointer : MonoBehaviour
{
    public bool CanInteract { get; private set; } = true;
    private float CanInteractTimer = 0f;

    public void OnInteract()
    {
        CanInteract = false;
        CanInteractTimer = 1f;
    }

	void Update()
    {
		if (CanInteractTimer > 0f) {
            CanInteractTimer -= Time.deltaTime;
            if (CanInteractTimer < 0f)
            {
                CanInteract = true;
                CanInteractTimer = 0f;
			}
        }
    }
}
