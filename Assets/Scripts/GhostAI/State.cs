using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class State
{
    protected GameObject _player;
    protected Camera _camera;
    protected GameObject _ghost;
    protected Animator _animator;
    
    public State(GameObject player, GameObject ghost, Animator animator)
    {
        this._player = player;
        this._camera = player.GetComponentInChildren<Camera>();

        this._ghost = ghost;
        this._animator = animator;
    }

    public virtual void Enter()
    {
        Debug.Log($"Entering {this.GetType()} state.");
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
