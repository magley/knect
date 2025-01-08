using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slideshow : MonoBehaviour
{
    [SerializeField] private float TimeBetweenEachImage = 3f;
    [SerializeField] private List<Sprite> Images;
    private float time = 0f;
    private Image image;
    private int imageIndex = 0;

    void Start()
    {
        time = TimeBetweenEachImage;
        image = GetComponent<Image>();

        image.sprite = Images[imageIndex];
	}

    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = TimeBetweenEachImage;
            imageIndex = (imageIndex + 1) % Images.Count;
			image.sprite = Images[imageIndex];
		}
	}
}
