using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{   
    [Header("Agents")]
    [SerializeField]
    private GameObject player = null;

    [Header("Patrol Parameters")]
    [SerializeField]
    private float patrolSpeed = 1.0f;

    [Header("Flee Parameters")]
    [SerializeField]
    private float fleeSpeed = 1.0f;
    [SerializeField]
    private int numSamples = 10;
    [SerializeField]
    private float samplingRadius = 5.0f;
    [SerializeField]
    private float maxSamplingDistance = 2.0f;

    [Header("Search Parameters")]
    [SerializeField]
    private float searchTimeout = 10.0f;

    [Header("Hide Parameters")]
    [SerializeField]
    private float hideTimeout = 10.0f;

    [Header("MISC")]
    [SerializeField]
    private bool modeDebug = false;
    [SerializeField]
    private Gradient candidateGradient = null;

    private GameObject __ghost;
    private Animator __animator;
    private State __currentState;
    private AnimatorClipInfo[] __currentClipInfo;
    private string __clipName;
    private StateParams __params;

    void Start()
    {   
        __ghost = gameObject;
        __animator = __ghost.GetComponent<Animator>();

        __animator.SetFloat("hideTimeout", hideTimeout);
        __animator.SetFloat("searchTimeout", searchTimeout);

        __params = new StateParams(
            patrolSpeed, hideTimeout, searchTimeout,
            fleeSpeed, numSamples, samplingRadius, maxSamplingDistance,
            modeDebug, candidateGradient
        );

        __currentState = new PatrolState(player, __ghost, __animator, __params);
        __currentState.Enter();
    }

    void FixedUpdate()
    {
        __currentClipInfo = __animator.GetCurrentAnimatorClipInfo(0);
        string currentClipName = __currentClipInfo[0].clip.name; 
   
        if(__clipName != currentClipName) {
            __clipName = currentClipName;
            
            State newState = GetStateClass(__clipName);
            ChangeState(newState);
        }

        __currentState.StateUpdate();
    }

    public void ChangeState(State newState)
    {
        __currentState.Exit();
        __currentState = newState;
        __currentState.Enter();
    }

    private State GetStateClass(string stateName) {
        if (modeDebug)
        {
            Color debugColor = GetStateColor(stateName);
            __ghost.GetComponentInChildren<MeshRenderer>().material.color = debugColor;
        }

        switch (stateName)
        {
            case "Patrol":
                return new PatrolState(player, __ghost, __animator, __params); 
            case "Flee":
                return new FleeState(player, __ghost, __animator, __params); 
            case "Search":
                return new SearchState(player, __ghost, __animator, __params);
            case "Hide":
                return new HideState(player, __ghost, __animator, __params);
            default:
                return null;
        }
    }

    private Color GetStateColor(string stateName) {
        switch (stateName)
        {
            case "Patrol":
                return Color.blue; 
            case "Flee":
                return Color.red; 
            case "Search":
                return Color.yellow;
            case "Hide":
                return Color.green;
            default:
                return Color.black;
        }
    }
}
