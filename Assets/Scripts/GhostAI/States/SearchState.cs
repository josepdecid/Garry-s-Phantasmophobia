using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : State
{
    private float __timeout;

    public SearchState(GameObject player, GameObject ghost, Animator animator, float timeout) : base(player, ghost, animator)
    {
        __timeout = timeout;
    }

    public override void StateUpdate()
    {
        // Go to Flee state if ghost is inside player's FoV
        bool insideFov = Utils.IsTargetVisible(_player, _ghost, _camera.fieldOfView, Mathf.Infinity);
        _animator.SetBool("insideFoV", insideFov);

        // Go to Hide state if a spot is found
        bool nearSpot = _agent.remainingDistance < 1.0f;
        _animator.SetBool("spotFound", nearSpot);

        // Go to patrol state after a timeout if no spot is found
        __timeout -= Time.deltaTime;
        _animator.SetFloat("searchTimeout", __timeout);


        GameObject spot = GetNearestSpot();
        _agent.destination = spot.transform.position;
    }

    private GameObject GetNearestSpot()
    {
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
