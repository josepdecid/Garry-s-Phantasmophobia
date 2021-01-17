using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;
using TMPro;
using UnityEngine.AI;

public class CaptureScript : MonoBehaviour
{
    [SerializeField]
    private Animator handsAnimator = null;
    [SerializeField]
    private Sounds sounds = null;
    [SerializeField]
    private float captureDistance = 5f;
    [SerializeField]
    private float dragSpeed = 3f;
    [SerializeField]
    private float captureTime = 2f;
    private GhostScript currentGhost;
    private Vector3 axis;
    private Camera __playerCamera;
    private LaserControl __laserControl;

    private bool __capturing;
    private bool __shooting;
    private float __currentTime;
    

    void Start()
    {
        __playerCamera = gameObject.GetComponentInChildren<Camera>();
        __laserControl = gameObject.GetComponentInChildren<LaserControl>();

        __capturing = false;
        __shooting = false;
    }

    void FixedUpdate()
    {
        GameObject targetGhost = Utils.GetGhostInFront(__playerCamera.gameObject, __playerCamera.fieldOfView, captureDistance);
        if (targetGhost != null) currentGhost = targetGhost.GetComponent<GhostScript>();
        else currentGhost = null;

        AnimatorStateInfo animState = handsAnimator.GetCurrentAnimatorStateInfo(0);

        // If we have a Ghost in front
        if (currentGhost != null)
        {
            // Start Shooting
            if (Input.GetMouseButtonDown(0) && !__shooting)
            {
                __shooting = true;
                handsAnimator.SetTrigger("Shoot");
                sounds.PlayStartCapture();
                __laserControl.EnableLaser(targetGhost.transform.position);
                __currentTime = 0f;
            }

            // While shooting
            else if (Input.GetMouseButton(0) && __shooting && __currentTime < captureTime)
            {
                __laserControl.UpdateLaser(targetGhost.transform.position);
                __currentTime += Time.deltaTime;
            }

            // Ghost captured
            else if (Input.GetMouseButton(0) && __shooting && __currentTime >= captureTime) {
                __shooting = false;
                Capture();
                __laserControl.DisableLaser();
            }

            // Stopped shooting
            else if (Input.GetMouseButtonUp(0) && __shooting)
            {
                __shooting = false;
                handsAnimator.SetTrigger("Stop");
                sounds.PlayEndCapture();
                __laserControl.DisableLaser();
            }

            else {
                __shooting = false;
                __laserControl.DisableLaser();
            }
        }

        // If we do not have Ghost in front and it was shooting -> Stop the animation
        else if (__shooting)
        {
            __shooting = false;
            handsAnimator.SetTrigger("Stop");
            sounds.PlayEndCapture();
            __laserControl.DisableLaser();
        }

        // If we do not have Ghost in front and it was shooting -> Error sound
        else if (Input.GetMouseButtonDown(0))
        {
            sounds.PlayErrorCapture(false);
        }

        else {
            __shooting = false;
            __laserControl.DisableLaser();
        }
    }

    private void Capture()
    {
        __capturing = false;
        currentGhost.Capture();

        TMP_Text numGhosts = GameObject.Find("GhostCounter").GetComponent<TMP_Text>();
        string count = numGhosts.text.Substring(2);
        numGhosts.text = $"x {Int32.Parse(count) - 1}";
    }
}
