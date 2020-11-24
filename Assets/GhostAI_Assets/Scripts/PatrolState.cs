using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private float timer = 0.0f;

    public PatrolState(Animator animator) : base(animator) { }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
     public override void StateUpdate()
    {
        timer += Time.deltaTime;
        // Check if we have reached beyond 2 seconds.
        // Subtracting two is more accurate over time than resetting to zero.
        if(timer > 2.0f)
        {
            animator.SetBool("isNoticed", true);
            
        }
    }
}
