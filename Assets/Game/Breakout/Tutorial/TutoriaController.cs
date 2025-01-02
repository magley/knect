using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoriaController : MonoBehaviour
{
    [SerializeField] private GameObject clearArea;
    [SerializeField] private Image playerIcon;
	[SerializeField] private Image sofa;
	[SerializeField] private Image chair;
    private float clearArea_timeUntilMove = 2f;
    private float clearArea_timeUntilMoveStop = 1.5f;

	[SerializeField] List<Sprite> sprPlayerIdleFrames;

    private void AnimatePlayerIcon()
    {
        playerIcon.sprite = sprPlayerIdleFrames[(int)(Time.time * 1.5f) % sprPlayerIdleFrames.Count];
    }

    void Start()
    {
        
    }

    private void HandleClearArea()
    {
        if (!clearArea.activeSelf)
        {
            return;
        }

        clearArea_timeUntilMove -= Time.deltaTime;

        if (clearArea_timeUntilMove <= 0f)
        {
			if (clearArea_timeUntilMoveStop > 0f)
            {
                clearArea_timeUntilMoveStop -= Time.deltaTime;

				chair.rectTransform.position += new Vector3(-38, 38, 0) * Time.deltaTime;
				sofa.rectTransform.position += new Vector3(38, 48, 0) * Time.deltaTime;
			}
		}
	}

    void Update()
    {
        AnimatePlayerIcon();
        HandleClearArea();

	}
}
