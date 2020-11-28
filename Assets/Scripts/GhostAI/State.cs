using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

abstract public class State
{
    protected GameObject _player;
    protected Camera _camera;
    
    protected GameObject _ghost;
    protected NavMeshAgent _agent;

    protected Animator _animator;
    
    protected StateParams _parameters = null;
    
    public State(GameObject player, GameObject ghost, Animator animator, StateParams parameters)
    {
        this._player = player;
        this._camera = player.GetComponentInChildren<Camera>();

        this._ghost = ghost;
        this._agent = ghost.GetComponent<NavMeshAgent>();

        this._animator = animator;

        this._parameters = parameters;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entering {this.GetType()} state.");

        _agent.isStopped = true;
        _agent.ResetPath();

        if (_parameters.isDebug) DrawDebugInfo();
    }

    public virtual void Exit()
    {
        Debug.Log($"Exiting {this.GetType()} state.");

        if (_parameters.isDebug) DestroyDebugInfo();
    }

    public abstract void StateUpdate();

    protected virtual void DrawDebugInfo() {}

    protected virtual void DestroyDebugInfo() {}
}
