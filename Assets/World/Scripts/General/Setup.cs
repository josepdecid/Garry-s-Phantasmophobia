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
	
	[Header("Outline Materials")]
	[SerializeField]
	private Material outlineMaskMaterial = null;
	[SerializeField]
	private Material outlineFillMaterial = null;

	[Header("Visualization Parameters")]
	[SerializeField]
	private Color outlineColor = Color.white;
    [SerializeField]
	private Color outlineColorPowerUp = Color.white;
	[SerializeField, Range(0f, 20f)]
	private float outlineWidth = 1.0f;
	[SerializeField]
	private float interactionDistance = 1.0f;

    private GameObject __player;
    private NavMeshSurface __navMeshSurface;

    private int __numberOfGhosts, __temptativeSize, __maxLockedRooms;
    private bool __multiFloor;

    void Awake()
    {
        SetupDifficulty();
        SetupMapGeneration();
        SetupNavMeshSurfaces();
        SetupPlayer();
        SetupProps();
        SetupNPCs();
        SetupGUI();
    }

    private void SetupMapGeneration() 
    {
        generator.SetupGenerator(__temptativeSize, __maxLockedRooms, __multiFloor);
        generator.GenerateMap();
    }

    private void SetupNavMeshSurfaces()
    {
        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
        foreach(GameObject go in allObjects){
            // Override NavMesh for no-floor gameobjects
            Component[] components = go.GetComponentsInChildren<Transform>().Where(r => r.tag == "Floor").ToArray();
            if (go.tag != "Floor" && components.Length == 0 ){
                NavMeshModifier navMeshModifier = go.AddComponent<NavMeshModifier>();
                navMeshModifier.overrideArea = true;
                navMeshModifier.area = 1;
            }
            
            // Disable door gameobjects
            if (go.tag == "InteractiveObject") {
                go.SetActive(false);
            }
        }

        GameObject[] walkableSurfaces = GameObject.FindGameObjectsWithTag("Floor");
        CombineInstance[] objectsToCombine = new CombineInstance[walkableSurfaces.Length];

        for (int i = 0; i < walkableSurfaces.Length; ++i)
        {
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

        // Enable again door gameobjects
        foreach(GameObject go in allObjects.Where(r => r.tag == "InteractiveObject").ToArray()){
            go.SetActive(true);
        }
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
        string[] tagsToSetup = {"Prop", "Key", "InteractiveObject", "PowerUp"};
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
                }
                else if (tag == "InteractiveObject"){
                    outline.outlineColor = prop.GetComponent<KeyDoorController>().GetOutlineColor();
                }
                else if (tag == "PowerUp") {
                    outline.outlineColor = outlineColorPowerUp;
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
        GameObject[] ghosts = new GameObject[__numberOfGhosts];

        for (int i = 0; i < __numberOfGhosts; ++i)
        {
            Vector3 spawnPosition = navMeshSampler.GetRandomPointOnNavMesh();
            ghosts[i] = Instantiate(ghostPrefab, spawnPosition, Quaternion.identity);
            ghosts[i].name = $"Ghost {i}";
        }
    }

    private void SetupGUI()
    {
        TMP_Text numGhosts = GameObject.Find("GhostCounter").GetComponent<TMP_Text>();
        numGhosts.text = $"x {__numberOfGhosts}";
    }

    private void SetupDifficulty()
    {
        __multiFloor = GameDifficulty.Difficulty >= 5;
        __numberOfGhosts = GetValueByDifficulty(3, 15);
        __temptativeSize = GetValueByDifficulty(8, 60);
        __maxLockedRooms = GetValueByDifficulty(0, 16);
        if (__multiFloor) {
            __temptativeSize = (int) __temptativeSize/2;
            __maxLockedRooms = (int) __maxLockedRooms/2;
        }
    }

    private int GetValueByDifficulty(int min, int max) {
        int minDiff = 1;
        int maxDiff = 10;
        int diff = GameDifficulty.Difficulty;

        return (int) min + (max - min) * (diff-minDiff)/(maxDiff-minDiff);
    }
}
