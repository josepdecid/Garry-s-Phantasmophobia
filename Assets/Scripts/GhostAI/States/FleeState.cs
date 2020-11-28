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
            Vector3 destination = GetFleePoint(10, 10);
            _agent.destination = destination;
        }
    }

    private Vector3 GetFleePoint(int numSamples, int radius)
    {
        Vector3 ghostPosition = _ghost.transform.position;
        Vector3 playerPosition = _player.transform.position;
        _ghost.transform.rotation = Quaternion.LookRotation(ghostPosition - playerPosition);

        float bestGoodness = -Mathf.Infinity;
        Vector3 bestDestination = _ghost.transform.position;
        NavMeshHit hit;

        for (int i = 0; i < numSamples; ++i) 
        {
            Vector3 randomPos = Random.insideUnitSphere * radius + ghostPosition;

            NavMesh.SamplePosition(randomPos, out hit, radius, 1 << NavMesh.GetAreaFromName("Walkable"));
            float goodness = CalculateDestinationGoodness(hit.position);
            if (goodness > bestGoodness)
            {
                bestGoodness = goodness;
                bestDestination = hit.position;
            }
        }
        
        return bestDestination;
    }

    private float CalculateDestinationGoodness(Vector3 destination)
    {
        // Maximize distance from player
        float distance = Vector3.Distance(_player.transform.position, destination);
        float goodness = distance;
        Debug.Log(destination);

        // 
        if (Utils.IsDestinationHidden(_player, destination, _ghost.name, 60, distance))
        {
            goodness += 100;
        }

        return goodness;
    }
}
