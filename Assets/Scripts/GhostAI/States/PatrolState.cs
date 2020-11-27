using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    private float __speed;
    private Vector3 __goalPosition;
    private UnityEngine.AI.NavMeshAgent __agent;

    public PatrolState(GameObject player, GameObject ghost, Animator animator, float speed) : base(player, ghost, animator)
    {
        __speed = speed;
        __agent = _ghost.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    public override void Enter()
    {
        base.Enter();

        __agent.autoBraking = false;
        __goalPosition = GenerateRandomPoint(5);
    }

    public override void StateUpdate()
    {
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        if (!__agent.pathPending && __agent.remainingDistance < 0.5f)
        {
            __goalPosition = GenerateRandomPoint(5);
            __agent.destination = __goalPosition;
        }
    }

    private Vector3 GenerateRandomPoint(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += _ghost.transform.position;
        return randomDirection;
    }
}
