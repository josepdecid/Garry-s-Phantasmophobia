using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float __timeout;
    private MeshRenderer __renderer;
    private BoxCollider __collider;

    public HideState(GameObject player, GameObject ghost, StateParams parameters)
        : base(player, ghost, parameters)
        { 
            __timeout = parameters.hideTimeout;
            __renderer = ghost.GetComponent<MeshRenderer>();
            __collider = ghost.GetComponent<BoxCollider>();
        }

    public override void Enter()
    {
        SetVisibility(false);
    }

    public override StateType StateUpdate()
    {
        __timeout -= Time.deltaTime;
        
        if (__timeout < 0f)
        {
            // _ghostSpotMapping.RemoveSpot(_ghost.name);
        }

        StateType nextState = NextState();
        if (nextState != StateType.Hide)
            SetVisibility(true);

        return nextState;
    }

    protected override StateType NextState()
    {
        // Go to Roam state after a timeout
        if (__timeout < 0f)
            return StateType.Roam;

        // Go to Flee state if player interacts with hiding spot
        if (PlayerIsInteracting())
            return StateType.Flee;

        // Keep the same state
        return StateType.Hide;
    }

    public override void Exit()
    {
        base.Exit();
        __timeout = _parameters.hideTimeout;
    }

    private bool PlayerIsInteracting()
    {   
        string interactedSpot = _ghostSpotMapping.GetInteractedSpot();
        string ghostSpot = _ghostSpotMapping.GetSpot(_ghost.name);
        Debug.Log($"{ghostSpot} - {interactedSpot}");
        return interactedSpot == ghostSpot && interactedSpot != null;
    }

    private void SetVisibility(bool visible)
    {
        __renderer.enabled = visible;
        __collider.enabled = visible;
    }
}
