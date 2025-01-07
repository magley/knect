using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour
{
    [SerializeField] private List<Material> Skyboxes;
    [SerializeField] private List<Color> SkyColors;

    private Light Light;

    void Start()
    {
		Light = GetComponent<Light>();

        ApplyPreset(Random.Range(0, Skyboxes.Count));
	}

    private void ApplyPreset(int index)
    {
		RenderSettings.skybox = Skyboxes[index];
        Light.color = SkyColors[index];
	}
}
