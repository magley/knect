using UnityEngine;

public class BrickShard : MonoBehaviour
{
    void FixedUpdate()
    {
        if (transform.position.y < -50)
        {
            Destroy(gameObject);
        }
    }
}
