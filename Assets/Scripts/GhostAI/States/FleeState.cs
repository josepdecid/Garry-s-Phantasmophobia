using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    private float timer = 0.0f;
    public FleeState(GameObject ghost, Animator animator) : base(ghost, animator) { }


    // Update is called once per frame
    public override void StateUpdate()
    {
        timer += Time.deltaTime;
        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if (timer > 2.0f)
        {
            _animator.SetBool("isNoticed", false);

        }
    }
}
