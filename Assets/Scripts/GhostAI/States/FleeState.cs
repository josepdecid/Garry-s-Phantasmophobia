using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class FleeState : State
{
    private NavMeshAgent __agent;
    
    private GameObject __fleeArea;
    private GameObject[] __fleeCandidates;

    public FleeState(GameObject player, GameObject ghost, Animator animator, StateParams parameters)
        : base(player, ghost, animator, parameters) { }

    public override void StateUpdate()
    {
        // Go to Search state if ghost is outside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        float distance = Vector3.Distance(_ghost.transform.position, _player.transform.position);
        if (distance < 5.0f || (!_agent.pathPending && _agent.remainingDistance < 0.5f))
        {
            Vector3 destination = GetFleePoint();
            _agent.destination = destination;
        }

        if (_parameters.isDebug) UpdateAreaInfo();
    }

    private Vector3 GetFleePoint()
    {
        Vector3 ghostPosition = _ghost.transform.position;
        Vector3 playerPosition = _player.transform.position;
        _ghost.transform.rotation = Quaternion.LookRotation(ghostPosition - playerPosition);

        float bestGoodness = -Mathf.Infinity;
        Vector3 bestDestination = _ghost.transform.position;
        NavMeshHit hit;

        for (int i = 0; i < _parameters.numSamples; ++i) 
        {
            Vector2 randomPos = Random.insideUnitCircle * _parameters.samplingRadius;
            Vector3 randomPosOffset = new Vector3(randomPos.x, 0, randomPos.y) + ghostPosition;

            // TODO: Infinity
            NavMesh.SamplePosition(randomPosOffset, out hit, _parameters.maxSamplingDistance, 1 << NavMesh.GetAreaFromName("Walkable"));
            Vector3 candidate = hit.position;

            float goodness = CalculateDestinationGoodness(candidate);
            if (goodness > bestGoodness)
            {
                bestGoodness = goodness;
                bestDestination = hit.position;
            }

            if (_parameters.isDebug) UpdateCandidateInfo(i, candidate);
        }
        
        return bestDestination;
    }

    private float CalculateDestinationGoodness(Vector3 destination)
    {
        // TODO: Improve, not always hiding behind walls when it should

        // Maximize distance from player
        float distance = Vector3.Distance(_player.transform.position, destination);
        float goodness = distance;

        // 
        if (Utils.IsDestinationHidden(_player, destination, _ghost.name, _camera.fieldOfView, distance))
        {
            goodness *= 100;
        }

        return goodness;
    }

    protected override void DrawDebugInfo()
    {
        __fleeArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        __fleeArea.transform.localScale = new Vector3(_parameters.samplingRadius, 0.01f, _parameters.samplingRadius);
        __fleeArea.GetComponent<SphereCollider>().enabled = false;
        __fleeArea.GetComponent<MeshRenderer>().material.color = Color.magenta;

        __fleeCandidates = new GameObject[_parameters.numSamples];
        for (int i = 0; i < _parameters.numSamples; ++i)
        {
            __fleeCandidates[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            __fleeCandidates[i].transform.localScale = new Vector3(0.1f, 0.01f, 0.1f);
            __fleeCandidates[i].GetComponent<SphereCollider>().enabled = false;
            __fleeCandidates[i].GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }

    protected override void DestroyDebugInfo()
    {
        Object.Destroy(__fleeArea);
        for (int i = 0; i < _parameters.numSamples; ++i) Object.Destroy(__fleeCandidates[i]);
    }

    private void UpdateAreaInfo()
    {
        Vector3 ghostPos = _ghost.transform.position;
        __fleeArea.transform.position = new Vector3(ghostPos.x, 0.01f, ghostPos.z);
    }

    private void UpdateCandidateInfo(int index, Vector3 position)
    {
        __fleeCandidates[index].transform.position = position;
    }
}
