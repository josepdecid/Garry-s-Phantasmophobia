using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Outline : MonoBehaviour {
	[SerializeField]
	private GameObject player = null;
	
	[Header("Materials")]
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

	private static HashSet<Mesh> registeredMeshes = new HashSet<Mesh>();
	private Renderer[] renderers;
	private Camera __playerCamera;
	private GhostSpotMapping __ghostSpotMapping;
	private bool __needsUpdate;
	private bool __canInteract;

	void Awake()
	{
		renderers = GetComponentsInChildren<Renderer>();
		
		LoadSmoothNormals();

		__playerCamera = player.GetComponentInChildren<Camera>();
		__ghostSpotMapping = GhostSpotMapping.Instance;

		__needsUpdate = true;
		__canInteract = true;
	}

	void OnEnable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();
			materials.Add(outlineMaskMaterial);
			materials.Add(outlineFillMaterial);
			renderer.materials = materials.ToArray();
		}
	}

	void OnDisable()
	{
		foreach (Renderer renderer in renderers)
		{
			List<Material> materials = renderer.sharedMaterials.ToList();
			materials.Remove(outlineMaskMaterial);
			materials.Remove(outlineFillMaterial);
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

		bool canInteract = Utils.IsTargetFocused(__playerCamera.gameObject, gameObject.name, interactionDistance);
		
		if (canInteract != __canInteract)
		{
			__canInteract = canInteract;
			if (__canInteract) OnEnable();
			else OnDisable();
		}

		if (canInteract && Input.GetKeyDown(KeyCode.E))
		{
			__ghostSpotMapping.UpdateInteracted(gameObject.name);
		}
		else if (Input.GetKeyUp(KeyCode.E) && __ghostSpotMapping.GetInteractedSpot() == gameObject.name)
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
		outlineFillMaterial.SetColor("_OutlineColor", outlineColor);
		outlineMaskMaterial.SetFloat("_ZTest", (float) UnityEngine.Rendering.CompareFunction.Always);
		outlineFillMaterial.SetFloat("_ZTest", (float) UnityEngine.Rendering.CompareFunction.LessEqual);
		outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
	}
	

	// switch (outlineMode) {
	// 	case Mode.OutlineAll:
	// 	outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
	// 	outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
	// 	outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
	// 	break;

	// 	case Mode.OutlineHidden:
	// 	outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
	// 	outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
	// 	outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
	// 	break;

	// 	case Mode.OutlineAndSilhouette:
	// 	outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
	// 	outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Always);
	// 	outlineFillMaterial.SetFloat("_OutlineWidth", outlineWidth);
	// 	break;

	// 	case Mode.SilhouetteOnly:
	// 	outlineMaskMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
	// 	outlineFillMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.Greater);
	// 	outlineFillMaterial.SetFloat("_OutlineWidth", 0);
	// 	break;
	// }
	// }
}
