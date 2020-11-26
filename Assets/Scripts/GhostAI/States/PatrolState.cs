using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private float __speed;
    private Vector3 __offset;
    private Rigidbody __rb;

    public PatrolState(GameObject ghost, Animator animator, float speed) : base(ghost, animator)
    {
        __speed = speed;
    }

    public override void Enter()
    {
        base.Enter();

        __rb = _ghost.GetComponent<Rigidbody>();

        Vector2 direction = new Vector2(Random.value, Random.value);
        __offset = new Vector3(direction.x, 0, direction.y);
    }

    public override void StateUpdate()
    {
        __rb.AddForce(Vector3.forward * __speed);
    }

    public override void StateCollisionEnter(Collision collision)
    {
        Vector2 direction = new Vector2(Random.value, Random.value);
        __offset = new Vector3(direction.x, 0, direction.y);
    }
}
