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

    void Awake()
    {
        SetupNavMeshSurfaces();
        SetupNavMeshLinks();
        SetupPlayer();
        SetupProps();
        SetupNPCs();
    }

    private void SetupNavMeshSurfaces()
    {
        GameObject[] walkable = GameObject.FindGameObjectsWithTag("Floor");
        Debug.Log(walkable);

        GameObject[] props = GameObject.FindGameObjectsWithTag("Prop");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        GameObject[] nonWalkable = props.Concat(walls).ToArray();

        foreach (GameObject floor in walkable)
        {
            NavMeshSurface surface = floor.AddComponent<NavMeshSurface>();
            // We need this for NavMeshLink to work
            surface.tileSize = 64;
            surface.BuildNavMesh();
        }
    }

    private void SetupNavMeshLinks()
    {
        // TODO: Implement joining doors.
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
        GameObject[] ghosts = new GameObject[numberOfGhosts];
        for (int i = 0; i < numberOfGhosts; ++i)
        {
            ghosts[i] = Instantiate(ghostPrefab, new Vector3(10, 2, 10), Quaternion.identity);
        }
    }
}
