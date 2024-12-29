using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BreakoutCameraController : MonoBehaviour
{
    private GameObject player;
    private List<Collider> playerColliders;
    [SerializeField] private Vector3 myCentroid;
    private Vector3 initialCentroid;
    private Vector3 initialPosition;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerColliders = player.transform.GetComponentsInChildren<Collider>().ToList();

        myCentroid = GetPlayerCentroid();
        initialCentroid = myCentroid;
        initialPosition = transform.position;
	}

    void Update()
    {
        Vector3 targetCentroid = GetPlayerCentroid();
        myCentroid = Vector3.Lerp(myCentroid, targetCentroid, 0.05f);
        myCentroid = initialCentroid + (myCentroid - initialCentroid) * 0.85f;

		transform.LookAt(myCentroid, Vector3.up);
        transform.position = initialPosition + new Vector3((myCentroid - initialCentroid).x, (myCentroid - initialCentroid).y, 0) * 0.9f;
	}

    private Vector3 GetPlayerCentroid()
    {
		Vector3 playerCentroid = Vector3.zero;
		foreach (var cld in playerColliders)
		{
			playerCentroid += cld.transform.position;
		}

		playerCentroid /= playerColliders.Count;
        return playerCentroid;
	}
}
