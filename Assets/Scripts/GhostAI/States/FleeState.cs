using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class FleeState : State
{
    private UnityEngine.AI.NavMeshAgent __agent;

    public FleeState(GameObject player, GameObject ghost, Animator animator) : base(player, ghost, animator) { }

    public override void StateUpdate()
    {
        // Go to Search state if ghost is outside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        float distance = Vector3.Distance(_ghost.transform.position, _player.transform.position);
        if (distance < 5.0f || (!_agent.pathPending && _agent.remainingDistance < 0.5f))
        {
            Flee();
        }
    }

    // TODO: Strange behavior when trapped in a corner
    // Instead of going always towards the opposite direction, use an "heuristic" to go to more "covered" places.
    private void Flee()
    {
        Vector3 ghostPosition = _ghost.transform.position;
        Vector3 playerPosition = _player.transform.position;
        _ghost.transform.rotation = Quaternion.LookRotation(ghostPosition - playerPosition);
 
        Vector3 runTo = ghostPosition + _ghost.transform.forward * 2;
        
        NavMeshHit hit;
        NavMesh.SamplePosition(runTo, out hit, 5, 1 << NavMesh.GetAreaFromName("Walkable"));
        
        _agent.destination = hit.position;
    }
}
