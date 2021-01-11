using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : State
{
    private float __speed;
    private Vector3 __goalPosition;
    private UnityEngine.AI.NavMeshAgent __agent;

    public RoamState(GameObject player, GameObject ghost, StateParams parameters)
        : base(player, ghost, parameters) { }

    public override void Enter()
    {
        base.Enter();

        _agent.autoBraking = false;
        __goalPosition = GenerateRandomPoint(5);
    }

    public override StateType StateUpdate()
    {
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            __goalPosition = GenerateRandomPoint(5);
            _agent.SetDestination(__goalPosition);
        }

        return NextState();
    }

    protected override StateType NextState()
    {
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        if (insideFov)
            return StateType.Flee;

        // Keep the same state
        return StateType.Roam;
    }

    private Vector3 GenerateRandomPoint(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += _ghost.transform.position;
        return randomDirection;
    }
}
