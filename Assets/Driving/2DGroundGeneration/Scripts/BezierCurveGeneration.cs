using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public enum CHUNK_STYLES { random, rounded, straight, flat };

[RequireComponent(typeof(LineRenderer))]
[ExecuteAlways]
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

    public enum ANGLE_TYPE { UPHILL, DOWNHILL, FLAT }

    public bool debugMode;
    public ANGLE_TYPE angleType;
    public CHUNK_STYLES chunkStyle;

    [HideInInspector]
    public bool generationFinished;
    [HideInInspector]
    private LineRenderer lineRenderer;
    [HideInInspector]
    public List<Vector3> generatedPoints = new List<Vector3>();
    [HideInInspector]
    public List<float> generatedRotations = new List<float>();
    [HideInInspector]
    public List<Transform> bezierPoints = new List<Transform>();
    [HideInInspector]
    public Vector3 p0_pos, p1_pos, p2_pos, p3_pos;

    [Header("Generation Editing ===========================================")]
    [Range(0.01f, 0.1f), Tooltip("Distance between points in bezier curve")]
    public float spaceBetweenPoints = 0.1f;

    public Vector2 rounded_p1_offset = new Vector2(3, 1);
    public Vector2 rounded_p2_offset = new Vector2(3, 1);

    public Vector2 straight_p1_offset = new Vector2(2, 1);
    public Vector2 straight_p2_offset = new Vector2(5, 2);

    public Vector2 flat_p1_offset = new Vector2(30, 20);
    public Vector2 flat_p2_offset = new Vector2(30, 20);

    private void Start()
    {
    }

    public void Update()
    {

        // [[ DEBUG MODE ]]
        if (debugMode)
        {
            if (lineRenderer == null)
            {
                lineRenderer = GetComponent<LineRenderer>();

            }
            lineRenderer.enabled = true;

            GenerateCurve();
            UpdateDebugPositions();

            if (generatedPoints.Count > 0)
            {
                DrawCurveLine(generatedPoints);
            }
        }
        else
        {
            if (lineRenderer != null)
            {
                lineRenderer.enabled = false;
            }
        }

    }

    public void GenerateCurve()
    {
        // reset curve
        generationFinished = false;
        generatedPoints.Clear();
        generatedRotations.Clear();

        SetAngleType();

        if (chunkStyle == CHUNK_STYLES.random)
        {
            int enumLength = System.Enum.GetValues(typeof(CHUNK_STYLES)).Length;
            chunkStyle = (CHUNK_STYLES)Random.Range(1, enumLength);
        }

        SetChunkStyle(chunkStyle);

        // update transform points
        for (int i = 0; i < bezierPoints.Count; i++)
        {
            if (i == 0) { bezierPoints[0].position = p0_pos; }
            else if (i == 1) { bezierPoints[1].position = p1_pos; }
            else if (i == 2) { bezierPoints[2].position = p2_pos; }
            else if (i == 3) { bezierPoints[3].position = p3_pos; }
        }

        // [[ GENERATION ]]
        generatedPoints = GenerateCurvePositions(spaceBetweenPoints); //create list of point positions
        generatedRotations = GenerateCurveRotations(spaceBetweenPoints); //createt list of point rotations

        generationFinished = true;
    }

#region POINT GENERATION =================================================================================
    public Vector3 GetPointOnCurve(float t)
    {
        //first set of lerps
        Vector3 a = Vector3.Lerp(p0_pos, p1_pos, t);
        Vector3 b = Vector3.Lerp(p1_pos, p2_pos, t);
        Vector3 c = Vector3.Lerp(p2_pos, p3_pos, t);

        //second set of lerps
        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        //getting the optimal point
        Vector3 pointOnCurve = Vector3.Lerp(d, e, t);

        return pointOnCurve;
    }

    public List<Vector3> GenerateCurvePositions(float adjustedSpacing)
    {
        List<Vector3> point_positions = new List<Vector3>();
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

    public List<float> GenerateCurveRotations(float adjustedSpacing)
    {
        List<float> point_rotations = new List<float>();
        // adjusted spacing changes the spacing between the points based on the tangent of the line

        for (float x = 0; x < 1; x += adjustedSpacing)
        {
            // get the updated spacing for the next point
            adjustedSpacing = SetSpaceBetweenObjs(x);

            // save current point rotation ( the curve's normal at the given point )
            point_rotations.Add(GetNormalRotationAngle(x, p0_pos, p1_pos, p2_pos, p3_pos));
        }

        return point_rotations;
    }
#endregion

#region BEZIER MATH ====================================================================
    public float SetSpaceBetweenObjs(float t)
    {
        float pointYTangent = ComputeBezierDerivative(t, p0_pos.y, p1_pos.y, p2_pos.y, p3_pos.y);
        float objSpace = spaceBetweenPoints;

        // if ground slope is steep then make space between smaller based on how steep
        if (Mathf.Abs(pointYTangent) >= 6f)
        {
            // Calculate the angle of the tangent
            Vector2 tangent = new Vector2(ComputeBezierDerivative(t, p0_pos.x, p1_pos.x, p2_pos.x, p3_pos.x), pointYTangent);
            float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            // Calculate a scaling factor based on the angle
            float scale = Mathf.Clamp01((angle - 45f) / 45f); // scale from 0 (shallow) to 1 (steep)

            // Adjust the space between points based on the scaling factor
            objSpace = spaceBetweenPoints * (1f - 0.5f * scale); // decrease space by up to 50%
        }

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

#region CHUNK STYLE =======================

    public void SetChunkStyle(CHUNK_STYLES chunkStyle)
    {
        // get beg and end pos
        Vector2 begPos = p0_pos;
        Vector2 endPos = p3_pos;

        // get distances
        float horzDistance = Mathf.Abs(endPos.x - begPos.x);
        float vertDistance = Mathf.Abs(endPos.y - begPos.y);

        // default offset
        Vector2 p1Offset = new Vector2(1, 1);
        Vector2 p2Offset = new Vector2(1, 1);

        if (chunkStyle == CHUNK_STYLES.rounded)
        {
            p1Offset = rounded_p1_offset;
            p2Offset = rounded_p2_offset;


            if (angleType == ANGLE_TYPE.UPHILL)
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y);
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y);
            }
            else
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y);
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y);
            }
        }
        else if (chunkStyle == CHUNK_STYLES.straight)
        {
            p1Offset = straight_p1_offset;
            p2Offset = straight_p2_offset;


            if (angleType == ANGLE_TYPE.UPHILL)
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y);
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y - (vertDistance / p2Offset.y));
            }
            else
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y);
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y + (vertDistance / p2Offset.y));
            }
        }
        else if (chunkStyle == CHUNK_STYLES.flat)
        {
            p1Offset = flat_p1_offset;
            p2Offset = flat_p2_offset;

            if (angleType == ANGLE_TYPE.UPHILL)
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y + (vertDistance / p1Offset.y));
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y - (vertDistance / p2Offset.y));
            }
            else
            {
                p1_pos = new Vector3(begPos.x + (horzDistance / p1Offset.x), begPos.y - (vertDistance / p1Offset.y));
                p2_pos = new Vector3(endPos.x - (horzDistance / p2Offset.x), endPos.y + (vertDistance / p2Offset.y));
            }
        }


    }
#endregion

#region HELPER FUNCTIONS =====================================================================
    public void SetAngleType()
    {
        //if downhill
        if (p0_pos.y > p3_pos.y)
        {
            angleType = ANGLE_TYPE.DOWNHILL;
        }
        //if uphill
        else if (p0_pos.y < p3_pos.y)
        {
            angleType = ANGLE_TYPE.UPHILL;

        }
        // if flat
        else
        {
            angleType = ANGLE_TYPE.FLAT;
        }
    }

    public void DestroyAll(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }

        list.Clear();
    }

    public void DestroyAll(List<Transform> list)
    {
        foreach (Transform obj in list)
        {
            Destroy(obj);
        }

        list.Clear();
    }

    public void DrawCurveLine(List<Vector3> points)
    {
        if (lineRenderer == null) { lineRenderer = GetComponent<LineRenderer>(); }

        lineRenderer.positionCount = 0; // Clear existing points first
        lineRenderer.positionCount = points.Count;

        // set points
        lineRenderer.SetPositions(points.ToArray());
    }

    public void UpdateDebugPositions()
    {

        p0_pos = new Vector3(0, 0);
        if (angleType == ANGLE_TYPE.FLAT)
        {
            p3_pos = new Vector3(200, 0);
        }
        else if (angleType == ANGLE_TYPE.UPHILL)
        {
            p3_pos = new Vector3(200, 200);
        }
        else if (angleType == ANGLE_TYPE.DOWNHILL)
        {
            p3_pos = new Vector3(200, -200);
        }
    }
#endregion


#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        float lineThickness = 2;

        //first set of lines
        Handles.color = Color.black;
        Handles.DrawLine(p0_pos, p1_pos, lineThickness);
        Handles.DrawLine(p1_pos, p2_pos, lineThickness);
        Handles.DrawLine(p2_pos, p3_pos, lineThickness);

        //second set
        Handles.color = Color.blue;
        Vector3 demo_a = GetPointOnCurve(0f);
        Vector3 demo_b = GetPointOnCurve(0.33f);
        Vector3 demo_c = GetPointOnCurve(0.66f);
        Handles.DrawLine(demo_a, demo_b, lineThickness);
        Handles.DrawLine(demo_b, demo_c, lineThickness);
        Handles.DrawLine(demo_c, generatedPoints[generatedPoints.Count-1], lineThickness);

        // Draw gizmos for generated points and rotations
        for (int i = 0; i < generatedPoints.Count; i++)
        {
            Vector3 point = generatedPoints[i];
            float rotation = generatedRotations[i];

            Handles.color = Color.white;
            Handles.DrawSolidDisc(point, Vector3.forward, 1f);
        }

    }

#endif

}


