using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public enum StateType { Roam, Flee, Search, Hide };

public class FiniteStateMachine : MonoBehaviour
{   
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

    private GameObject __player;
    private GameObject __ghost;
    
    private State __currentState;
    private StateType __currentStateType;
    
    private AnimatorClipInfo[] __currentClipInfo;
    private string __clipName;
    private StateParams __params;

    private State __roamState;
    private State __hideState;
    private State __searchState;
    private State __fleeState;

    void Awake()
    {
        __player = GameObject.FindWithTag("Player");
        
        __ghost = gameObject;

        __params = new StateParams(
            patrolSpeed, hideTimeout, searchTimeout,
            fleeSpeed, numSamples, samplingRadius, maxSamplingDistance,
            modeDebug, candidateGradient
        );

        __roamState = new RoamState(__player, __ghost, __params);
        __fleeState = new FleeState(__player, __ghost, __params);
        __searchState = new SearchState(__player, __ghost, __params);
        __hideState = new HideState(__player, __ghost, __params);
    }

    void Start()
    {   
        __currentStateType = StateType.Roam;
        __currentState = GetStateInstance();
        __currentState.Enter();
    }

    void Update()
    {
        StateType nextStateType = __currentState.StateUpdate();

        if (nextStateType != __currentStateType)
        {
            __currentStateType = nextStateType;
            State newState = GetStateInstance();
            ChangeState(newState);
        }
    }

    public void ChangeState(State newState)
    {
        __currentState.Exit();

        __currentState = newState;
        __currentState.Enter();
    }

    private State GetStateInstance() {
        if (modeDebug)
        {
            Color debugColor = GetStateColor();
            __ghost.GetComponentInChildren<MeshRenderer>().material.color = debugColor;
        }

        switch (__currentStateType)
        {
            case StateType.Roam:
                return __roamState; 

            case StateType.Flee:
                return __fleeState; 

            case StateType.Search:
                return __searchState;

            case StateType.Hide:
                return __hideState;
                
            default:
                return null;
        }
    }

    private Color GetStateColor() {
        switch (__currentStateType)
        {
            case StateType.Roam:
                return Color.blue; 

            case StateType.Flee:
                return Color.red; 

            case StateType.Search:
                return Color.yellow;

            case StateType.Hide:
                return Color.green;

            default:
                return Color.black;
        }
    }
}
