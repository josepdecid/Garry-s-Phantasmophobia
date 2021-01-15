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
    // public CinemachineVirtualCamera gameCamera;
    // CinemachineImpulseSource impulse;

    [SerializeField]
    private Animator handsAnimator = null;
    [SerializeField]
    private Sounds sounds = null;
    [SerializeField]
    private float captureDistance = 5f;
    [SerializeField]
    private float dragSpeed = 3f;
    // [SerializeField]
    // private CharacterController c;

    // [Header("Ghost")]
    private GhostScript currentGhost;

    // [Space]
    // [Header("Booleans")]

    // [Space]
    // [Header("Rigs")]
    // public Rig flashRig;
    // public Rig captureRig;

    private Vector3 axis;
    
    // public GameObject light;
    [SerializeField]
    private GameObject tornado;
    // public Transform controller;

    Animator anim;
    // RandomRotation randomRot;
    private Renderer __tornadoRenderer;
    private Camera __playerCamera;
    private LaserControl __laserControl;
    // Renderer lightRenderer;

    private bool __capturing;

    void Start()
    {
        // c = GetComponent<CharacterController>();
        // impulse = gameCamera.GetComponent<CinemachineImpulseSource>();
        __tornadoRenderer = tornado.GetComponent<Renderer>();
        // lightRenderer = light.GetComponent<Renderer>();
        // __tornadoRenderer.material.SetFloat("Opacity", 0);
        // lightRenderer.material.SetFloat("Opacity", .3f);
        // randomRot = GetComponent<RandomRotation>();
        anim = GetComponent<Animator>();

        __playerCamera = gameObject.GetComponentInChildren<Camera>();
        __laserControl = gameObject.GetComponentInChildren<LaserControl>();

        __capturing = false;
    }

    void Update()
    {
        // TODO: Remove, just to test
        if (Input.GetMouseButtonDown(0) && handsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            handsAnimator.SetTrigger("Shoot");
            sounds.PlayStartCapture();
            __laserControl.EnableLaser();
        }

        if (Input.GetMouseButton(0) && handsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shooting"))
        {
            Ray ray = __playerCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
                print("I'm looking at " + hit.transform.name);
            else
                print("I'm looking at nothing!");
            Debug.DrawRay(__playerCamera.transform.position, Vector3.forward, Color.green);
            __laserControl.UpdateLaser(hit.transform.position);
        }

        else if (Input.GetMouseButtonUp(0) && handsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Shooting"))
        {
            handsAnimator.SetTrigger("Stop");
            sounds.PlayEndCapture();
            __laserControl.DisableLaser();
        }

        /*GameObject targetGhost = Utils.GetGhostInFront(__playerCamera.gameObject, captureDistance);
        if (targetGhost != null) currentGhost = targetGhost.GetComponent<GhostScript>();
        else currentGhost = null;
        if (currentGhost != null)
        {
            bool isCapturing = Input.GetMouseButton(0);
            handsAnimator.SetTrigger("Shoot");
            sounds.PlayStartCapture();


            Debug.Log($"Capturing: {__capturing}");
            if (isCapturing != __capturing) SetCaptureState(isCapturing);
            
            if (__capturing)
            {

                Debug.Log("Capturing the ghost!");
                
            }
        }
        else
        {
            SetCaptureState(false);
        }
        */
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
