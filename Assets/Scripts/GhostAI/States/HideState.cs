using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float __timeout;

    public HideState(GameObject player, GameObject ghost, Animator animator, StateParams parameters)
        : base(player, ghost, animator, parameters) { }

    public override void StateUpdate()
    {
        // Go to Patrol state after a timeout
        __timeout -= Time.deltaTime;
        _animator.SetFloat("hideTimeout", _parameters.hideTimeout);

        // Go to Flee state if player interacts with hiding spot
        // bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        bool isInteracting = PlayerIsInteracting();
        _animator.SetBool("playerInteraction", isInteracting);

        if (__timeout <= 0) _ghostSpotMapping.UpdateSpot(_ghost.name, null);
    }

    private bool PlayerIsInteracting()
    {   
        string interactedSpot = _ghostSpotMapping.GetInteractedSpot();
        string ghostSpot = _ghostSpotMapping.GetSpot(_ghost.name);
        Debug.Log($"{ghostSpot} - {interactedSpot}");
        return interactedSpot == ghostSpot;
    }
}
