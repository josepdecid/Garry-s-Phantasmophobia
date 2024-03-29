using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SearchState : State
{
    private float __timeout;
    private bool __isNearSpot;

    public SearchState(GameObject player, GameObject ghost, StateParams parameters)
        : base(player, ghost, parameters)
        {
            __timeout = parameters.searchTimeout;
            __isNearSpot = false;
        }

    public override StateType StateUpdate()
    {
        (GameObject spot, float minDist, Vector3 sampledPos) = GetNearestAvailableSpot();
        if (spot != null) {
            _agent.SetDestination(sampledPos);

            __isNearSpot = !_agent.pathPending || minDist <= 2.0f;
            if (__isNearSpot) _ghostSpotMapping.UpdateSpot(_ghost.name, spot.GetInstanceID().ToString());
        }
        
        __timeout -= Time.deltaTime;

        return NextState();
    }

    protected override StateType NextState()
    {
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        if (insideFov)
            return StateType.Flee;

        // Go to Hide state if a spot is found
        if (__isNearSpot)
            return StateType.Hide;

        // Go to Roam state after a timeout if no spot is found
        if (__timeout < 0f)
            return StateType.Roam;

        // Keep the same state
        return StateType.Search;
    }

    public override void Exit()
    {
        base.Exit();
        __timeout = _parameters.searchTimeout;
    }

    private (GameObject, float, Vector3) GetNearestAvailableSpot()
    {
        GameObject[] spots = GameObject.FindGameObjectsWithTag("Prop");
        if (spots.GetLength(0) == 0) Debug.LogWarning("There are no hiding spots!");
        
        float minDist = Mathf.Infinity;
        int minIdx = -1;
        Vector3 minSampledPos = new Vector3();
        _agent.isStopped = true;
        for (int i = 0; i < spots.GetLength(0); ++i)
        {
            NavMeshPath path = new NavMeshPath();
            NavMeshHit hit;
            float yAxisPosition = spots[i].transform.position.y;
            Vector3 spotPosition;
            if (yAxisPosition < 4.0f)
                spotPosition = new Vector3(spots[i].transform.position.x, 0, spots[i].transform.position.z);
            else
                spotPosition = new Vector3(spots[i].transform.position.x, 4.0f, spots[i].transform.position.z);
            NavMesh.SamplePosition(spotPosition, out hit, 2.0f, NavMesh.AllAreas);
            if (hit.position.x != Mathf.Infinity && NavMesh.CalculatePath(_ghost.transform.position, hit.position, NavMesh.AllAreas, path)){
                _agent.SetPath(path);
                float dist = _agent.remainingDistance;
                bool spotFree = _ghostSpotMapping.GetGhost(spots[i].GetInstanceID().ToString()) == null;
                if (dist < minDist && spotFree)
                {
                    minIdx = i;
                    minDist = dist;
                    minSampledPos = hit.position;
                }
            }
        }
        _agent.isStopped = false;

        if (minIdx != -1) {
            return (spots[minIdx], minDist, minSampledPos);
        }
        else {
            return (null, minDist, minSampledPos);
        }
        
    }

    protected override void DrawDebugInfo()
    {
        // __targetArea = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // __targetArea.transform.localScale = new Vector3(2 * _parameters.samplingRadius, 0.01f, 2 * _parameters.samplingRadius);
        // __targetArea.GetComponent<SphereCollider>().enabled = false;
        // __targetArea.GetComponent<MeshRenderer>().material.color = Color.grey;
    }

    private void UpdateTargetInfo()
    {
    }
}
