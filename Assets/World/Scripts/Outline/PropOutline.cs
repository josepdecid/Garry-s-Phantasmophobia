using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PropOutline : MonoBehaviour {
	public GameObject player;
	public Material outlineMaskMaterial;
	public Material outlineFillMaterial;
	public Color outlineColor;
	public Color debugOutlineColor;
	public float outlineWidth;
	public float interactionDistance;

	private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
	private Renderer[] renderers;
	private Camera __playerCamera;
	private GhostSpotMapping __ghostSpotMapping;
	private bool __needsUpdate;
	private bool __canInteract;

	private Material __outlineMaskMaterial;
	private Material __outlineFillMaterial;

	void Awake()
	{
		renderers = GetComponentsInChildren<Renderer>();
		
		LoadSmoothNormals();

		__playerCamera = player.GetComponentInChildren<Camera>();
		__ghostSpotMapping = GhostSpotMapping.Instance;

		__needsUpdate = true;
		__canInteract = true;

		__outlineMaskMaterial = new Material(outlineMaskMaterial);
		__outlineFillMaterial = new Material(outlineFillMaterial);
	}

	void OnEnable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();

			materials.Add(__outlineMaskMaterial);
			materials.Add(__outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

	void OnDisable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();
			materials.Remove(__outlineMaskMaterial);
			materials.Remove(__outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

	void Update()
	{	
		if (__needsUpdate)
		{
			__needsUpdate = false;
			UpdateMaterialProperties();
		}

		bool canInteract = Utils.IsTargetFocused(__playerCamera.gameObject, gameObject.GetInstanceID().ToString(), interactionDistance);
		
		if (canInteract != __canInteract)
		{
			__canInteract = canInteract;
			if (__canInteract) OnEnable();
			else OnDisable();
		}

		if (canInteract && Input.GetMouseButtonUp(0))
		{
			__ghostSpotMapping.UpdateInteracted(gameObject.GetInstanceID().ToString());
		}
		else if (Input.GetMouseButtonDown(0) && __ghostSpotMapping.GetInteractedSpot() == gameObject.GetInstanceID().ToString())
		{
			__ghostSpotMapping.UpdateInteracted(null);
		}

	}

	void LoadSmoothNormals()
	{
		foreach (MeshFilter meshFilter in GetComponentsInChildren<MeshFilter>())
		{
			// Skip if smooth normals have already been adopted
			if (!registeredMeshes.Add(meshFilter.sharedMesh)) continue;

			List<Vector3> smoothNormals = SmoothNormals(meshFilter.sharedMesh);
			meshFilter.sharedMesh.SetUVs(3, smoothNormals);
		}

		// Clear UV3 on skinned mesh renderers
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			if (registeredMeshes.Add(skinnedMeshRenderer.sharedMesh))
				skinnedMeshRenderer.sharedMesh.uv4 = new Vector2[skinnedMeshRenderer.sharedMesh.vertexCount];
		}
	}

	List<Vector3> SmoothNormals(Mesh mesh)
	{
		// Group vertices by location
		IEnumerable<IGrouping<Vector3, KeyValuePair<Vector3, int>>> groups;
		groups = mesh.vertices.Select((vertex, index) => new KeyValuePair<Vector3, int>(vertex, index)).GroupBy(pair => pair.Key);

		List<Vector3> smoothNormals = new List<Vector3>(mesh.normals);

		// Average normals for grouped vertices
		foreach (IGrouping<Vector3, KeyValuePair<Vector3, int>> group in groups) {
			// Skip single vertices
			if (group.Count() == 1) continue;
			// Calculate the average normal
			Vector3 smoothNormal = Vector3.zero;

			foreach (KeyValuePair<Vector3, int> pair in group)
				smoothNormal += mesh.normals[pair.Value];
			smoothNormal.Normalize();

			// Assign smooth normal to each vertex
			foreach (KeyValuePair<Vector3, int> pair in group)
				smoothNormals[pair.Value] = smoothNormal;
		}

		return smoothNormals;
	}

	void UpdateMaterialProperties()
	{
		__outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
		__outlineMaskMaterial.SetFloat("_ZTest", (float) UnityEngine.Rendering.CompareFunction.Always);
		__outlineFillMaterial.SetFloat("_ZTest", (float) UnityEngine.Rendering.CompareFunction.LessEqual);
		__outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
	}
}
