using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float __timeout;

    public HideState(GameObject ghost, Animator animator, float timeout):base(ghost, animator)
    {
        __timeout = timeout;
    }

    public override void StateUpdate()
    {
        // Check if enters inside player's FoV

        __timeout += Time.deltaTime;
        _animator.SetFloat("hideTimeout", __timeout);
    }
}
