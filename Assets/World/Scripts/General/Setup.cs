using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Setup : MonoBehaviour
{
    [SerializeField]
    private Generator generator = null;
    [SerializeField]
	private GameObject playerPrefab = null;
    [SerializeField]
	private GameObject ghostPrefab = null;
    [SerializeField]
    private int numberOfGhosts = 1;
	
	[Header("Outline Materials")]
	[SerializeField]
	private Material outlineMaskMaterial = null;
	[SerializeField]
	private Material outlineFillMaterial = null;

	[Header("Visualization Parameters")]
	[SerializeField]
	private Color outlineColor = Color.white;
	[SerializeField, Range(0f, 20f)]
	private float outlineWidth = 1.0f;
	[SerializeField]
	private float interactionDistance = 1.0f;

    private GameObject __player;
    private NavMeshSurface __navMeshSurface;

    void Awake()
    {
        SetupMapGeneration();
        SetupNavMeshSurfaces();
        SetupPlayer();
        SetupProps();
        SetupNPCs();
        SetupGUI();
    }

    private void SetupMapGeneration() 
    {
        generator.GenerateMap();
    }

    private void SetupNavMeshSurfaces()
    {
        
        // Override NavMesh
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects){
            Component[] components = go.GetComponentsInChildren<Transform>().Where(r => r.tag == "Floor").ToArray();
            if (go.tag != "Floor" && components.Length == 0){
                NavMeshModifier navMeshModifier = go.AddComponent<NavMeshModifier>();
                navMeshModifier.overrideArea = true;
                navMeshModifier.area = 1;
            }
        }

        GameObject[] floorSurfaces = GameObject.FindGameObjectsWithTag("Floor");
        //GameObject[] doorSurfaces = GameObject.FindGameObjectsWithTag("InteractiveObject");
        //GameObject[] walkableSurfaces = floorSurfaces.Union(doorSurfaces).ToArray();
        GameObject[] walkableSurfaces = floorSurfaces;
        Debug.Log("floorSurfaces len: " + floorSurfaces.Length);
        //Debug.Log("doorSurfaces len: " + doorSurfaces.Length);
        Debug.Log("union len: " + walkableSurfaces.Length);
        CombineInstance[] objectsToCombine = new CombineInstance[walkableSurfaces.Length];

        for (int i = 0; i < walkableSurfaces.Length; ++i)
        {
            Debug.Log("I ENTER HERE");
            MeshFilter meshFilter = walkableSurfaces[i].GetComponent<MeshFilter>();

            // Extract mesh from each walkable surface (floor)
            objectsToCombine[i].mesh = meshFilter.sharedMesh;
            objectsToCombine[i].transform = meshFilter.transform.localToWorldMatrix;
            
            // Disable original single component
            // walkableSurfaces[i].gameObject.SetActive(false);
            walkableSurfaces[i].gameObject.GetComponent<MeshCollider>().enabled = false;
        }

        // New GameObject to merge the floors
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        Debug.Log("objectsToCombine: " + objectsToCombine.Length);
        // Combine all meshes and set as the new mesh filter
        floor.GetComponent<MeshFilter>().mesh = new Mesh();
        floor.GetComponent<MeshFilter>().mesh.CombineMeshes(objectsToCombine);

        // Update collider to fit all sub-components
        MeshCollider meshCollider = floor.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = floor.GetComponent<MeshFilter>().mesh;
        
        // Get Ghost AgentType ID to build the NavMesh with that parameters
        int count = NavMesh.GetSettingsCount();
        int agentTypeID = -1;
        for (int i = 0; i < count && agentTypeID == -1; ++i)
        {
            int id = NavMesh.GetSettingsByIndex(i).agentTypeID;
            string name = NavMesh.GetSettingsNameFromID(id);
            if (name == "Ghost")
                agentTypeID = id;
        }

        floor.GetComponent<MeshRenderer>().enabled = false;

        // Build dynamic NavMesh
        __navMeshSurface = floor.AddComponent<NavMeshSurface>();
        __navMeshSurface.agentTypeID = agentTypeID;
        __navMeshSurface.BuildNavMesh();
    }

    private void SetupPlayer()
    {
        RandomNavMeshPoint navMeshSampler = gameObject.AddComponent<RandomNavMeshPoint>();
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("Spawn");
        NavMeshHit hit;
        NavMesh.SamplePosition(spawnPoint.transform.position, out hit, 0.5f, NavMesh.AllAreas);
        __player = Instantiate(playerPrefab, hit.position, Quaternion.identity);
    }

    private void SetupProps()
    {
        string[] tagsToSetup = {"Prop", "Key", "InteractiveObject"};
        foreach(string tag in tagsToSetup) {
            GameObject[] props = GameObject.FindGameObjectsWithTag(tag);
            foreach (GameObject prop in props)
            {
                if (tag == "InteractiveObject" && prop.GetComponent<KeyDoorController>().IsUnlocked()){
                    continue;
                }

                prop.SetActive(false);

                PropOutline outline = prop.AddComponent<PropOutline>();
                outline.player = __player;
                outline.outlineMaskMaterial = outlineMaskMaterial;
                outline.outlineFillMaterial = outlineFillMaterial;

                if (tag == "Key"){
                    outline.outlineColor = prop.GetComponent<Key>().GetOutlineColor();
                    Debug.Log("Key");
                    Debug.Log(outline.outlineColor);
                }
                else if (tag == "InteractiveObject"){
                    outline.outlineColor = prop.GetComponent<KeyDoorController>().GetOutlineColor();
                    Debug.Log("Door");
                    Debug.Log(outline.outlineColor);
                }
                else {
                    outline.outlineColor = outlineColor;
                }

                outline.outlineWidth = outlineWidth;
                outline.interactionDistance = interactionDistance;

                prop.SetActive(true);
            }
        }
    }

    private void SetupNPCs()
    {
        RandomNavMeshPoint navMeshSampler = gameObject.AddComponent<RandomNavMeshPoint>();
        GameObject[] ghosts = new GameObject[numberOfGhosts];

        for (int i = 0; i < numberOfGhosts; ++i)
        {
            Vector3 spawnPosition = navMeshSampler.GetRandomPointOnNavMesh();
            ghosts[i] = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
            ghosts[i].name = $"Ghost {i}";
        }
    }

    private void SetupGUI()
    {
        TMP_Text numGhosts = GameObject.Find("GhostCounter").GetComponent<TMP_Text>();
        numGhosts.text = $"x {numberOfGhosts}";
    }
}
