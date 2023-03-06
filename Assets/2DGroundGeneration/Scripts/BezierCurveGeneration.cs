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
    [Space(10)]
    [Range(0.001f, 0.1f), Tooltip("Distance between points in bezier curve")]
    public float spaceBetweenPoints = 0.1f;

    [Header("Curve Points ===========================================")]
    [Tooltip("List of all generated curve points")]
    public List<Vector3> generatedCurvePoints = new List<Vector3>();
    [Tooltip("List of all rotations of index corresponding points")]
    public List<float> generatedCurvePointRotations = new List<float>();


    [Header("Mesh Creation ===========================================")]
    [Tooltip("Determine if mesh gets generated")]
    public bool generateMesh;

    [Tooltip("Object for underground mesh")]
    public GameObject undergroundMeshObj;

    [Tooltip("Object for depth mesh")]
    public GameObject depthMeshObj;

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


        SetAngleType(); // sets the angle type


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

            generatedCurvePoints = GenerateCurvePointPositions(spaceBetweenPoints); //create list of point positions
            generatedCurvePointRotations = GenerateCurvePointRotations(spaceBetweenPoints); //createt list of point rotations

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

        CameraRendering();
    }

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
            adjustedSpacing = SetSpaceBetweenObjs(x);

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
            adjustedSpacing = SetSpaceBetweenObjs(x);

            // save current point rotation ( the curve's normal at the given point )
            point_rotations.Add(GetNormalRotationAngle(x, p0.position, p1.position, p2.position, p3.position));
        }

        return point_rotations;
    }
    #endregion

    #region BEZIER MATH ====================================================================
    public float SetSpaceBetweenObjs(float t)
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
        
        /*
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
        */

    }

    private void DisplayBezierTangent()
    {
        Vector3 pointOnCurve = GetPointOnCurve(t);

        /*
        Handles.color = Color.white;
        Vector3 tangent = new Vector3(ComputeBezierDerivative(t, p0.position.x, p1.position.x, p2.position.x, p3.position.x), ComputeBezierDerivative(t, p0.position.y, p1.position.y, p2.position.y, p3.position.y));
        Handles.DrawSolidDisc(tangent, Vector3.forward, 0.05f);
        

        Debug.DrawRay(pointOnCurve, new Vector3(tangent.x, tangent.y, 0)); //tangent
        Debug.DrawRay(pointOnCurve, new Vector3(-tangent.y * 0.2f, tangent.x * 0.2f, 0)); //normal

        print("Tangent at point t: " + tangent.x + "/" + tangent.y);

    */
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

        /*
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
        */
    }

    private void OnDrawGizmos()
    {
        // show start point of curve
        if (generatedCurvePoints.Count > 0)
        {
            /*
            Handles.color = Color.black;
            Handles.DrawSolidDisc(generatedCurvePoints[0], Vector3.forward, 0.1f);
            */
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

    public void CameraRendering()
    {
        // << CAMERA RENDERING >>
        // if within range and generation is not made, make generation
        if ((inCameraRange || cameraRangeOverride) && !generationFinished)
        {
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
            // disable mesh
            undergroundMeshObj.GetComponent<MeshRenderer>().enabled = false;
            depthMeshObj.GetComponent<MeshRenderer>().enabled = false;

            generationFinished = false;
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

