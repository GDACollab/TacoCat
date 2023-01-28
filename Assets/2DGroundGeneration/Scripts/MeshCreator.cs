using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    private GameObject undergroundMeshObj;
    private Mesh undergroundMesh;

    private void Start()
    {
        undergroundMeshObj = this.gameObject;
        undergroundMesh = new Mesh();
        undergroundMeshObj.GetComponent<MeshFilter>().mesh = undergroundMesh;


    }

    public void CreateUnderground(List<Vector3> genCurvePoints, int chunkCount, float underground_height)
    {
        //find distance between end points
        //divide by set count of meshes to generate
        //generate new mesh based on chunk points in for loop adding vertices and triangles to 

        /*  
         * 0____1
         * |  /| 
         * | / | 
         * 2 -- 3
         * 
         *  generate triangles , 0 - 1 - 2, 2 - 1 - 3 
         *                       A   B   C  C   B   D
         */

        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();

        int chunkSize = (genCurvePoints.Count - 1) / chunkCount; //get number of chunks based on chunk size
        //print("genCurPoints.Count: " + genCurvePoints.Count + " / chunkCount: " + chunkCount + " = chunkSize: "  + chunkSize);

        //DONT TOUCH THIS OR I WILL CASTRATE YOU
        //For some reason this fixes positioning problems
        //undergroundMeshObj.transform.position = new Vector3(undergroundMeshObj.transform.localPosition.x, undergroundMeshObj.transform.localPosition.y);

        //move the 0 index x position to the left a tiny bit
        genCurvePoints[0] = new Vector3(genCurvePoints[0].x - 0.1f, genCurvePoints[0].y);

        //init these variables for use outside of for loop
        int chunkBegPoint_index = 0;
        int chunkEndPoint_index = chunkSize;

        //iterate through chunk count
        for (int i = 0; i < chunkCount; i++)
        {

            chunkBegPoint_index = i * chunkSize;
            chunkEndPoint_index = (i * chunkSize) + chunkSize;

            //print(genCurvePoints[0] + " // Point Count " + genCurvePoints.Count + "// End Point index " + chunkEndPoint_index + " vertices: " + verticesList.Count);

            //VERTICE POINTS
            //only need 0 && 2 if its the first mesh
            if (i == 0)
            {
                //                                                              move point to the left a little pit
                Vector3 pointA = new Vector3(genCurvePoints[chunkBegPoint_index].x, genCurvePoints[chunkBegPoint_index].y + 0.1f, 0); // 0 
                verticesList.Add(pointA);
            }

            //move up a little bit
            Vector3 pointB = new Vector3(genCurvePoints[chunkEndPoint_index].x, genCurvePoints[chunkEndPoint_index].y + 0.025f, 0); // 1
            verticesList.Add(pointB);

            if (i == 0)
            {
                Vector3 pointC = new Vector3(genCurvePoints[chunkBegPoint_index].x, genCurvePoints[chunkBegPoint_index].y - underground_height); // 2
                verticesList.Add(pointC);
            }

            Vector3 pointD = new Vector3(genCurvePoints[chunkEndPoint_index].x, genCurvePoints[chunkEndPoint_index].y - underground_height); // 3
            verticesList.Add(pointD);


            //TRIANGLE POINTS
            if (i == 0)
            {
                trianglesList.Add(0);
                trianglesList.Add(1);
                trianglesList.Add(2);
                trianglesList.Add(2);
                trianglesList.Add(1);
                trianglesList.Add(3);
            }
            else if (i == 1)
            {
                trianglesList.Add(1);
                trianglesList.Add(4);
                trianglesList.Add(3);
                trianglesList.Add(3);
                trianglesList.Add(4);
                trianglesList.Add(5);
            }
            else if (i > 1)
            {
                trianglesList.Add(i * 2);
                trianglesList.Add((i * 2) + 2);
                trianglesList.Add((i * 2) + 1);
                trianglesList.Add((i * 2) + 1);
                trianglesList.Add((i * 2) + 2);
                trianglesList.Add((i * 2) + 3);
            }
        }



        //FILL IN LAST EXTRA BIT OF MESH
        //if last endpoint isn't last point

        if (chunkEndPoint_index < (genCurvePoints.Count - 1))
        {
            //Get Vertices
            Vector3 pointB = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x + 0.1f, genCurvePoints[genCurvePoints.Count - 1].y + 0.05f); // 1
            verticesList.Add(pointB);

            //                                                              add a little extra just to cover
            Vector3 pointD = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x + 0.1f, genCurvePoints[genCurvePoints.Count - 1].y - underground_height); // 3
            verticesList.Add(pointD);

            //Get Triangle points
            int i = chunkCount;
            trianglesList.Add(i * 2);
            trianglesList.Add((i * 2) + 2);
            trianglesList.Add((i * 2) + 1);
            trianglesList.Add((i * 2) + 1);
            trianglesList.Add((i * 2) + 2);
            trianglesList.Add((i * 2) + 3);

        }

        undergroundMesh.Clear();
        undergroundMesh.vertices = verticesList.ToArray();
        undergroundMesh.triangles = trianglesList.ToArray();

        /*
        string trianglesArray = "Triangles: ";
        foreach (int triangle in trianglesList)
        {
            trianglesArray += triangle.ToString() + ", ";
        }
        Debug.Log(trianglesArray);
        */

        undergroundMesh.RecalculateNormals(); //fixes lighting


        // << SET EDGE COLLIDER >>

        // create Vector 2 list of points
        // i dont know why I didnt start with this but we're too far in now
        List<Vector2> edgePoints = new List<Vector2>();
        foreach (Vector3 v in genCurvePoints)
        {
            edgePoints.Add(new Vector2(v.x, v.y));
        }

        undergroundMeshObj.GetComponent<EdgeCollider2D>().SetPoints(edgePoints);
    }
}
