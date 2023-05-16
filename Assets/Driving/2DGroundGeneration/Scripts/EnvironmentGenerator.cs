using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(StageManager))]
public class EnvironmentGenerator : MonoBehaviour
{

    /* ==============================================
     * 
     * This spawns the given prefabs at the points generated in groundGeneration
     * at a random interval, that is determined by minSpace and maxSpace
     * 
     */
    
    public StageManager stageManager;
    public bool environmentSpawned;
    [HideInInspector]
    public List<Vector3> groundPoints = new List<Vector3>();
    [HideInInspector]
    public List<float> groundRotations = new List<float>();
    [HideInInspector]
    public List<Vector3> mainGenerationPoints = new List<Vector3>();
    [HideInInspector]
    public List<GameObject> allSpawnedObjects = new List<GameObject>(); // stores all spawned env objects

    [Header("Environment Generation Values ===========================")]
    [Tooltip("Minimum space between environment objects")]
    [Range(1, 100)]
    public int minSpaceBetweenObjects = 10;
    [Tooltip("Maximum space between environment objects")]
    [Range(1, 100)]
    public int maxSpaceBetweenObjects = 40;

    [Header("Line")]
    public bool drawLine;
    LineRenderer lineRenderer;
    [Range(1, 100)]
    public float lineWidth = 20;
    public Vector3 lineOffset;
    public Material lineMaterial;

    [Header("<< Gas Stations >>")]
    [Tooltip("Gas station prefab")]
    public GameObject gasStationPrefab;
    [Tooltip("List of gas station sprites")]
    public List<Sprite> gasStationSprites = new List<Sprite>();
    [Tooltip("Gas station object scale")]
    public float gasStationScale;
    [Tooltip("Y offset for the gas station objects")]
    public float gasStationYOffset;
    [Tooltip("Distance in ground points that the gas stations will spawn from each end")]
    public int gasStationGroundPointIndex;

    [Header("<< Trees >>")]
    [Tooltip("Parent for the spawned trees")]
    public Transform treeGenParent;
    [Tooltip("Base scale of the Tree Objects")]
    public float treeScale = 50;
    [Tooltip("Amount of variance in the scales of individual tree objects")]
    public float treeScaleVariance = 10;
    [Tooltip("Whether or not the trees rotate with the ground at all")]
    public bool treeRotationEnabled = true;
    [Range(0, 100), Tooltip("From 0-100%, how closely will the trees align with the rotation of the ground")]
    public float treeRotScalar = 70;
    [Range(-100, 100), Tooltip("Vertical offset for the trees")]
    public float treeYOffset = 0;

    public int treeZPosition = -1;

    [Tooltip("List of tree prefabs to spawn")]
    public List<GameObject> treePrefabs = new List<GameObject>();

    [Header("<< Signs >>")]
    [Tooltip("Base sign prefab")]
    public GameObject signPrefab;
    [Tooltip("Parent for the spawned signs")]
    public Transform signGenParent;
    [Tooltip("Base scale of the Sign Objects")]
    public float signScale = 40;
    [Tooltip("Number of signs to spawn")]
    public int numSigns = 4;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        stageManager = GetComponent<StageManager>();

        StartCoroutine(Generate());
    }

    // Start is called before the first frame update
    public IEnumerator Generate()
    {
        if (stageManager == null) { yield return null; }
        yield return new WaitUntil(() => stageManager.allStagesGenerated);

        groundPoints.Clear();
        groundRotations.Clear();
        mainGenerationPoints.Clear();

        // get ground points
        groundPoints = stageManager.allLevelGroundPoints;
        groundRotations = stageManager.allLevelGroundRotations;

        foreach (GroundGeneration stage in stageManager.stages)
        {
            mainGenerationPoints.AddRange(stage.allGroundPoints); // add stage points
        }

        // if generation finished and environment not spawned
        if (!environmentSpawned)
        {
            SpawnAllEnvironmentObjects();

            if (drawLine) { DrawCurveLine(groundPoints, lineWidth, lineMaterial); }
        }

    }

    public void SpawnAllEnvironmentObjects()
    {

        DeleteAllEnvironmentObjects();

        // spawn tree objects
        SpawnEnvironmentObjects(treePrefabs, treeScale, treeZPosition);

        environmentSpawned = true;
    }

    public void DeleteAllEnvironmentObjects()
    {
        // destroy
        foreach (GameObject obj in allSpawnedObjects)
        {
            Destroy(obj);
        }

        // clear parent list
        allSpawnedObjects.Clear();

        environmentSpawned = false;
    }

    public void SpawnEnvironmentObjects(List<GameObject> prefabs, float scale, int zposition)
    {
        // check prefabs
        if (prefabs.Count < 1) { Debug.LogWarning("No environment prefabs."); return; }

        // check ground points
        if (groundPoints.Count < 1) { Debug.LogWarning("No ground points."); return; }

        // check ground rotations
        if (groundRotations.Count < 1) { Debug.LogWarning("No rotation points."); return; }

        int sortingOrder = 0; // sorting order of the object to be spawned
        int spacing = minSpaceBetweenObjects; // minimum spacing between objects
        int levelNum = GameObject.Find("GameManager").GetComponent<GameManager>().currLevel - 1;

        // << SPAWN GAS STATIONS >>
        GameObject startStation   = Instantiate(gasStationPrefab, groundPoints[gasStationGroundPointIndex] + new Vector3(0, gasStationYOffset, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
        startStation.transform.localScale = startStation.transform.localScale * gasStationScale;
        startStation.GetComponent<SpriteRenderer>().sprite = gasStationSprites[levelNum];
        if(levelNum != 2){
            GameObject endStation = Instantiate(gasStationPrefab, groundPoints[groundPoints.Count - gasStationGroundPointIndex] + new Vector3(0, gasStationYOffset, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            endStation.GetComponent<SpriteRenderer>().sprite = gasStationSprites[levelNum+1];
            endStation.transform.localScale = endStation.transform.localScale * gasStationScale;
        }

        // << SPAWN TREES >>
        for (int currPointIndex = 10; currPointIndex < groundPoints.Count - 1; currPointIndex += spacing)
        {
            // spawn new environment object
            int facing = Random.Range(0, 2)*2 - 1; 
            float thisScale = scale + (Random.Range(0, treeScaleVariance * 2) - treeScaleVariance); //this will have to be changed once we are spawning objects besides trees
            SpawnTree(prefabs[levelNum], currPointIndex, thisScale, facing, sortingOrder, zposition);

            /* ===============================
             *  << SET UP FOR NEXT ENVIRONMENT OBJECT >>
             * ============================== */

            // << SORTING ORDER >>
            // toggle sorting order so that objects on this layer dont overlap
            if (sortingOrder == 0) 
            { 
                sortingOrder = 1; 
            } 
            else if (sortingOrder == 1) 
            { 
                sortingOrder = 0; 
            }

            // << OBJECT SPACING >>
            // Determine the number of points to skip before instantiating the next prefab
            spacing = Random.Range(minSpaceBetweenObjects, maxSpaceBetweenObjects + 1);

            // Make sure we don't go past the end of the line
            if (currPointIndex + spacing >= groundPoints.Count - 1)
            {
                break;
            }
        }

        // << SPAWN SIGNS >>
        float percentage = (float)1 / (float)numSigns; // get the distance percentage
        for (int i = 0; i < numSigns; i++)
        {
            // get ground point at percentage
            int mainGenStartIndex = stageManager.PosToGroundPointIndex(stageManager.stages[0].begGenPos);
            int distanceIndex = Mathf.FloorToInt(mainGenerationPoints.Count * percentage);

            // spawn sign
            GameObject sign = SpawnSign( (i * distanceIndex) + mainGenStartIndex , i + 1);
            sign.name = "LandmarkSign " + i + " " + percentage;
        }

    }

    public GameObject SpawnSign(int pointIndex, int signNum)
    {
        Vector3 signLoc = findNearestPeak(pointIndex);
        //Debug.Log(groundPoints[pointIndex] + "       " + signLoc);
        GameObject newSignObject = Instantiate(signPrefab, signLoc, Quaternion.Euler(new Vector3(0, 0, 0)));
        newSignObject.transform.parent = signGenParent;

        newSignObject.transform.localScale = newSignObject.transform.localScale * signScale;

        TMPro.TextMeshProUGUI textBox = newSignObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        textBox.text = System.Math.Floor((100.0f / numSigns) * signNum) + "%";

        allSpawnedObjects.Add(newSignObject);
        return newSignObject;
    }

    public GameObject SpawnTree(GameObject prefab, int pointIndex, float scale, int facing, int sortingOrder, int zposition = 0)
    {
        // create a random environment object at indexed groundPoint and with rotation
        GameObject newTreeObject = Instantiate(prefab, groundPoints[pointIndex], Quaternion.Euler(new Vector3(0, 0, 0)));

        //set parent
        newTreeObject.transform.parent = treeGenParent;
        allSpawnedObjects.Add(newTreeObject);

        // randomly face left or right
        Vector3 facingVector = new Vector3( facing,  1 );
        newTreeObject.transform.localScale = facingVector * scale;

        // set z position
        newTreeObject.transform.position = SetZ(newTreeObject.transform.position, zposition);

        // Move down slightly 
        newTreeObject.transform.position = new Vector3(newTreeObject.transform.position.x, newTreeObject.transform.position.y + treeYOffset, newTreeObject.transform.position.z);

        //Apply rotation 
        if(treeRotationEnabled){
            newTreeObject.transform.Rotate(new Vector3(0, 0, groundRotations[pointIndex] * (treeRotScalar / 100)));
        }

        // << SET SORTING ORDER >>
        if (!newTreeObject.GetComponentInChildren<SpriteRenderer>())
        {
            Debug.LogError("Env Object doesn't have SpriteRenderer component", newTreeObject);
        }
        else
        {
            // set sorting order of sprite renderer
            SpriteRenderer treeSpriteRend = newTreeObject.GetComponentInChildren<SpriteRenderer>();
            treeSpriteRend.sortingOrder = sortingOrder;
        }

        return newTreeObject;
    }

    #region HELPER FUNCTIONS =================================================================

    //Given a groundPoint, find the nearest peak
    public Vector3 findNearestPeak(int pointIndex){
        Vector3 workingPoint = groundPoints[pointIndex];
        float curY = workingPoint.y;


        // set iteration indexes
        int leftIter  = pointIndex - 1;
        int rightIter = pointIndex + 1;

        // check for edge cases
        if (leftIter < 0) { leftIter = 0; }
        if (rightIter > groundPoints.Count) { rightIter = groundPoints.Count; }

        Vector3 leftNeighbor  = groundPoints[leftIter];
        Vector3 rightNeighbor = groundPoints[rightIter];

        //First check if we're already on a peak
        if (leftNeighbor.y <= curY && rightNeighbor.y <= curY){
            //Debug.Log("Sign-gen case 1");
            return workingPoint;
        }

        //Now check left
        while(leftNeighbor.y > curY){
            workingPoint = leftNeighbor;
            curY = workingPoint.y;
            leftIter--;
            leftNeighbor = groundPoints[leftIter]; 
            if(leftNeighbor.y <= curY){
                //Debug.Log("Sign-gen case 2");
                return workingPoint;
            }
        }

        //And right
        while(rightNeighbor.y > curY){
            workingPoint = rightNeighbor;
            curY = workingPoint.y;
            rightIter++;
            rightNeighbor = groundPoints[rightIter]; 
            if(rightNeighbor.y <= curY){
                //Debug.Log("Sign-gen case 3");
                return workingPoint;
            }
        }

        //Something went wrong if you're here
        //Debug.Log("Sign-gen - Nearest peak not found");
        return workingPoint;
    }


    public void DestroyListObjects(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }

        list.Clear();
    }

    Vector3 SetZ(Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }

    void DrawCurveLine(List<Vector3> points, float width,  Material material)
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.positionCount = points.Count;

        // add offset to points
        for (int i = 0; i < points.Count; i++)
        {
            points[i] += lineOffset;
        }

        // set points
        lineRenderer.SetPositions(points.ToArray());

        // set material
        lineRenderer.material = material;
    }

    #endregion


}
