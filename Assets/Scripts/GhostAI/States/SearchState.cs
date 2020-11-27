using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : State
{
    private float __timeout;

    public SearchState(GameObject player, GameObject ghost, Animator animator, float timeout) : base(player, ghost, animator)
    {
        __timeout = timeout;
    }

    public override void StateUpdate()
    {
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        // TODO: Go to Hide state if a spot is found

        // Go to patrol state after a timeout if no spot is found
        __timeout -= Time.deltaTime;
        _animator.SetFloat("searchTimeout", __timeout);
    }
}
