﻿using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]

[ExecuteInEditMode]
public class GlobController : MonoBehaviour
{
    //Variable and object declarations
    private TetraRenderer tr;
    public HashSet<Vertex> vertices = new HashSet<Vertex>();
    public HashSet<Side> sides = new HashSet<Side>();
    public List<NewTetrahedron> tetrahedrons = new List<NewTetrahedron>();
    public List<float> sideSetList = new List<float>();
    public List<int[]> vertexList = new List<int[]>();
    public int[] newTetraVtx = new int[3];

    public bool addNewTetra = false;
    public bool colorPicked = false;
    public bool runSetup = true;
    public bool internalView = false;
    public Vector3 centerMass;

    //Runs once at start of program
    void Start()
    {
        colorPicked = false;
        runSetup = true;
        tr = new TetraRenderer(this.gameObject);

        vertexList.Add(new int[4] { 0, 1, 2, 3 });
        vertexList.Add(new int[4] { 2, 1, 0, 4 });
        //vertexList.Add(new int[4] { 1, 2, 3, 5 });
        newTetraVtx = new int[3] { 1, 2, 3 };
    }

    //Calculates center of mass of glob
    public Vector3 calcCOM()
    {
        float averagedX = 0.0f;
        float averagedY = 0.0f;
        float averagedZ = 0.0f;
        int vertexCount = 0;
        foreach (Vertex vertex in vertices)
        {
            averagedX += vertex.pos.x;
            averagedY += vertex.pos.y;
            averagedZ += vertex.pos.z;
            vertexCount++;
        }
        averagedX /= (float)vertexCount;
        averagedY /= (float)vertexCount;
        averagedZ /= (float)vertexCount;
        Vector3 localCom = new Vector3(averagedX, averagedY, averagedZ);

        Vector3 worldCOM = transform.TransformPoint(localCom);
        return worldCOM;
    }

    public int closestVertex(Vector3 hitPos)
    {
        int closestID = 0;
        float closestDist = 10.0f;

        foreach (NewTetrahedron tetrahedron in tetrahedrons)
        {
            foreach (Vertex vertex in tetrahedron.vertices) {
                Vector3 absolutePos = transform.TransformPoint(vertex.pos);
                float distance = Vector3.Distance(absolutePos, hitPos);

                if (distance < closestDist)
                {
                    closestID = vertex.ID;
                    closestDist = distance;
                }
            }
        }

        return closestID;
    }

    public int maxVertex()
    {
        int maxVertexID = 0;

        for (int o = 0; o < vertexList.Count; o++)
        {
            for (int l = 0; l < 4; l++)
            {
                if (vertexList[o][l] > maxVertexID)
                {
                    maxVertexID = vertexList[o][l];
                }
            }
        }

        maxVertexID++;

        return maxVertexID;
    }

    public void addTetra(int a, int b, int c)
    {
        vertexList.Add(new int[4] { a, b, c, maxVertex() + 1 });

        setupTetras();
    }

    //Run to update list of tetrahedrons
    public void setupTetras() 
    {
        Debug.Log("---------Vertices---------");

        vertices.Clear();
        tetrahedrons.Clear();
        tr.renderTetras.Clear();

        int maxVertexCount = maxVertex();

        Debug.Log("max vertex count: " + maxVertexCount);

        Vertex[] tempVertices = new Vertex[maxVertexCount];

        for (int i = 0; i < maxVertexCount; i++)
        {
            Vertex tempVertex = new Vertex(i);
            Debug.Log("Vertex: " + tempVertex + "; ID: " + tempVertex.ID + "; Pos: " + tempVertex.pos);
            tempVertices[i] = tempVertex;
            vertices.Add(tempVertex);
        }

        TetraUtil.isNull(tempVertices);

        tetrahedrons.Add(new NewTetrahedron(tempVertices[0], tempVertices[1], tempVertices[2], tempVertices[3], true));

        for (int j = 1; j < vertexList.Count; j++)
        {
            tetrahedrons.Add(new NewTetrahedron(
                tempVertices[vertexList[j][0]], 
                tempVertices[vertexList[j][1]], 
                tempVertices[vertexList[j][2]], 
                tempVertices[vertexList[j][3]])
            );
        }

        Debug.Log(tetrahedrons.Count + " tetrahedron(s)");

        foreach (NewTetrahedron tetra in tetrahedrons)
        {
            tr.renderTetras.Add(tetra);
        }

        Debug.Log(tr.renderTetras.Count + " render tetrahedron(s)");
    }

    //Physics update, runs repeatedly
    void FixedUpdate()
    {
        //Sets center of mass
        centerMass = calcCOM();

        //Tells tetraRenderer to pick color
        if (!colorPicked)
        {
            colorPicked = true;
            tr.pickRandomColor();
        }

        //Runs setup of tetrahedrons
        if (runSetup)
        {
            runSetup = false;
            setupTetras();
        }

        //Runs adding of new tetrahedron
        if (addNewTetra)
        {
            addNewTetra = false;
            addTetra(newTetraVtx[0], newTetraVtx[1], newTetraVtx[2]);
        }

        //Sets side lengths in tetrahedrons
        int p = 0;

        foreach (NewTetrahedron tetrahedron in tetrahedrons)
        {
            foreach (Side side in tetrahedron.sides)
            {
                if(sideSetList.Count < p + 1)
                {
                    sideSetList.Add(2.0f);
                }

                side.length = sideSetList[p];
                   
                p++;
            }
        }

        //Runs loop of tetrahedrons
        foreach(NewTetrahedron tetra in tetrahedrons)
        {
            tetra.loop();
        }

        //Catches if tetraRenderer does not exist
        if (tr == null)
        {
            tr = new TetraRenderer(this.gameObject);
        }

        //Loops tetraRenderer
        tr.loop();
    }
}