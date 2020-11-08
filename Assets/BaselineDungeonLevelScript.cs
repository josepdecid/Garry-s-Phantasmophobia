using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaselineDungeonLevelScript : MonoBehaviour
{
    public int sizeX;
    public int sizeZ;

    // Storage for floor tiles
    public Dictionary<string,GameObject> planes = new Dictionary<string,GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Remove all generated objects
    public void CleanUpFloor() {
        // Cleanup according to saved planes on runtime
        foreach (var key in planes.Keys)
        {
            GameObject plane = planes[key];
            GameObject.DestroyImmediate(plane);
        }

        planes.Clear();

        // Cleanup all possibly existing other planes
        foreach(Transform child in GetComponentsInChildren<Transform>()) {
            if(child.gameObject.tag == "Floor") {
                // Debug.Log(child.gameObject.name);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    // Cleanup all objects
    public void CleanUp() {
        // Cleanup floor
        // CleanUpFloor();
    
        // Cleanup all possibly existing other planes
        foreach(Transform child in GetComponentsInChildren<Transform>()) {
            if(child != transform){
                // Debug.Log(child.gameObject.name);
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    // Build dungeon's matrix floor
    public void BuildDungeonFloor() {
        
        // First cleanup 
        CleanUpFloor();

        /// Spawn multiple planes in x-z plane
        for(int x = 0; x < sizeX; x++) {
            for(int z = 0; z < sizeZ; z++) {
                // Create plane with tag
                GameObject plane  = GameObject.CreatePrimitive(PrimitiveType.Plane);
                string label = "Plane_" + x.ToString() + "_" + z.ToString();
                plane.name = label;
                plane.tag = "Floor";

                // Set parent, scale and position
                plane.transform.SetParent(transform);
                plane.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                plane.transform.position = new Vector3(2.5f + x * 5, 0, 2.5f + z * 5);

                // Add to list for tracking
                planes.Add(label, plane);
            }
        }

    }
}
