using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float __timeout;

    public HideState(GameObject player, GameObject ghost, StateParams parameters)
        : base(player, ghost, parameters)
        { 
            __timeout = parameters.hideTimeout;
        }

    public override StateType StateUpdate()
    {
        if (__timeout < 0f)
            _ghostSpotMapping.UpdateSpot(_ghost.name, null);

        __timeout -= Time.deltaTime;

        return NextState();
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
}
