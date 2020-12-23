using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CaptureScript : MonoBehaviour
{
    // public CinemachineVirtualCamera gameCamera;
    // CinemachineImpulseSource impulse;

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

    public ParticleSystem captureParticle;
    public ParticleSystem finishParticle;
    
    // public GameObject light;
    [SerializeField]
    private GameObject tornado;
    // public Transform controller;

    Animator anim;
    // RandomRotation randomRot;
    private Renderer __tornadoRenderer;
    private Camera __playerCamera;
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
        captureParticle = GameObject.Find("CaptureParticles").GetComponent<ParticleSystem>();
        finishParticle = GameObject.Find("FinishParticles").GetComponent<ParticleSystem>();

        captureParticle.Stop();
        finishParticle.Stop();

        __playerCamera = gameObject.GetComponentInChildren<Camera>();

        __capturing = false;
    }

    void Update()
    {
        GameObject targetGhost = Utils.GetGhostInFront(__playerCamera.gameObject, captureDistance);
        if (targetGhost != null) currentGhost = targetGhost.GetComponent<GhostScript>();
        else currentGhost = null;
        if (currentGhost != null)
        {
            bool isCapturing = Input.GetMouseButton(0);
            Debug.Log($"Capturing: {__capturing}");
            if (isCapturing != __capturing) SetCaptureState(isCapturing);
            
            if (__capturing)
            {

                Debug.Log("Capturing the ghost!");
                
                /*else
                {
                    if (currentGhost.energy > 0)
                    {
                        float x = Input.GetAxis("Horizontal");
                        float z = Input.GetAxis("Vertical");

                        axis = new Vector3(-x, 0, -z);
                        float angle = Vector3.Angle(transform.forward, axis);

                        currentGhost.Damage(angle, axis);
                    }
                    else Capture();
                }*/
            }
        }
        else
        {
            SetCaptureState(false);
        }
    }

    private void Capture()
    {
        __capturing = false;
        currentGhost.Capture();
        SetCaptureState(false);
    }

    public void SetCaptureState(bool state)
    {
        
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
    }
}
