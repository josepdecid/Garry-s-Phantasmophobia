using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurePropObjects : MonoBehaviour
{
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

    void Awake()
    {
        GameObject[] props;
        props = GameObject.FindGameObjectsWithTag("Prop");
        foreach (GameObject prop in props)
        {
            prop.SetActive(false);

            Outline outline = prop.AddComponent<Outline>();
            outline.player = player;
            outline.outlineMaskMaterial = outlineMaskMaterial;
            outline.outlineFillMaterial = outlineFillMaterial;
            outline.outlineColor = outlineColor;
            outline.outlineWidth = outlineWidth;
            outline.interactionDistance = interactionDistance;

            prop.SetActive(true);
        }
    }
}
