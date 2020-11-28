﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

abstract public class State
{
    protected GameObject _player;
    protected Camera _camera;
    
    protected GameObject _ghost;
    protected UnityEngine.AI.NavMeshAgent _agent;

    protected Animator _animator;
    
    public State(GameObject player, GameObject ghost, Animator animator)
    {
        this._player = player;
        this._camera = player.GetComponentInChildren<Camera>();

        this._ghost = ghost;
        this._agent = ghost.GetComponent<NavMeshAgent>();

        this._animator = animator;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entering {this.GetType()} state.");

        _agent.isStopped = true;
        _agent.ResetPath();
    }

    public virtual void Exit()
    {
        Debug.Log($"Exiting {this.GetType()} state.");
    }

    public abstract void StateUpdate();

    public virtual void StateCollisionEnter(Collision collision) {
        Debug.Log(collision);
    }
}
