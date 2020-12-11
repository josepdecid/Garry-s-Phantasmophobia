using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine;

public class Setup : MonoBehaviour
{
    [SerializeField]
	private GameObject playerPrefab = null;
    [SerializeField]
	private GameObject ghostPrefab = null;
    [SerializeField]
    private int numberOfGhosts = 1;
    [SerializeField]
    private GameObject floor = null;
	
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
        SetupNavMeshSurfaces();
        SetupPlayer();
        SetupProps();
        SetupNPCs();
    }

    private void SetupNavMeshSurfaces()
    {
        GameObject[] walkableSurfaces = GameObject.FindGameObjectsWithTag("Floor");
        CombineInstance[] objectsToCombine = new CombineInstance[walkableSurfaces.Length];

        for (int i = 0; i < walkableSurfaces.Length; ++i)
        {
            MeshFilter meshFilter = walkableSurfaces[i].GetComponent<MeshFilter>();

            // Extract mesh from each walkable surface (floor)
            objectsToCombine[i].mesh = meshFilter.sharedMesh;
            objectsToCombine[i].transform = meshFilter.transform.localToWorldMatrix;
            
            // Disable original single component
            walkableSurfaces[i].gameObject.SetActive(false);
        }

        // New GameObject to merge the floors
        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);

        // Combine all meshes and set as the new mesh filter
        floor.GetComponent<MeshFilter>().mesh = new Mesh();
        floor.GetComponent<MeshFilter>().mesh.CombineMeshes(objectsToCombine);

        // Update collider to fit all sub-components
        MeshCollider meshCollider = floor.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = floor.GetComponent<MeshFilter>().mesh;
        
        // Build dynamic NavMesh
        __navMeshSurface = floor.AddComponent<NavMeshSurface>();
        __navMeshSurface.BuildNavMesh();
    }

    private void SetupPlayer()
    {
        __player = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
    }

    private void SetupProps()
    {
        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        foreach (GameObject prop in props)
        {
            prop.SetActive(false);

            Outline outline = prop.AddComponent<Outline>();
            outline.player = __player;
            outline.outlineMaskMaterial = outlineMaskMaterial;
            outline.outlineFillMaterial = outlineFillMaterial;
            outline.outlineColor = outlineColor;
            outline.outlineWidth = outlineWidth;
            outline.interactionDistance = interactionDistance;

            prop.SetActive(true);
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
        }
    }
}
