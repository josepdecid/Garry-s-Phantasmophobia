using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    private float timer = 0.0f;
    public  HideState(Animator animator):base(animator) {} 
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void StateUpdate()
    {
        timer += Time.deltaTime;
        animator.SetFloat("timeCounter", timer);

    }
}
