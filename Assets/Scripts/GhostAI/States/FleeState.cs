using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    public FleeState(GameObject player, GameObject ghost, Animator animator) : base(player, ghost, animator) { }

    public override void StateUpdate()
    {
        // Go to Search state if ghost is outside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);
    }
}
