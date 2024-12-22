using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPanCameraController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;

    public void OnAnimFinish()
    {
        mainCamera.enabled = true;
        GetComponent<Camera>().enabled = false;
        FindObjectOfType<LevelManager>().TextCountdownStart();
    }
}
