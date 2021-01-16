using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;
using System;
using TMPro;

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

    private GhostScript currentGhost;
    private Vector3 axis;
    private Camera __playerCamera;
    private LaserControl __laserControl;

    private bool __capturing;

    void Start()
    {
        __playerCamera = gameObject.GetComponentInChildren<Camera>();
        __laserControl = gameObject.GetComponentInChildren<LaserControl>();

        __capturing = false;
    }

    void Update()
    {
        GameObject targetGhost = Utils.GetGhostInFront(__playerCamera.gameObject, __playerCamera.fieldOfView, captureDistance);
        if (targetGhost != null) currentGhost = targetGhost.GetComponent<GhostScript>();
        else currentGhost = null;

        AnimatorStateInfo animState = handsAnimator.GetCurrentAnimatorStateInfo(0);

        // If we have a Ghost in front
        if (currentGhost != null)
        {
            if (Input.GetMouseButtonDown(0) && animState.IsName("Idle"))
            {
                handsAnimator.SetTrigger("Shoot");
                sounds.PlayStartCapture();
                __laserControl.EnableLaser(targetGhost.transform.position);
            }

            if (Input.GetMouseButton(0) && animState.IsName("Shooting"))
            {
                __laserControl.UpdateLaser(targetGhost.transform.position);
            }

            else if (Input.GetMouseButtonUp(0) && animState.IsName("Shooting"))
            {
                handsAnimator.SetTrigger("Stop");
                sounds.PlayEndCapture();
                __laserControl.DisableLaser();
            }
        }

        // If we do not have Ghost in front and it was shooting -> Stop the animation
        else if (animState.IsName("Shooting"))
        {
            handsAnimator.SetTrigger("Stop");
            sounds.PlayEndCapture();
            __laserControl.DisableLaser();
        }

        // If we do not have Ghost in front and it was shooting -> Error sound
        else if (Input.GetMouseButtonDown(0))
        {
            sounds.PlayErrorCapture(false);
        }
    }

    private void Capture()
    {
        __capturing = false;
        currentGhost.Capture();
        SetCaptureState(false);

        TMP_Text numGhosts = GameObject.Find("GhostCounter").GetComponent<TMP_Text>();
        string count = numGhosts.text.Substring(2);
        numGhosts.text = $"x {Int32.Parse(count) - 1}";
    }

    public void SetCaptureState(bool state)
    {
        /*
        __capturing = state;

        if (__capturing)
        {

            if (!captureParticle.isPlaying)
            {
                Debug.Log("Play particle");
                captureParticle.Play();
            }
            
        }
        else  captureParticle.Stop();
        */
    }
}
