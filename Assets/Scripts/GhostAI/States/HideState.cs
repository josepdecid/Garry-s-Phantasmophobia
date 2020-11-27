using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float __timeout;

    public HideState(GameObject player, GameObject ghost, Animator animator, float timeout):base(player, ghost, animator)
    {
        __timeout = timeout;
    }

    public override void StateUpdate()
    {
        // Go to Patrol state after a timeout
        __timeout -= Time.deltaTime;
        _animator.SetFloat("hideTimeout", __timeout);

        // Go to Flee state if inside FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);
    }
}
