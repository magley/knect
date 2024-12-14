using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static event Action<string> OnActiveMenuChanged;

	public static void SetActiveMenu(string nameID)
	{
		OnActiveMenuChanged.Invoke(nameID);
	}

	[SerializeField] private Transform backgroundObject;
	private float backgroundScale = 1f;

	void Start()
    {
        
    }

    void Update()
    {
		HandleBackground();
	}

	private void HandleBackground()
	{
		backgroundObject.Rotate(new Vector3(0, 0, 0.1f));
		backgroundScale = Mathf.Sin(Time.time * 0.25f) * 0.5f + 1f;
		backgroundObject.localScale = new Vector3(1, 1, 0) * backgroundScale + Vector3.forward;
	}
}
