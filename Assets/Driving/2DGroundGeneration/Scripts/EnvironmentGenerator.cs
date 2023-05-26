using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    LineRenderer lineRenderer;
    DrivingUIManager uiManager;
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

    [Header("======== Environment Generation Values ========")]
    [Space(10)]


    [Space(10)]
    public Transform envGenParent;

    [Header("<< Environment Objects 1 >>>>>>>>>>>>>>>>>>>")]
    [Tooltip("Minimum space between environment objects")]
    [Range(1, 100)]
    public int env1_minSpacing = 10;
    [Tooltip("Maximum space between environment objects")]
    [Range(1, 100)]
    public int env1_maxSpacing = 40;
    [Tooltip("Amount of variance in the scales of individual tree objects")]
    public Vector2 env1ScaleRange = new Vector2(1, 10);
    [Space(10), Tooltip("Whether or not the trees rotate with the ground at all")]
    public bool env1_rotationEnabled = true;
    public bool env1_flipEnabled;

    [Range(0, 100), Tooltip("From 0-100%, how closely will the trees align with the rotation of the ground")]
    public float env1_rotScalar = 30;
    
    [Space(10), Range(0, 200), Tooltip("Vertical offset for the trees")]
    public float env1ObjYOffset = 0;

    [Space(10)]
    public Transform envGen1Parent;
    public List<GameObject> envPrefabs_1 = new List<GameObject>();



    [Header("<< Environment Objects 2 >>>>>>>>>>>>>>>>>>>")]
    [Tooltip("Minimum space between environment objects")]
    [Range(1, 100)]
    public int env2_minSpacing = 10;
    [Tooltip("Maximum space between environment objects")]
    [Range(1, 100)]
    public int env2_maxSpacing = 40;
    [Tooltip("Amount of variance in the scales of individual tree objects")]
    public Vector2 env2ScaleRange = new Vector2(1, 10);
    [Tooltip("Whether or not the trees rotate with the ground at all")]
    public bool env2_rotationEnabled = true;
    public bool env2_flipEnabled;
    [Range(0, 100), Tooltip("From 0-100%, how closely will the trees align with the rotation of the ground")]
    public float env2_rotScalar = 30;

    [Space(10), Range(0, 200), Tooltip("Vertical offset for the trees")]
    public float env2ObjYOffset = 0;
    public Transform envGen2Parent;
    public List<GameObject> envPrefabs_2 = new List<GameObject>();

    [Header("Line")]
    public bool drawLine;
    public bool enableCollision;
    [Range(1, 100)]
    public float lineWidth = 20;
    public Vector3 lineOffset;
    public Material lineMaterial;

    [Header("<< Landmark Signs >>")]
    public bool spawnLandmarkSigns;
    [Tooltip("Base sign prefab")]
    public GameObject signPrefab;
    [Tooltip("Parent for the spawned signs")]
    public Transform signGenParent;
    [Tooltip("Base scale of the Sign Objects")]
    public float signScale = 40;
    [Tooltip("Number of signs to spawn")]
    public int numSigns = 4;

    [Header("<< Gas Stations >>")]
    public bool spawnGasStationEnds;
    [Tooltip("Gas station prefab")]
    public GameObject gasStationPrefab;
    public Transform playerSpawnPoint;
    [Tooltip("List of gas station sprites")]
    public List<Sprite> gasStationSprites = new List<Sprite>();
    [Tooltip("Gas station object scale")]
    public float gasStationScale;
    [Tooltip("X offset for the gas station objects")]
    public float gasStationXOffset;
    [Tooltip("Y offset for the gas station objects")]
    public float gasStationYOffset;
    [Tooltip("Distance in ground points that the gas stations will spawn from each end")]
    public int gasStationGroundPointIndex = 1;

    private void Awake()
    {
        uiManager = GameObject.FindGameObjectWithTag("DrivingGameManager").GetComponentInChildren<DrivingUIManager>();
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
        groundPoints = stageManager.allStageGroundPoints;
        groundRotations = stageManager.allStageGroundRotations;

        foreach (GroundGeneration stage in stageManager.stages)
        {
            mainGenerationPoints.AddRange(stage.allGroundPoints); // add stage points
        }
        
        // if generation finished and environment not spawned
        if (!environmentSpawned)
        {
            SpawnAllEnvironmentObjects();

            if (drawLine) { DrawCurveLine(groundPoints, lineWidth, lineMaterial); }
            else { lineRenderer.enabled = false; }

            GetComponentInChildren<EdgeCollider2D>().enabled = enableCollision;
        }
    }

    public void SpawnAllEnvironmentObjects()
    {
        DeleteAllEnvironmentObjects();

        // << SPAWN ENV OBJECTS >>
        SpawnEnvObjs(envPrefabs_1, envGen1Parent, env1_rotationEnabled, env1_flipEnabled, env1_rotScalar, env1ScaleRange, env1_minSpacing, env1_maxSpacing);
        envGen1Parent.transform.position += new Vector3(0, env1ObjYOffset, 0);

        SpawnEnvObjs(envPrefabs_2, envGen2Parent, env2_rotationEnabled, env2_flipEnabled, env2_rotScalar, env2ScaleRange, env2_minSpacing, env2_maxSpacing);
        envGen2Parent.transform.position += new Vector3(0, env2ObjYOffset, 0);

        // << SPAWN SIGNS >>
        if (spawnLandmarkSigns)
        {
            float percentage = (float)1 / (float)numSigns; // get the distance percentage
            for (int i = 0; i < numSigns; i++)
            {
                // get ground point at percentage
                int mainGenStartIndex = stageManager.PosToGroundPointIndex(stageManager.stages[0].begGenPos);
                int distanceIndex = Mathf.FloorToInt(mainGenerationPoints.Count * percentage);

                // spawn sign
                GameObject sign = SpawnSign((i * distanceIndex) + mainGenStartIndex, i);
                sign.name = "LandmarkSign " + i + " " + percentage;
            }
        }

        // << SPAWN GAS STATIONS >>
        if (spawnGasStationEnds)
        {
            // << SPAWN GAS STATIONS >>
            int levelNum = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().currLevel;

            // START STATION
            GameObject startStation = Instantiate(gasStationPrefab, groundPoints[gasStationGroundPointIndex] + new Vector3(-gasStationXOffset, gasStationYOffset, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            startStation.transform.localScale = startStation.transform.localScale * gasStationScale;
            startStation.GetComponentInChildren<SpriteRenderer>().sprite = gasStationSprites[levelNum - 1];
            startStation.GetComponentInChildren<EndTrigger>().start = true;
            startStation.GetComponentInChildren<EndTrigger>().end = false;

            playerSpawnPoint = startStation.transform;

            // END STATION
            GameObject endStation;
            endStation = Instantiate(gasStationPrefab, groundPoints[groundPoints.Count - gasStationGroundPointIndex] + new Vector3(gasStationXOffset, gasStationYOffset, 0), Quaternion.Euler(new Vector3(0, 0, 0)));
            endStation.transform.localScale = endStation.transform.localScale * gasStationScale;
            endStation.GetComponentInChildren<EndTrigger>().start = false;
            endStation.GetComponentInChildren<EndTrigger>().end = true;
            if (levelNum < 3)
            {
                endStation.GetComponentInChildren<SpriteRenderer>().sprite = gasStationSprites[levelNum];
            }
            else
            {
                endStation.GetComponentInChildren<SpriteRenderer>().sprite = gasStationSprites[Random.Range(0, gasStationSprites.Count)];
            }

        }

        // set offset
        envGenParent.transform.localPosition = stageManager.generationOffset;

        environmentSpawned = true;
    }


    public void SpawnEnvObjs(List<GameObject> envObjs, Transform parent, bool rotationEnabled, bool flipping, float rotScalar, Vector2 scaleRange, int minSpacing = 10, int maxSpacing = 40)
    {
        if (envObjs.Count < 1) { Debug.Log("No ground points."); return; }
        // check ground points
        if (groundPoints.Count < 1) { Debug.LogWarning("No ground points."); return; }
        // check ground rotations
        if (groundRotations.Count < 1) { Debug.LogWarning("No rotation points."); return; }

        // << SPAWN ENV OBJECTS >>
        int sortingOrder = 0; // sorting order of the object to be spawned
        int maxSortingOrder = 4; // max sorting order
        int curSpacing = minSpacing; // minimum spacing between objects

        for (int pointIndex = 10; pointIndex < groundPoints.Count - 1; pointIndex += curSpacing)
        {
            // spawn new environment object
            int facing = Random.Range(0, 2)*2 - 1;
            if (!flipping) { facing = 1; } // disable flipping


            float thisScale = Random.Range(scaleRange.x, scaleRange.y);
            GameObject newObj = SpawnEnvObject(GetRandomObject(envObjs), pointIndex, thisScale, facing, sortingOrder, 0);

            // set parent
            newObj.transform.parent = parent;

            //Apply rotation 
            if (rotationEnabled)
            {
                newObj.transform.Rotate(new Vector3(0, 0, groundRotations[pointIndex] * (env1_rotScalar / 100)));
            }

            /* =========================================== ////
             *  << SET UP FOR NEXT ENVIRONMENT OBJECT >>
             * ============================== */

            // << SORTING ORDER >>
            // toggle sorting order so that objects on this layer dont overlap
            if (sortingOrder < maxSortingOrder)
            {
                sortingOrder++;
            }
            else { sortingOrder = 0; }

            // << OBJECT SPACING >>
            // Determine the number of points to skip before instantiating the next prefab
            curSpacing = Random.Range(minSpacing, maxSpacing + 1);

            // Make sure we don't go past the end of the line
            if (pointIndex + curSpacing >= groundPoints.Count - 1)
            {
                break;
            }
        }
    }

    #region SPAWN ENV OBJECTS ===============================================================
    public GameObject SpawnEnvObject(GameObject prefab, int pointIndex, float scale, int facing, int sortingOrder, int zposition = 0)
    {
        // create a random environment object at indexed groundPoint and with rotation
        GameObject newEnvObject = Instantiate(prefab, groundPoints[pointIndex], Quaternion.Euler(new Vector3(0, 0, 0))); ;

        //set parent
        allSpawnedObjects.Add(newEnvObject);

        // randomly face left or right, then scale
        Vector3 flipScaleVector = newEnvObject.transform.localScale;
        flipScaleVector.x *= facing;
        flipScaleVector *= scale;
        newEnvObject.transform.localScale = flipScaleVector;

        // set z position
        newEnvObject.transform.position = SetZ(newEnvObject.transform.position, zposition + Random.Range(-0.01f, 0.01f));

        // Move down slightly 
        newEnvObject.transform.position = new Vector3(newEnvObject.transform.position.x, newEnvObject.transform.position.y + env1ObjYOffset, newEnvObject.transform.position.z);


        // << SET SORTING ORDER >>
        if (!newEnvObject.GetComponentInChildren<SpriteRenderer>())
        {
            Debug.LogError("Env Object doesn't have SpriteRenderer component", newEnvObject);
        }
        else
        {
            // set sorting order of sprite renderer
            SpriteRenderer sr = newEnvObject.GetComponentInChildren<SpriteRenderer>();
            sr.sortingOrder = sortingOrder;
        }

        return newEnvObject;
    }

    public GameObject SpawnSign(int pointIndex, int signNum)
    {
        Vector3 signLoc = FindNearestPath(pointIndex);
        GameObject newSignObject = Instantiate(signPrefab, signLoc, Quaternion.Euler(new Vector3(0, 0, 0)));
        newSignObject.transform.parent = signGenParent;

        newSignObject.transform.localScale = newSignObject.transform.localScale * signScale;

        TMPro.TextMeshProUGUI textBox = newSignObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();

        // Update "x Miles Till Cat Nyansisco" sign
        int miles = (int)(uiManager.totalMiles - uiManager.totalMiles * (uiManager.drivingGameManager.percentageTraveled));

        if (GameManager.instance.currLevel == 1)
        {
            textBox.text = uiManager.signDistances[signNum] + " Miles Till Barkersfield";
        }
        else if (GameManager.instance.currLevel == 2)
        {
            textBox.text = uiManager.signDistances[signNum] + " Miles Till Manta Cruz";
        }
        else if (GameManager.instance.currLevel == 3)
        {
            textBox.text = uiManager.signDistances[signNum] + " Miles Till Croakland";
        }

        allSpawnedObjects.Add(newSignObject);
        return newSignObject;
    }
    #endregion

    #region HELPER FUNCTIONS =================================================================

    //Given a groundPoint, find the nearest peak
    public Vector3 FindNearestPath(int pointIndex){
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

    public GameObject GetRandomObject(List<GameObject> objects)
    {
        if (objects.Count == 0) { Debug.LogError("Object List is NULL"); return null; }

        return objects[Random.Range(0, objects.Count)];
    }

    public void DestroyListObjects(List<GameObject> list)
    {
        foreach (GameObject obj in list)
        {
            Destroy(obj);
        }

        list.Clear();
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
