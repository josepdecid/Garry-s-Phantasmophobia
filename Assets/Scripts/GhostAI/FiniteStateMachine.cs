using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    [SerializeField]
    private GameObject ghost;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float patrolSpeed = 1.0f;
    [SerializeField]
    private float fleeSpeed = 1.0f;
    [SerializeField]
    private float hideTimeout = 10.0f;
    [SerializeField]
    private float searchTimeout = 10.0f;

    private State __currentState;
    private AnimatorClipInfo[] __currentClipInfo;
    private string __clipName;

    void Start()
    {
        animator.SetFloat("hideTimeout", hideTimeout);
        animator.SetFloat("searchTimeout", searchTimeout);

        __currentState = new PatrolState(ghost, animator, patrolSpeed);
        __currentState.Enter();
    }

    void Update()
    {
        __currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string currentClipName = __currentClipInfo[0].clip.name; 
   
        if(__clipName != currentClipName) {
            __clipName = currentClipName;
            
            State newState = GetStateClass(__clipName);
            ChangeState(newState);
        }

        __currentState.StateUpdate();
    }

    void OnCollisionEnter(Collision collision)
    {
        __currentState.StateCollisionEnter(collision);
    }

    public void ChangeState(State newState)
    {
        __currentState.Exit();
        __currentState = newState;
        __currentState.Enter();
    }

    private State GetStateClass(string stateName) {
        switch (stateName)
        {
            case "Patrol":
                return new PatrolState(ghost, animator, patrolSpeed); 
            case "Flee":
                return new FleeState(ghost, animator); 
            case "Hide":
                return new HideState(ghost, animator, hideTimeout);
            default:
                return null;
        }
    }
}
