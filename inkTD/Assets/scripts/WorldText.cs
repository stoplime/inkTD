using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldText : MonoBehaviour
{

    [Tooltip("The text that the world text will display.")]
    public string text = "";

    [Tooltip("The number of milliseconds the world text will live for before being destroyed.")]
    public int life = 1000;

    [Tooltip("The amount the world text will move per second.")]
    public Vector3 movementPerSecond = Vector3.zero;

    [Tooltip("The camera the world text will always look at.")]
    public Camera cameraToFollow;

    /// <summary>
    /// Gets or sets the number of milliseconds the world text will live for.
    /// </summary>
    public int Life
    {
        get { return life; }
        set
        {
            life = value;

            if (timer != null)
                timer.TargetTime = life;
        }
    }

    /// <summary>
    /// Gets or sets the text the world text is displaying.
    /// </summary>
    public string Text
    {
        get { return text; }
        set
        {
            text = value;
            if (textMesh != null)
            {
                textMesh.text = text;
            }
        }
    }

    private TextMesh textMesh;
    private TaylorTimer timer;

	// Use this for initialization
	void Start ()
    {
        textMesh = GetComponent<TextMesh>();
        textMesh.text = text;

        timer = new TaylorTimer(life);
        timer.Elapsed += Timer_Elapsed;
	}

    void OnValidate()
    {
        textMesh = GetComponent<TextMesh>();
        Text = text;
    }

    private void Timer_Elapsed(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update ()
    {
        timer.Update();

        transform.position = transform.position + movementPerSecond * Time.deltaTime;

		if (cameraToFollow != null)
        {
            transform.rotation = cameraToFollow.transform.rotation;
        }
	}
}
