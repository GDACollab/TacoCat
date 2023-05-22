using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
public class GroundMeshCreator : MonoBehaviour
{
    private StageManager stageManager;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private Mesh generatedMesh;
    private EdgeCollider2D edgeCollider;

    public bool meshCreated;
    public bool collisionMesh = true;
    public Material groundMaterial;
    Vector3 begPos, endPos, middleOfMesh, scaleOfGeneration, safeGenTextureScale;


    [Space(20)]
    public int underground_height = -500;

    [Header("Debug")]
    public GameObject debugMarker;
    public List<GameObject> debugMarkerList = new List<GameObject>();

    private void Awake()
    {
        // setup mesh components
        stageManager = GetComponentInParent<StageManager>();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = groundMaterial;
        generatedMesh = new Mesh();
        edgeCollider = GetComponent<EdgeCollider2D>();
    }

    public void GenerateUndergroundMesh(List<Vector3> genGroundPoints)
    {
        // get mesh values
        begPos = genGroundPoints[0];
        endPos = genGroundPoints[genGroundPoints.Count - 1];

        // get mesh positions
        middleOfMesh = begPos + (endPos - begPos) / 2;
        scaleOfGeneration = new Vector2(endPos.x - begPos.x, (endPos.y - begPos.y) * 3); // shows how tall and deep hills can get
        safeGenTextureScale = scaleOfGeneration * 1.25f; // add additional safety margins

        // [[ CREATE UNDERGROUND ]]
        CreateUnderground(genGroundPoints, underground_height);
    }

    public void DestroyUndergroundMesh()
    {
        generatedMesh.Clear();
        generatedMesh = new Mesh();
        meshFilter.mesh = generatedMesh;
        meshCreated = false;
    }

    #region UNDERGROUND MESH ======================================================
    void CreateUnderground(List<Vector3> genCurvePoints, float underground_height)
    {
        //find distance between end points
        //divide by set count of meshes to generate
        //generate new mesh based on chunk points in for loop adding vertices and triangles to 

        //if (chunkCount <= 0) { chunkCount = Mathf.FloorToInt(genCurvePoints.Count * 0.99f); } // what the fuck why idk

        int chunkCount = genCurvePoints.Count - 1;
        if (underground_height <= 0) { underground_height *= -1; }

        int triSize = Mathf.FloorToInt(genCurvePoints.Count / chunkCount); //get number of chunks based on chunk size

        Debug.Log("Underground Mesh Creator :: chunkCount " + chunkCount + " // underground_height " + underground_height + " // chunkSize: "  + triSize);

        //DONT TOUCH THIS OR I WILL CASTRATE YOU
        //For some reason this fixes positioning problems
        transform.position = Vector3.zero + stageManager.generationOffset;
        //undergroundMeshObj.transform.position = new Vector3(undergroundMeshObj.transform.localPosition.x, undergroundMeshObj.transform.localPosition.y);


        // create vertices, triangles, uvs
        generatedMesh.Clear();
        generatedMesh.vertices = GetUndergroundVertices(genCurvePoints, underground_height, triSize, chunkCount);
        generatedMesh.triangles = GetTriangles(chunkCount);
        generatedMesh.uv = GetQuadUVs();

        // Debug Vertice Points
        //DebugVertices(undergroundMesh.vertices);

        generatedMesh.RecalculateBounds();
        generatedMesh.RecalculateNormals(); //fixes lighting
        meshFilter.sharedMesh = generatedMesh;

        // << SET EDGE COLLIDER >>
        if (collisionMesh)
        {
            edgeCollider.enabled = true;

            // create Vector 2 list of points
            // i dont know why I didnt start with this but we're too far in now
            List<Vector2> edgePoints = new List<Vector2>();
            foreach (Vector3 v in genCurvePoints)
            {
                edgePoints.Add(new Vector2(v.x, v.y));
            }

            edgeCollider.SetPoints(edgePoints);
        }
        else
        {
            edgeCollider.enabled = false;
        }

        Debug.Log("Underground Mesh Created", gameObject);
        meshCreated = true;
    }

    Vector3[] GetUndergroundVertices(List<Vector3> genCurvePoints, float underground_height, int triSize, int chunkCount)
    {
        List<Vector3> verticesList = new List<Vector3>();

        /*  
         * 0___2
         * |  /| 
         * | / | 
         * 1---3
         * 
         *  generate triangles :: 1 - 0 - 2, 1 - 2 - 3 
         */

        // << FIRST VERTICES >>
        Vector3 vert0 = new Vector3(genCurvePoints[0].x, genCurvePoints[0].y, 0); // 0
        verticesList.Add(vert0);


        // << MIDDLE VERTICES >>
        Vector3 vert1 = new Vector3(genCurvePoints[0].x, -underground_height, 0); // 1
        verticesList.Add(vert1);

        //init these variables for use outside of for loop
        int nextHorzPointIndex = triSize;
        //iterate through chunk count
        for (int i = 1; i < chunkCount; i++)
        {
            // index of the next horizontal vertice
            nextHorzPointIndex = (i * triSize);

            //print(genCurvePoints[0] + " // Point Count " + genCurvePoints.Count + "// End Point index " + chunkEndPoint_index + " vertices: " + verticesList.Count);

            // create vertice quad from last two vertices
            Vector3 topVertice = new Vector3(genCurvePoints[nextHorzPointIndex].x, genCurvePoints[nextHorzPointIndex].y, 0); // 2
            verticesList.Add(topVertice);

            Vector3 bottomVertice = new Vector3(genCurvePoints[nextHorzPointIndex].x, -underground_height, 0); // 3
            verticesList.Add(bottomVertice);
        }

        // << LAST VERTICES >>
        // if last iteration of horz point is not the end ... 
        if (nextHorzPointIndex < (genCurvePoints.Count - 1))
        {
            Vector3 endVertTop = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x, genCurvePoints[genCurvePoints.Count - 1].y);
            verticesList.Add(endVertTop);

            Vector3 endVertBot = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x, - underground_height);
            verticesList.Add(endVertBot);

        }

        Debug.Log("undergroundMesh vertice count :: " + verticesList.Count);

        return verticesList.ToArray();
    }
    #endregion


    #region HELPER FUNCTIONS ==============================================
    int[] GetTriangles(int chunkCount)
    {
        List<int> trianglesList = new List<int>();

        for (int i = 0; i < chunkCount; i++)
        {
            //TRIANGLE POINTS
            if (i == 0)
            {
                trianglesList.Add(1);
                trianglesList.Add(0);
                trianglesList.Add(2);
                trianglesList.Add(1);
                trianglesList.Add(2);
                trianglesList.Add(3);
            }
            else if (i == 1)
            {
                trianglesList.Add(3);
                trianglesList.Add(2);
                trianglesList.Add(4);
                trianglesList.Add(3);
                trianglesList.Add(4);
                trianglesList.Add(5);
            }
            else if (i < chunkCount)
            {
                trianglesList.Add((i * 2) + 1); // 5
                trianglesList.Add((i * 2));     // 4
                trianglesList.Add((i * 2) + 2); // 6
                trianglesList.Add((i * 2) + 1); // 5
                trianglesList.Add((i * 2) + 2); // 6
                trianglesList.Add((i * 2) + 3); // 7
            }
        }

        Debug.Log("undergroundMesh triangles count :: " + trianglesList.Count);

        return trianglesList.ToArray();
    }

    Vector2[] GetQuadUVs()
    {
        Vector2[] uvs = new Vector2[generatedMesh.vertices.Length];
        int scale = 4;
        int quad_count = Mathf.FloorToInt(generatedMesh.vertices.Length / scale);

        // create quad uvs
        for (int i = 0; i < quad_count; i ++)
        {

            int j = i * scale; // every 4 indexes

            uvs[j] = new Vector2(j, j);
            uvs[j + 1] = new Vector2(j, j + scale);
            uvs[j + 2] = new Vector2(j + scale, j + scale);
            uvs[j + 3] = new Vector2(j + scale, j);

        }

        return uvs;
    }

    void DebugVertices(Vector3[] vertices)
    {
        foreach (GameObject obj in debugMarkerList)
        {
            Destroy(obj);
        }
        debugMarkerList.Clear();


        int i = 0;
        foreach (Vector3 pos in vertices)
        {
            i++;
            
            GameObject newMark = Instantiate(debugMarker, pos, Quaternion.identity);
            newMark.transform.name = "V" + i;
            debugMarkerList.Add(newMark);
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(middleOfMesh, scaleOfGeneration);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(middleOfMesh, safeGenTextureScale);
    }
    #endregion

    #region DEPTH MESH ============================================================
    /*
    void CreateGroundDepth(List<Vector3> genCurvePoints, int chunkCount, float ground_depth)
    {
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();

        int chunkSize = (genCurvePoints.Count - 1) / chunkCount; //get number of chunks based on chunk size

        //DONT TOUCH THIS OR I WILL CASTRATE YOU
        //For some reason this fixes positioning problems
        depthMeshObj.transform.position = new Vector3(depthMeshObj.transform.localPosition.x, depthMeshObj.transform.localPosition.y);

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
                Vector3 pointA = new Vector3(genCurvePoints[chunkBegPoint_index].x, genCurvePoints[chunkBegPoint_index].y - 1f, 0); // 0 
                verticesList.Add(pointA);
            }

            Vector3 pointB = new Vector3(genCurvePoints[chunkEndPoint_index].x, genCurvePoints[chunkEndPoint_index].y - 1f, 0); // 1
            verticesList.Add(pointB);

            if (i == 0)
            {
                Vector3 pointC = new Vector3(genCurvePoints[chunkBegPoint_index].x, genCurvePoints[chunkBegPoint_index].y, depth_length); // 2
                verticesList.Add(pointC);
            }

            Vector3 pointD = new Vector3(genCurvePoints[chunkEndPoint_index].x, genCurvePoints[chunkEndPoint_index].y, depth_length); // 3
            verticesList.Add(pointD);


            //TRIANGLE POINTS
            if (i == 0)
            {
                trianglesList.Add(0);
                trianglesList.Add(2);
                trianglesList.Add(1);

                trianglesList.Add(1);
                trianglesList.Add(2);
                trianglesList.Add(3);
            }
            else if (i == 1)
            {
                trianglesList.Add(1);
                trianglesList.Add(3);
                trianglesList.Add(4);

                trianglesList.Add(4);
                trianglesList.Add(3);
                trianglesList.Add(5);
            }
            else if (i > 1)
            {

                //if i == 2:  4 5 6 6 5 7

                trianglesList.Add(i * 2);
                trianglesList.Add((i * 2) + 1);
                trianglesList.Add((i * 2) + 2);
                trianglesList.Add((i * 2) + 2);
                trianglesList.Add((i * 2) + 1);
                trianglesList.Add((i * 2) + 3);
            }
        }

        //FILL IN LAST EXTRA BIT OF MESH
        //if last endpoint isn't last point

        if (chunkEndPoint_index < (genCurvePoints.Count - 1))
        {
            //Get Vertices
            Vector3 pointB = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x, genCurvePoints[genCurvePoints.Count - 1].y); // 1
            verticesList.Add(pointB);

            //                                                              add a little extra just to cover
            Vector3 pointD = new Vector3(genCurvePoints[genCurvePoints.Count - 1].x, genCurvePoints[genCurvePoints.Count - 1].y, depth_length); // 3
            verticesList.Add(pointD);

            //Get Triangle points
            int i = chunkCount;
            trianglesList.Add(i * 2);
            trianglesList.Add((i * 2) + 1);
            trianglesList.Add((i * 2) + 2);
            trianglesList.Add((i * 2) + 2);
            trianglesList.Add((i * 2) + 1);
            trianglesList.Add((i * 2) + 3);

        }

        depthMesh.Clear();
        depthMesh.vertices = verticesList.ToArray();
        depthMesh.triangles = trianglesList.ToArray();

        /*
        
        // DEBUG TRIANGLES ARRAY
        string trianglesArray = "Triangles: ";
        foreach (int triangle in trianglesList)
        {
            trianglesArray += triangle.ToString() + ", ";
        }
        Debug.Log(trianglesArray);
        */

    //depthMesh.RecalculateNormals(); //fixes lighting
    //}
    #endregion

}
