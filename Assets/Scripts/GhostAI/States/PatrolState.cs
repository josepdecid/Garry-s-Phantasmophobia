using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private float __speed;
    private Vector3 __offset;
    private Rigidbody __rb;

    public PatrolState(GameObject player, GameObject ghost, Animator animator, float speed) : base(player, ghost, animator)
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
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        __rb.AddForce(Vector3.forward * __speed);
    }

    public override void StateCollisionEnter(Collision collision)
    {
        Vector2 direction = new Vector2(Random.value, Random.value);
        __offset = new Vector3(direction.x, 0, direction.y);
    }
}
