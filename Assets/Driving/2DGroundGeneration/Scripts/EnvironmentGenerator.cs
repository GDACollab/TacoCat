using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(LineRenderer))]
public class EnvironmentGenerator : MonoBehaviour
{

    /* ==============================================
     * 
     * This spawns the given prefabs at the points generated in groundGeneration
     * at a random interval, that is determined by minSpace and maxSpace
     * 
     */
    
    public GroundGeneration groundGeneration;
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



    [Header("Ground Objects ===========================================")]

    [Tooltip("Toggle ground object spawning")]
    public bool spawnGroundObjects;

    [Tooltip("Parent of all spawned ground objects")]
    public GameObject groundParent;

    [Tooltip("Ground object prefabs")]
    public List<GameObject> groundObjectPrefabs = new List<GameObject>();

    [Tooltip("Special 'connector' object prefabs")]
    public List<GameObject> endPointObjectPrefabs = new List<GameObject>();

    [Tooltip("active ground objects")]
    public List<GameObject> genGroundObjs = new List<GameObject>();

    [Tooltip("Scale of the Ground Objects")]
    public float groundObjScale = 20;

    [Tooltip("Z offset of the ground")]
    public int groundZOffest = -10;

    [Range(1, 100), Tooltip("The amount of points between spawned ground objects")]
    public int pointsBetweenGroundObjs = 40;

    [Range(0, 0.2f), Tooltip("Makes the ground objects more randomly placed so it looks more natural")]
    public float positionNoise = 0.1f;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Start is called before the first frame update
    void Update()
    {
        // if generation finished and environment not spawned
        if (groundGeneration.generationFinished && !environmentSpawned)
        {
            SpawnAllEnvironmentObjects();
            SpawnGroundObjects(groundGeneration.allGroundPoints, groundGeneration.allGroundRotations, pointsBetweenGroundObjs);

            if (drawLine) { DrawCurveLine(groundPoints, lineWidth, lineMaterial); }
        }

        else if (!groundGeneration.generationFinished && environmentSpawned)
        {
            DeleteAllEnvironmentObjects();
            DestroyAllGroundObjs();
        }

    }

    public void SpawnAllEnvironmentObjects()
    {
        // get ground points
        groundPoints = groundGeneration.allGroundPoints;
        groundRotations = groundGeneration.allGroundRotations;

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

    #region GROUND OBJECT GENERATION ===================================================================

    void SpawnGroundObjects(List<Vector3> genPoints, List<float> genPointRots, int pointsBetweenObjs)
    {
        if (!spawnGroundObjects) { return; }

        // pointsBetweenObjs can't be 0
        if (pointsBetweenObjs == 0)
        {
            Debug.LogWarning("pointsBetweenObjs cannot be set to 0");

            pointsBetweenObjs = 1;
        }

        // return if no ground prefabs
        if (groundObjectPrefabs.Count == 0)
        {
            Debug.LogError("Generation does not have any ground object prefabs", this.gameObject);
            return;
        }

        // notify if no endpoint prefabs
        if (endPointObjectPrefabs.Count == 0)
        {
            Debug.LogWarning("Generation does not have any endpoint prefabs", this.gameObject);
        }

        DestroyListObjects(genGroundObjs);


        int sortingOrder = 0; // sorting order of the object to be spawned

        // for each generation point, spawn object
        for (int i = 0; i < genPoints.Count - 1; i += pointsBetweenObjs)
        {
            GameObject groundObj;


            //if either end point, choose from small ground points
            if ((i < genPoints.Count && i >= genPoints.Count * 0.8f))
            {
                // make sure end points exist
                if (endPointObjectPrefabs.Count > 0)
                {
                    groundObj = endPointObjectPrefabs[(int)Random.Range(0, endPointObjectPrefabs.Count)];
                }
                else { continue; }
            }
            else
            {
                //get random grass object in list
                groundObj = groundObjectPrefabs[(int)Random.Range(0, groundObjectPrefabs.Count)];
            }

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

            //TOP GROUND
            SpawnNewGround(groundObj, genPoints[i] + new Vector3(0, 0.5f, 0f), genPointRots[i], sortingOrder);
        }
    }

    void SpawnNewGround(GameObject obj, Vector3 position, float rotation, int sortingOrder)
    {
        //print("ground spawned at : " + position);
        float randomYPos = Random.Range(-positionNoise * 0.9f, positionNoise * 0.9f) + position.y; //set randomY

        obj = Instantiate(obj, new Vector3(position.x, randomYPos, 0), Quaternion.identity);

        //obj = Instantiate(obj, position, Quaternion.identity); //just in case you dont want the random y pos
        obj.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation));

        //add to generated objects list
        genGroundObjs.Add(obj);

        obj.transform.parent = groundParent.transform;
        obj.transform.localScale = new Vector2(groundObjScale, groundObjScale);

        obj.transform.localPosition = SetZ(obj.transform.localPosition, groundZOffest); // set z position

        // << SET SORTING ORDER >>
        if (!obj.GetComponentInChildren<SpriteRenderer>())
        {
            Debug.LogError("Env Object doesn't have SpriteRenderer component", obj);
        }
        else
        {
            // set sorting order of sprite renderer
            obj.GetComponentInChildren<SpriteRenderer>().sortingOrder = sortingOrder;
        }
    }

    void DestroyAllGroundObjs()
    {
        foreach (GameObject o in genGroundObjs)
        {
            DestroyImmediate(o);
        }
        genGroundObjs.Clear();
    }
    #endregion

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



    #region old code
    /*
    public void SpawnEnvironment()
    {
        //SORTING LAYERS
        /*
         *      Front Trees => 5 4
         *      Mid Trees => 3 2
         *      Back Trees => 1 0
         * 
         */
    /*
   GameObject newTree;
   //foreach row
   for (int zRow = 0; zRow < 3; zRow++ )
   {
       print("zRow : " + zRow);

       //TODO: Fix Index out of range problem
       //TODO: sort sorting order of trees by depth compared to last tree depth


       //ROW 0
       if (zRow == 0)
       {
           int index = 10;
           int sortingOrder = 4;

           for (int i = 0; i < 20; i++)
           {
               print("index: " + index);

               newTree = Instantiate(tree1, groundPoints[index] + new Vector3(0, 0, Random.Range(0.02f, 0.05f)), Quaternion.Euler(new Vector3(0, 0, groundRotations[index])));

               newTree.transform.parent = treeGenParent.transform; //set parent

               // randomly face left or right
               int randomFacing = Random.Range(0, 2) * 2 - 1;
               newTree.transform.localScale = new Vector3(randomFacing, 1);

               newTree.GetComponentInChildren<SortingGroup>().sortingLayerName = "Back Environment";


               //SORTING ORDER 4 / 5, very front
               newTree.GetComponentInChildren<SortingGroup>().sortingOrder = sortingOrder;

               if (sortingOrder == 4) { sortingOrder = 5; } else if (sortingOrder == 5) { sortingOrder = 4; } //switch back and forth so that trees on this layer dont overlap


               //if index + max space < end of groundPoints, give random index amount
               if (index + maxSpace < groundPoints.Count )
               {
                   index += Random.Range(minSpace, maxSpace + 1);
               }
               //else add last bit of count to index
               else
               {
                   index += groundPoints.Count - 1 - index;
               }
           }
       }

       //ROW 1
       if (zRow == 1)
       {

           int index = 10;
           int sortingOrder = 2;

           for (int i = 0; i < 20; i++)
           {
               newTree = Instantiate(tree1, groundPoints[index] + new Vector3(0, 0, Random.Range(farthestZpos * 0.3f, farthestZpos * 0.6f)), Quaternion.Euler(new Vector3(0, 0, groundRotations[index])));

               newTree.transform.parent = treeGenParent.transform; //set parent

               // randomly face left or right
               int randomFacing = Random.Range(0, 2) * 2 - 1;
               newTree.transform.localScale = new Vector3(randomFacing, 1);

               newTree.GetComponentInChildren<SortingGroup>().sortingLayerName = "Back Environment";

               //SORTING ORDER 2 / 3, mid
               newTree.GetComponentInChildren<SortingGroup>().sortingOrder = sortingOrder;

               if (sortingOrder == 2) { sortingOrder = 3; } else if (sortingOrder == 3) { sortingOrder = 2; } //switch back and forth so that trees on this layer dont overlap

               newTree.GetComponentInChildren<SpriteRenderer>().enabled = false; //disable sprite renderer

               //if index + max space < end of groundPoints, give random index amount
               if (index + maxSpace < groundPoints.Count)
               {
                   index += Random.Range(minSpace, maxSpace + 1);
               }
               //else add last bit of count to index
               else
               {
                   index += groundPoints.Count - 1 - index;
               }
           }
       }

       if (zRow == 2)
       {
           int index = 10;
           int sortingOrder = 0;

           for (int i = 0; i < 20; i++)
           {
               newTree = Instantiate(tree1, groundPoints[index] + new Vector3(0, 0, Random.Range(farthestZpos * 0.6f, farthestZpos)), Quaternion.Euler(new Vector3(0, 0, groundRotations[index])));

               newTree.transform.parent = treeGenParent.transform; //set parent

               // randomly face left or right
               int randomFacing = Random.Range(0, 2) * 2 - 1;
               newTree.transform.localScale = new Vector3(randomFacing, 1);

               // SORTING LAYERS 0 1, farthest back
               newTree.GetComponentInChildren<SortingGroup>().sortingLayerName = "Back Environment";
               newTree.GetComponentInChildren<SortingGroup>().sortingOrder = sortingOrder;
               if (sortingOrder == 0) { sortingOrder = 1; } else if (sortingOrder == 1) { sortingOrder = 0; } //switch back and forth so that trees on this layer dont overlap


               newTree.GetComponentInChildren<SpriteRenderer>().enabled = false; //disable sprite renderer

               //if index + max space < end of groundPoints, give random index amount
               if (index + maxSpace < groundPoints.Count)
               {
                   index += Random.Range(minSpace, maxSpace + 1);
               }
               //else add last bit of count to index
               else
               {
                   index += groundPoints.Count - 1 - index;
               }
           }

       }

   }


}
*/
    #endregion

}
