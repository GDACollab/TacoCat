using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BezierCurveGeneration : MonoBehaviour
{
    /* ================
     * 
     * Sky Casey , updated 2023
     * 
     * This script creates a bezier curve based off of the average of 4 edit points :: p0, p1, p2, p3
     * The bezier curve is made up of a set number of points that move closer together depending on the slope of the curve
     * 
     * From this list of curves, you can spawn a "groundObject" every set interval of points
     * These groundObjects will spawn with the normal rotation of the point they are spawned at
     * 
     * You can play with the bezier curve by moving the edit points (p0, p1, p2, p3)
     * If you start the scene in edit mode, the points will stay at their placed transforms
     * If edit mode is not selected when Play Mode is started, then the edit points will snap back to their scripted positions
     * 
     * To be able to view the underground and depth mesh, select the cameraRangeOverride
     * 
     * =======================================*/

    public LineRenderer lineRenderer;
    public MeshCreator meshCreator;

    [Header("Generation Debug Tools ===========================================")]

    [Tooltip("Select edit mode to adjust the bezier curve." +
        "If edit mode is turned on when entering Play Mode, the current positions of the edit points will be used." +
        "Otherwise, the line will default to the script edit point positions")]
    public bool editMode = false;
    [Tooltip("Allows the generation to update every frame to show value changes")]
    public bool generationEditUpdate;

    [Range(1, 10), Tooltip("Change the size of the edit point")]
    public float editPointScale = 1f;

    [Tooltip("Shows outline of the full chunk")]
    public bool showEdgeLines;

    [Tooltip("Shows line renderer")]
    public bool showBezierLine;

    [Range(0.1f, 100), Tooltip("Width of the Bezier Line")]
    public float bezierLineWidth = 0.1f;

    [Tooltip("Shows math visuals")]
    public bool showBezierMath;

    [Tooltip("Shows tangent at a specific point t")]
    public bool showBezierTangent;

    [Range(0, 1), Tooltip("Adjustable point t")]
    public float t;




    [Space(10)]
    [Header("Generation States ===========================================")]
    [Tooltip("Shows the current angle type :: flat / downhill / uphill." +
        "Shown in the Inspector for visual purposes only.")]
    public string generationAngleType;
    [HideInInspector]
    public bool generationFinished;
    [HideInInspector]
    public bool first_generation; //gatekeeps new generation




    [Header("Render States ===========================================")]
    [Tooltip("If in camera range, render objects and mesh")]
    public bool inCameraRange;

    [Tooltip("Override and show objects and mesh even if not in camera range")]
    public bool cameraRangeOverride;




    [Header("Generation Editing ===========================================")]
    [Range(0, 0.2f), Tooltip("Makes the ground objects more randomly placed so it looks more natural")]
    public float positionNoise = 0.1f;

    [Space(10)]
    [Range(0.001f, 0.1f), Tooltip("Distance between points in bezier curve")]
    public float spaceBetweenPoints = 0.1f;

    [Tooltip("The amount of points between spawned ground objects")]
    [Range(1, 100)]
    public int pointsBetweenObjs = 40;




    [Header("Curve Points ===========================================")]
    [Tooltip("List of all generated curve points")]
    public List<Vector3> generatedCurvePoints = new List<Vector3>();
    [Tooltip("List of all rotations of index corresponding points")]
    public List<float> generatedCurvePointRotations = new List<float>();

    [Header("Ground Objects ===========================================")]
    [Tooltip("Parent of all spawned ground objects")]
    public GameObject groundParent;

    [Tooltip("Ground object prefabs")]
    public List<GameObject> groundObjectPrefabs = new List<GameObject>();

    [Tooltip("Special 'connector' object prefabs")]
    public List<GameObject> endPointObjectPrefabs = new List<GameObject>();

    [Tooltip("active ground objects")]
    public List<GameObject> generated_objs = new List<GameObject>();

    [Tooltip("Scale of the Ground Objects")]
    public float groundObjScale = 20;




    [Header("Mesh Creation ===========================================")]
    [Tooltip("Determine if mesh gets generated")]
    public bool generateMesh;

    [Tooltip("Object for underground mesh")]
    public GameObject undergroundMeshObj;
    Mesh undergroundMesh;

    [Tooltip("Object for depth mesh")]
    public GameObject depthMeshObj;
    Mesh depthMesh;

    [Tooltip("Distance between horizontal points of mesh")]
    public int meshDistBetweenPoints = 10;

    [Tooltip("Height of the underground mesh")]
    public float underground_height = 5;

    [Tooltip("Depth of the depth mesh")]
    public float depth_length = 10;


    [Header("Edit Points"), Tooltip("Change the Bezier Curve with these points")]
    public Transform p0;
    public Vector3 p0_pos;

    [Space(5)]
    public Transform p1;
    public Vector3 p1_pos;

    [Space(5)]
    public Transform p2;
    public Vector3 p2_pos;

    [Space(5)]
    public Transform p3;
    public Vector3 p3_pos;

    [HideInInspector]
    public List<Transform> bezierPoints = new List<Transform>();

    //needed for gizmos
    Vector3 demo_a;
    Vector3 demo_b;
    Vector3 demo_c;
    Vector3 demo_d;
    Vector3 demo_e;

    public void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        meshCreator = undergroundMeshObj.GetComponent<MeshCreator>();

        // add points to list
        bezierPoints.Add(p0);
        bezierPoints.Add(p1);
        bezierPoints.Add(p2);
        bezierPoints.Add(p3);


        // << EDIT MODE SAVES THE POSITION OF THE TRANSFORMS >>
        if (editMode)
        {
            // sets script values to point positions
            p0_pos = p0.position;
            p1_pos = p1.position;
            p2_pos = p2.position;
            p3_pos = p3.position;

        }
        else
        {
            // sets points to script values
            p0.position = p0_pos;
            p1.position = p1_pos;
            p2.position = p2_pos;
            p3.position = p3_pos;
        }


        // set underground mesh
        //undergroundMesh = new Mesh();
        //undergroundMeshObj.GetComponent<MeshFilter>().mesh = undergroundMesh;

        // set depth mesh
        //depthMesh = new Mesh();
        //depthMeshObj.GetComponent<MeshFilter>().mesh = depthMesh;

    }

    public void Update()
    {
        Main(editMode);

        //changes the scale of the edit points
        p0.transform.localScale = new Vector3(editPointScale, editPointScale);
        p1.transform.localScale = new Vector3(editPointScale, editPointScale);
        p2.transform.localScale = new Vector3(editPointScale, editPointScale);
        p3.transform.localScale = new Vector3(editPointScale, editPointScale);

    }

    public void Main(bool edit_mode)
    {
        EditModeEnabled(edit_mode);

        // << GENERATION >>
        if (first_generation || generationEditUpdate)
        {
            generationFinished = false;

            SetAngleType(); // sets the angle type

            //if first generation && objs already spawned, deleted previously spawned
            if (first_generation && generated_objs.Count > 0) { DestroyGenObjs(); }

            generatedCurvePoints = GenerateCurvePointPositions(spaceBetweenPoints); //create list of point positions
            generatedCurvePointRotations = GenerateCurvePointRotations(spaceBetweenPoints); //createt list of point rotations
             
            //create ground at right position && rotation
            GenerateGroundFromObjects(generatedCurvePoints, generatedCurvePointRotations);

            first_generation = false; //not first gen anymore

            if (generateMesh)
            {
                // create undergound mesh
                meshCreator.CreateUnderground(generatedCurvePoints);
                //undergroundMeshObj.GetComponent<MeshRenderer>().enabled = false;

                /*
                CreateGroundDepth(generatedCurvePoints, meshDistBetweenPoints, depth_length);
                depthMeshObj.GetComponent<MeshRenderer>().enabled = false;
                */
            }


            generationFinished = true;
        }

        // << CAMERA RENDERING >>
        // if within range and generation is not made, make generation
        if ((inCameraRange || cameraRangeOverride) && !generationFinished) {
            first_generation = true;
        }
        // if in range and generation is made
        else if (inCameraRange || cameraRangeOverride)
        {
            // enable mesh
            undergroundMeshObj.GetComponent<MeshRenderer>().enabled = true;
            depthMeshObj.GetComponent<MeshRenderer>().enabled = true;
        }
        // if not in range, destroy all generated objects
        else if (!inCameraRange && !cameraRangeOverride)
        {
            DestroyListObjects(generated_objs);

            // disable mesh
            undergroundMeshObj.GetComponent<MeshRenderer>().enabled = false;
            depthMeshObj.GetComponent<MeshRenderer>().enabled = false;

            generationFinished = false;
        }
    }

    #region MESH GENERATION ================================================================
    /*
    void CreateUnderground(List<Vector3> genCurvePoints, int meshPointDistance, float underground_height)
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

        /*

        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();

        int chunkSize = (genCurvePoints.Count - 1) / meshPointDistance; //get number of chunks based on chunk size
                                                                        //print("genCurPoints.Count: " + genCurvePoints.Count + " / chunkCount: " + chunkCount + " = chunkSize: "  + chunkSize);

        //DONT TOUCH THIS OR I WILL CASTRATE YOU
        //For some reason this fixes positioning problems
        //undergroundMeshObj.transform.position = new Vector3(undergroundMeshObj.transform.localPosition.x, undergroundMeshObj.transform.localPosition.y);
        undergroundMeshObj.transform.position = Vector3.zero;

        //move the 0 index x position to the left a tiny bit
        genCurvePoints[0] = new Vector3(genCurvePoints[0].x - 0.1f, genCurvePoints[0].y);

        //init these variables for use outside of for loop
        int chunkBegPoint_index = 0;
        int chunkEndPoint_index = chunkSize;

        //iterate through chunk count
        for (int i = 0; i < meshPointDistance; i++)
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
            else if ( i == 1)
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
            int i = meshPointDistance;
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
    */

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

        depthMesh.RecalculateNormals(); //fixes lighting
    }
    #endregion

    #region POINT GENERATION =================================================================================
    public Vector3 GetPointOnCurve(float t)
    {
        //first set of lerps
        Vector3 a = Vector3.Lerp(p0.position, p1.position, t);
        Vector3 b = Vector3.Lerp(p1.position, p2.position, t);
        Vector3 c = Vector3.Lerp(p2.position, p3.position, t);

        //second set of lerps
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        //getting the optimal point
        Vector3 pointOnCurve = Vector3.Lerp(d, e, t);

        return pointOnCurve;
    }

    public List<Vector3> GenerateCurvePointPositions(float intervalBetweenPoints)
    {
        List<Vector3> point_positions = new List<Vector3>();
        float adjustedSpacing = spaceBetweenPoints; // init the object spacing as the adjusted spacing
        // adjusted spacing changes the spacing between the points based on the tangent of the line

        // for all points
        for (float x = 0; x <= 1f; x += adjustedSpacing)
        {
            // get the updated spacing for the next point
            adjustedSpacing = GetSpaceBetweenObjs(x);

            // save current point position
            point_positions.Add(GetPointOnCurve(x)); //add position to list

        }

        return point_positions;
    }

    public List<float> GenerateCurvePointRotations(float intervalBetweenPoints)
    {
        List<float> point_rotations = new List<float>();
        float adjustedSpacing = spaceBetweenPoints; // init the object spacing as the adjusted spacing
        // adjusted spacing changes the spacing between the points based on the tangent of the line

        for (float x = 0; x < 1; x += adjustedSpacing)
        {
            // get the updated spacing for the next point
            adjustedSpacing = GetSpaceBetweenObjs(x);

            // save current point rotation ( the curve's normal at the given point )
            point_rotations.Add(GetNormalRotationAngle(x, p0.position, p1.position, p2.position, p3.position));
        }

        return point_rotations;
    }
    #endregion

    #region GROUND OBJECT GENERATION ===================================================================
    public float GetSpaceBetweenObjs(float t)
    {
        float pointYTangent = ComputeBezierDerivative(t, p0.position.y, p1.position.y, p2.position.y, p3.position.y);
        float objSpace = spaceBetweenPoints;

        /*
        // if ground slope is steep then make space between smaller based on how steep
        if (Mathf.Abs(pointYTangent) >= 6f) 
        {
            // subtracts a percentage from spaceBetween so that steep slopes spawn thier objects closer together
            objSpace = spaceBetweenPoints - (spaceBetweenPoints / pointYTangent);
        }
        */
       
        return objSpace;
    }
    
    void GenerateGroundFromObjects(List<Vector3> genPoints, List<float> genPointRots)
    {
        // pointsBetweenObjs can't be 0
        if (pointsBetweenObjs == 0)
        {
            Debug.LogWarning("pointsBetweenObjs cannot be set to 0");

            pointsBetweenObjs = 1;
        }

        int mod_pointsBetweenObjs = pointsBetweenObjs;

        // return if no ground prefabs
        if (groundObjectPrefabs.Count == 0) { 
            Debug.LogWarning("Generation does not have any ground object prefabs", this.gameObject);
            return;
        }

        // notify if no endpoint prefabs
        if (groundObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("Generation does not have any endpoint prefabs", this.gameObject);
            return;
        }

        DestroyListObjects(generated_objs);

        // for each generation point, spawn object
        for (int i = 0; i < genPoints.Count - 1; i += mod_pointsBetweenObjs)
        {
            GameObject groundObj;

            
            //if either end point, choose from small ground points
            if (endPointObjectPrefabs.Count > 0 && ( i < pointsBetweenObjs || i >= genPoints.Count - (pointsBetweenObjs * 2)))
            {
                groundObj = endPointObjectPrefabs[(int)Random.Range(0, endPointObjectPrefabs.Count)];

                mod_pointsBetweenObjs = pointsBetweenObjs / 5;
            }
            else
            {
                //get random grass object in list
                groundObj = groundObjectPrefabs[(int)Random.Range(0, groundObjectPrefabs.Count)];

                mod_pointsBetweenObjs = pointsBetweenObjs;

            }


            //TOP GROUND
            SpawnNewGround(groundObj, genPoints[i] + new Vector3(0, 0.5f, 0f), genPointRots[i]);
        }
    }

    void SpawnNewGround(GameObject obj, Vector3 position, float rotation)
    {
        //print("ground spawned at : " + position);
        float randomYPos = Random.Range(-positionNoise * 0.9f, positionNoise * 0.9f) + position.y; //set randomY

        obj = Instantiate(obj, new Vector3(position.x, randomYPos, position.z), Quaternion.identity); 

        //obj = Instantiate(obj, position, Quaternion.identity); //just in case you dont want the random y pos
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

        //add to generated objects list
        generated_objs.Add(obj);

        //get the width and height of the obj collider
        //topGroundCollider = obj.GetComponent<BoxCollider2D>();
        obj.transform.parent = groundParent.transform;
        obj.transform.localScale = new Vector2(groundObjScale, groundObjScale);
    }

    void DestroyGenObjs()
    {
        foreach (GameObject o in generated_objs)
        {
            DestroyImmediate(o);
        }
    }
    #endregion

    #region BEZIER MATH ====================================================================
    // finds the derivative
    public float ComputeBezierDerivative(float t, float a, float b, float c, float d)
    {
        a = 3 * (b - a);
        b = 3 * (c - b);
        c = 3 * (d - c);

        // a(1-t)^2 + 2b(1-t)t + ct^2
        return a * (1 - t) * (1 - t) + 2 * b * (1 - t) * t + c * t * t;
    }

    // gets the normal angle
    public float GetNormalRotationAngle(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float x = ComputeBezierDerivative(t, p0.x, p1.x, p2.x, p3.x);
        float y = ComputeBezierDerivative(t, p0.y, p1.y, p2.y, p3.y);

        Vector3 normal = new Vector3(-y, x); //finding the x,y positions

        float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f; //calculating the corresponding angle
        return angle;
    }
    #endregion

    #region DEBUG VISUALS ====================================================================

    private void EditModeEnabled(bool enabled)
    {
        //disables edit point sprites
        p0.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;
        p1.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;
        p2.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;
        p3.gameObject.GetComponent<SpriteRenderer>().enabled = enabled;

        if (enabled)
        {                  
            DrawBezierCurves(generatedCurvePoints, bezierLineWidth); //draw line renderer
            DestroyGenObjs(); //destroy previous objs
        }
    }

    void DrawBezierCurves(List<Vector3> vertex_pos, float width)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        //lineRenderer.loop = true;
        lineRenderer.positionCount = vertex_pos.Count;
        lineRenderer.SetPositions(vertex_pos.ToArray());
    }

    private void DisplayBezierMath()
    {
        Vector3 pointOnCurve = GetPointOnCurve(t);

        //first set of lines
        Handles.color = Color.black;
        Handles.DrawLine(p0.position, p1.position, 3f);
        Handles.DrawLine(p1.position, p2.position, 3f);
        Handles.DrawLine(p2.position, p3.position, 3f);

        //second set
        Handles.color = Color.yellow;
        Handles.DrawLine(demo_a, demo_b);
        Handles.DrawLine(demo_b, demo_c);

        //final line
        Handles.color = Color.red;
        Handles.DrawLine(demo_d, demo_e);

        //point
        Handles.color = Color.black;
        Handles.DrawSolidDisc(pointOnCurve, Vector3.forward, 0.05f);

    }

    private void DisplayBezierTangent()
    {
        Vector3 pointOnCurve = GetPointOnCurve(t);

        Handles.color = Color.white;
        Vector3 tangent = new Vector3(ComputeBezierDerivative(t, p0.position.x, p1.position.x, p2.position.x, p3.position.x), ComputeBezierDerivative(t, p0.position.y, p1.position.y, p2.position.y, p3.position.y));
        Handles.DrawSolidDisc(tangent, Vector3.forward, 0.05f);

        Debug.DrawRay(pointOnCurve, new Vector3(tangent.x, tangent.y, 0)); //tangent
        Debug.DrawRay(pointOnCurve, new Vector3(-tangent.y * 0.2f, tangent.x * 0.2f, 0)); //normal

        print("Tangent at point t: " + tangent.x + "/" + tangent.y);
    }

    private void DisplayEdgeLines()
    {

        List<Vector3> point_positions = new List<Vector3>();


        point_positions.Add(p0.position);
        point_positions.Add(p1.position);
        point_positions.Add(p2.position);
        point_positions.Add(p3.position);

        //find the lowest point out of all points in the bezier
        float lowestYpoint = -1.00069f; //fake value
        foreach (Vector3 point in point_positions)
        {
            //print(point.y);

            if (lowestYpoint == -1.00069f)
            {
                lowestYpoint = point.y; //set first point value as lowest
            }
            else if (point.y < lowestYpoint)
            {
                lowestYpoint = point.y; //sort through points
            }
        }

        //print("Lowest Y point: " + lowestYpoint);

        //draw line to lowest point y
        Handles.color = Color.blue;

        //left end point
        Vector3 p0_base_position = new Vector3(p0_pos.x, p0_pos.y - (p0_pos.y - lowestYpoint), 0);
        Handles.DrawLine(p0_pos, p0_base_position);

        //right end point
        Vector3 p3_base_position = new Vector3(p3_pos.x, p3_pos.y - (p3_pos.y - lowestYpoint), 0);
        Handles.DrawLine(p3_pos, p3_base_position);

        //draw horizontal line across
        Handles.DrawLine(p0_base_position, p3_base_position);

    }

    private void OnDrawGizmos()
    {
        // show start point of curve
        if (generatedCurvePoints.Count > 0)
        {
            Handles.color = Color.black;
            Handles.DrawSolidDisc(generatedCurvePoints[0], Vector3.forward, 0.1f);
        }

        //show visual math
        if (showBezierMath)
        {
            DisplayBezierMath();
        }

        //show tangent
        if (showBezierTangent)
        {
            DisplayBezierTangent();
        }

        
        //show line renderer
        if (showBezierLine)
        {
            lineRenderer.enabled = true;

            //get curve points for line renderer
            List<Vector3> points = new List<Vector3>();
            for (float x = 0; x < 1; x += spaceBetweenPoints)
            {
                //first set of lerps
                Vector3 aa = Vector3.Lerp(p0.position, p1.position, x);
                Vector3 bb = Vector3.Lerp(p1.position, p2.position, x);
                Vector3 cc = Vector3.Lerp(p2.position, p3.position, x);

                //second set of lerps
                Vector3 dd = Vector3.Lerp(aa, bb, x);
                Vector3 ee = Vector3.Lerp(bb, cc, x);

                Vector3 point = Vector3.Lerp(dd, ee, x);

                points.Add(point); //add position to list

            }

            DrawBezierCurves(points, bezierLineWidth);
            points.Clear();
        }
        else
        {
            lineRenderer.enabled = false;
        }
        

        //show edge lines
        if (showEdgeLines)
        {

            DisplayEdgeLines();

        }

    }

    #endregion

    #region HELPER FUNCTIONS =====================================================================
    public void SetAngleType()
    {
        //if downhill
        if (p0_pos.y > p3_pos.y)
        {
            generationAngleType = "downhill";
        }
        //if uphill
        else if (p0_pos.y < p3_pos.y)
        {
            generationAngleType = "uphill";
        }
        // if flat
        else
        {
            generationAngleType = "flat";
        }
    }

    public void DestroyListObjects(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }

        list.Clear();
    }
    #endregion

}

