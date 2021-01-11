using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Camera displayCamera;

    private GameObject[] Targets;

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;




    // Start is called before the first frame update
    void Start()
    {
        
        displayCamera = Camera.main; 
        

        Targets = GameObject.FindGameObjectsWithTag("Target");

    }

    // Update is called once per frame
    void Update()
    {

        foreach (GameObject target in Targets)
        {
            
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(displayCamera);
            if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<BoxCollider>().bounds))
            {
                Transform target_transform = target.transform;
                Vector3 dirToTarget = (target_transform.position - transform.position).normalized;
                
                float dstToTarget = Vector3.Distance(transform.position, target_transform.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    print("Object" + target.name + "seen, make it disappear");
                    target.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    //print("The object" + target.name + "has disappeared");
                    target.GetComponent<MeshRenderer>().enabled = true;
                }

            }
            else
            {
                //print("The object" + target.name + "has disappeared");
                target.GetComponent<MeshRenderer>().enabled = true;
            }

        }
    }
}
