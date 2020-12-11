using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureGhost : MonoBehaviour
{
    [SerializeField]
    private float captureDistance = 1.0f;

    private Camera __playerCamera;

    void Start()
    {
        __playerCamera = gameObject.GetComponentInChildren<Camera>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            GameObject targetGhost = Utils.GetGhostInFront(__playerCamera.gameObject, captureDistance);
            if (targetGhost != null) 
            {
                Destroy(targetGhost);
            }
        }
    }
}
