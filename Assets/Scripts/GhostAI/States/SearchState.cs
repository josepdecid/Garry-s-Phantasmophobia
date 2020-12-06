using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : State
{
    private float __timeout;
    private GameObject __spot;

    public SearchState(GameObject player, GameObject ghost, Animator animator, StateParams parameters)
        : base(player, ghost, animator, parameters)
        {
            __timeout = parameters.searchTimeout;
        }

    public override void StateUpdate()
    {
        __spot = GetNearestAvailableSpot();
        _agent.destination = __spot.transform.position;

        bool nearSpot = _agent.remainingDistance < 2.0f;
        if (nearSpot) _ghostSpotMapping.UpdateSpot(_ghost.name, __spot.name);

        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        // Go to Hide state if a spot is found
        _animator.SetBool("spotFound", nearSpot);

        // Go to patrol state after a timeout if no spot is found
        __timeout -= Time.deltaTime;
        _animator.SetFloat("searchTimeout", __timeout);
    }

    public override void Exit()
    {
        base.Exit();
        __timeout = _parameters.searchTimeout;
    }

    private GameObject GetNearestAvailableSpot()
    {
        // TODO: Filter out already occupied spots

        GameObject[] spots = GameObject.FindGameObjectsWithTag("Prop");
        if (spots.GetLength(0) == 0) Debug.LogWarning("There are no hiding spots!");
        
        float minDist = Vector3.Distance(spots[0].transform.position, _ghost.transform.position);
        int minIdx = 0;

        for (int i = 1; i < spots.GetLength(0); ++i)
        {
            float dist = Vector3.Distance(spots[i].transform.position, _ghost.transform.position);
            if (dist < minDist)
            {
                minIdx = i;
                minDist = dist;
            }
        }

        return spots[minIdx];
    }
}
