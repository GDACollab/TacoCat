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

        // if generation finished and environment not spawned
        if (!environmentSpawned)
        {
            SpawnAllEnvironmentObjects();

            if (drawLine) { DrawCurveLine(groundPoints, lineWidth, lineMaterial); }
        }

    }

    public void SpawnAllEnvironmentObjects()
    {
        // get ground points
        groundPoints = stageManager.allLevelGroundPoints;
        groundRotations = stageManager.allLevelGroundRotations;

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

        // << SPAWN OBJECTS >>
        for (int currPointIndex = 10; currPointIndex < groundPoints.Count - 1; currPointIndex += spacing)
        {
            // spawn new environment object
            int facing = Random.Range(0, 2)*2 - 1; 
            float thisScale = scale + (Random.Range(0, treeScaleVariance * 2) - treeScaleVariance); //this will have to be changed once we are spawning objects besides trees
            SpawnEnvObj(prefabs[Random.Range(0, prefabs.Count)], currPointIndex, thisScale, facing, sortingOrder, zposition);

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
    }

    public GameObject SpawnEnvObj(GameObject prefab, int pointIndex, float scale, int facing, int sortingOrder, int zposition = 0)
    {
        // create a random environment object at indexed groundPoint and with rotation
        GameObject newEnvObject = Instantiate(prefab, groundPoints[pointIndex], Quaternion.Euler(new Vector3(0, 0, 0)));

        //set parent
        newEnvObject.transform.parent = treeGenParent;
        allSpawnedObjects.Add(newEnvObject);

        // randomly face left or right
        Vector3 facingVector = new Vector3( facing,  1 );
        newEnvObject.transform.localScale = facingVector * scale;

        // set z position
        newEnvObject.transform.position = SetZ(newEnvObject.transform.position, zposition);

        // Move down slightly 
        newEnvObject.transform.position = new Vector3(newEnvObject.transform.position.x, newEnvObject.transform.position.y + treeYOffset, newEnvObject.transform.position.z);

        //Apply rotation 
        if(treeRotationEnabled){
            newEnvObject.transform.Rotate(new Vector3(0, 0, groundRotations[pointIndex] * (treeRotScalar / 100)));
        }

        // << SET SORTING ORDER >>
        if (!newEnvObject.GetComponentInChildren<SpriteRenderer>())
        {
            Debug.LogError("Env Object doesn't have SpriteRenderer component", newEnvObject);
        }
        else
        {
            // set sorting order of sprite renderer
            newEnvObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = sortingOrder;
        }

        return newEnvObject;
    }

    #region HELPER FUNCTIONS =================================================================
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
