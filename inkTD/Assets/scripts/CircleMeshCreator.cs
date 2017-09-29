using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMeshCreator : MonoBehaviour
{

    public int faces = 10;

    public float range = 2;

    public float yOffset = 0.1f;

    public Color color = Color.cyan;

    public bool drawViaGizmos = false;

    /// <summary>
    /// Gets or sets the range of the circle.
    /// </summary>
    public float Range
    {
        get { return range; }
        set
        {
            range = value;
            transform.localScale = new Vector3(range, 1, range);
        }
    }

    /// <summary>
    /// Gets or sets the position y offset.
    /// </summary>
    public float YOffset
    {
        get { return yOffset; }
        set
        {
            yOffset = value;
            transform.position = new Vector3(transform.position.x, yOffset, transform.position.z);
        }
    }

    /// <summary>
    /// Gets or sets the number of outer edges the circle mesh will be generated with.
    /// </summary>
    public int Faces
    {
        get { return faces; }
        set
        {
            faces = value;

            if (previousFaceCount != faces)
            {
                GenerateMesh();
                previousFaceCount = faces;
            }
        }
    }
    
    private Mesh mesh;
    private MeshFilter filter;

    private int previousFaceCount;

	// Use this for initialization
	void Start ()
    {
        previousFaceCount = faces;
        filter = GetComponent<MeshFilter>();
        GenerateMesh();
        filter.mesh = mesh;
	}

    void OnValidate()
    {
        Range = range;
        YOffset = yOffset;
        Faces = faces;
    }

    private void GenerateMesh()
    {
        filter = GetComponent<MeshFilter>();
        mesh = Help.CreateCircularMesh(faces, 1);
        
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            rend.enabled = !drawViaGizmos;            
        }
        if (filter != null)
        {
            if (!drawViaGizmos)
            {
                filter.sharedMesh = mesh;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (drawViaGizmos)
        {
            if (mesh == null)
            {
                mesh = Help.CreateCircularMesh(faces, range);
            }
            Gizmos.color = color;
            Gizmos.DrawMesh(mesh, transform.position);
        }
    }

        // Update is called once per frame
        //void Update ()
        //   {

        //}
    }
