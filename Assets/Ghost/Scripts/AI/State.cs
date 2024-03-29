﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

abstract public class State
{
    protected GameObject _player;
    protected Camera _camera;
    
    protected GameObject _ghost;
    protected NavMeshAgent _agent;

    protected GhostSpotMapping _ghostSpotMapping;
    
    protected StateParams _parameters = null;
    
    public State(GameObject player, GameObject ghost, StateParams parameters)
    {
        this._player = player;
        this._camera = player.GetComponentInChildren<Camera>();

        this._ghost = ghost;
        this._agent = ghost.GetComponent<NavMeshAgent>();

        this._ghostSpotMapping = GhostSpotMapping.Instance;

        this._parameters = parameters;
    }

    public virtual void Enter()
    {
        // Debug.Log($"Entering {this.GetType()} state.");
        _agent.isStopped = true;
        _agent.ResetPath();

        UpdateAgentProperties();
        if (_parameters.isDebug) DrawDebugInfo();
    }

    public virtual void Exit()
    {
        // Debug.Log($"Exiting {this.GetType()} state.");
        if (_parameters.isDebug) DestroyDebugInfo();
    }

    public abstract StateType StateUpdate();

    protected abstract StateType NextState();

    protected virtual void UpdateAgentProperties() {}

    protected virtual void DrawDebugInfo() {}

    protected virtual void DestroyDebugInfo() {}
}
