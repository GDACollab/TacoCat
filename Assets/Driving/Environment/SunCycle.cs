using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SunCycle : MonoBehaviour
{
    [Tooltip("The sun GameObject to move.")]
    public GameObject sunObj;
    [Tooltip("The starting position of the sun object. The z coordinate is taken into account.")]
    public Vector3 startPos;
    [Tooltip("The x coordinate of the end position of the sun object's path. The end position will have the same y coordinate as the starting position.")]
    public float endPosX;
    [Tooltip("The y coordinate of the center of the circle upon which the sun's arc-shaped path lies. Should be equal or less than the y coordinate of the start position.")]
    public float circleCenterY;
    [Tooltip("Adjusts the height of path over time")]
    [Range(0.0f, 1.0f)]
    public float heightPercentage;
    [Tooltip("The current progress of the sun on its path. A value of 0.5 would indicate that the sun is at the highest point in its path. Intended to be set through code.")]
    [Range(0.0f, 1.0f)]
    public float pathProgress;

    private Vector3 s_circleCenter;
    private Vector3 s_vecToStartPos;
    private float s_angleFromStartToEnd;

    // Update is called once per frame
    void Update()
    {
        SetUpSunPath();
        UpdateSunPos();
    }

    public void SetUpSunPath()
    {
        // Calculate the center of the circle upon which the sun moves. This is the middle point between the start and end positions.
        s_circleCenter = new Vector3((startPos.x + endPosX) * 0.5f, circleCenterY, startPos.z);

        // Calculate the vector from the circle's center to the starting position of the sun.
        s_vecToStartPos = startPos - s_circleCenter;

        // Calculate the vector from the circle's center to the end position of the sun.
        Vector3 vecToEndPos = (new Vector3(endPosX, startPos.y, startPos.z)) - s_circleCenter;

        // Calculate the angle that the sun needs to rotate around the circle to reach the end position.
        s_angleFromStartToEnd = Vector3.Angle(s_vecToStartPos, vecToEndPos);

        // Update the sun's position based on the newly calculated path.
        UpdateSunPos();
    }

    // Updates the sun's position based on pathProgress
    public void UpdateSunPos()
    {
        float angleOffsetFromStart = Mathf.Clamp(pathProgress, 0.0f, 1.0f) * s_angleFromStartToEnd;

        // Express sun position as a rotation of s_vecToStartPos about an axis passing through s_circleCenter whose only nonzero component is its z component
        Quaternion rotationQuat = Quaternion.AngleAxis(angleOffsetFromStart, Vector3.back);

        sunObj.transform.position = rotationQuat * s_vecToStartPos + s_circleCenter;
        sunObj.transform.position = new Vector3(sunObj.transform.position.x, sunObj.transform.position.y * heightPercentage, sunObj.transform.position.z);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(s_circleCenter, 10);
    }
}
