using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    public enum Type
    {
        TimesTwo,
        TimesFive,
    }

    [SerializeField] private Type CollectableType;
    [SerializeField] private int ScoreBonus = 1000;

    private void OnCollect()
    {
        if (ScoreBonus > 0)
        {
            GameState.AddScore(ScoreBonus);
        }

        switch (CollectableType)
        {
            case Type.TimesTwo:
                PlayerAdditions.SetScoreMultiplier(2);
				break;
            case Type.TimesFive:
				PlayerAdditions.SetScoreMultiplier(5);
				break;
            default: break;
        }

        Destroy(gameObject);
    }

    void Update()
    {
        transform.Translate(Vector3.back * 5 * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up * 6);

        if (transform.position.z < -20)
        {
            Destroy(gameObject);
        }
    }

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("BouncePad"))
        {
            OnCollect();
        }
	}
}
