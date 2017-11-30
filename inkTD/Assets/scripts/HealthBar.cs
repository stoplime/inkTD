using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthAligns
{
	Center = 0,
	Left = 1,
	Right = 2
}

public class HealthBar : MonoBehaviour {

	[Tooltip("The scale for the health bar")]
	public float Scale = 1;

	[Tooltip("The hieght of the health bar off of the parent object")]
	public float Yoffset = 2;

	[Tooltip("The margin between the actual health and the background max health.")]
	public float Margin = 0.05f;

	public GameObject maxHealthBarPrefab;
	public GameObject healthBarPrefab;

	private GameObject maxHealthBar;
	private GameObject healthBar;

	private HealthAligns Align = HealthAligns.Left;

	private InkObject parent;


	// Use this for initialization
	void Start () {
		parent = gameObject.GetComponentInParent<InkObject>();

		Vector3 maxHealthOffset = transform.position + new Vector3(0, Yoffset, 0);

		maxHealthBar = (GameObject)Instantiate(maxHealthBarPrefab, maxHealthOffset, transform.rotation);
		healthBar = (GameObject)Instantiate(healthBarPrefab, maxHealthOffset, transform.rotation);

		Vector3 targetLook = 2*maxHealthOffset - Camera.main.transform.position;
		maxHealthBar.transform.LookAt(targetLook);
		healthBar.transform.LookAt(targetLook);
		
		maxHealthBar.transform.localScale *= Scale;
		healthBar.transform.localScale *= Scale;
		healthBar.transform.localScale += new Vector3(-2*Margin, -2*Margin, 0);
	}
	
	// Update is called once per frame
	void Update () {
		maxHealthBar.transform.position = transform.position;
		maxHealthBar.transform.position += new Vector3(0, Yoffset, 0);
		Vector3 targetLook = 2*maxHealthBar.transform.position - Camera.main.transform.position;
		maxHealthBar.transform.LookAt(targetLook);

		float healthPercentage = parent.Health/parent.maxHealth;
		healthBar.transform.position = maxHealthBar.transform.position;
		healthBar.transform.LookAt(targetLook);
		healthBar.transform.localScale = new Vector3(healthPercentage*(maxHealthBar.transform.localScale.x-2*Margin),
													 healthBar.transform.localScale.y,
													 healthBar.transform.localScale.z);
		float xTranslate = 0;
		if (Align == HealthAligns.Left)
		{
			xTranslate = -(1-healthPercentage)*Scale/2;
		}
		else if (Align == HealthAligns.Right)
		{
			xTranslate = (1-healthPercentage)*Scale/2;
		}
		healthBar.transform.localPosition += healthBar.transform.localRotation * new Vector3(xTranslate, 0, -0.01f);
	}

	// Delete the health bars
	void OnDestroy() {
        Destroy(maxHealthBar);
        Destroy(healthBar);
    }
}
