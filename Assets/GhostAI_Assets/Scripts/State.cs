using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class State : MonoBehaviour
{
    protected Animator animator;
    public State(Animator animator)
    {
        this.animator = animator;
    }

    public virtual void Enter()
    {
        
        Debug.Log("State type: " + this.GetType());
    }

    public virtual void HandleInput()
    {

    }

    public virtual void StateUpdate()
    {

    }

    public virtual void Exit()
    {

    }

}
