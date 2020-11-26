using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    public FleeState(GameObject ghost, Animator animator) : base(ghost, animator) { }

    public override void StateUpdate()
    {
        // Go to Search state if ghost is outside player's FoV
        bool insideFov = Utils.IsTargetVisible(Camera.main, _ghost);
        _animator.SetBool("insideFoV", insideFov);
    }
}
