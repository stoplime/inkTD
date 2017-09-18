using helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMeshCreator : MonoBehaviour
{

    public int faces = 10;

    public float range = 2;

    public Color color = Color.cyan;

    public bool drawViaGizmos = false;

    //public Vector3 center;

    private Mesh mesh;

    private MeshFilter filter;

	// Use this for initialization
	void Start ()
    {
        filter = GetComponent<MeshFilter>();
        mesh = Help.CreateCircularMesh(faces, range);
        filter.mesh = mesh;
	}

    void OnValidate()
    {
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        filter = GetComponent<MeshFilter>();
        mesh = Help.CreateCircularMesh(faces, range);

        if (!drawViaGizmos)
        {
            if (filter != null)
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
