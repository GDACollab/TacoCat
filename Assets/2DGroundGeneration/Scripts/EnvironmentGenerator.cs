using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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

    [Header("Generation Values")]
    [Tooltip("Minimum space between environment objects")]
    [Range(1, 100)]
    public int minSpaceBetweenObjects = 10;
    [Tooltip("Maximum space between environment objects")]
    [Range(1, 100)]
    public int maxSpaceBetweenObjects = 40;

    [HideInInspector]
    public List<GameObject> allSpawnedObjects = new List<GameObject>(); // stores all spawned env objects


    [Header("Trees")]
    [Tooltip("Parent for the spawned trees")]
    public Transform treeGenParent;
    [Tooltip("List of tree prefabs to spawn")]
    public List<GameObject> treePrefabs = new List<GameObject>();
    [HideInInspector]


    // Start is called before the first frame update
    void Update()
    {
        // if generation finished and environment not spawned
        if (groundGeneration.generationFinished && !environmentSpawned)
        {
            SpawnAllEnvironmentObjects();
        }

        else if (!groundGeneration.generationFinished && environmentSpawned)
        {
            DeleteAllEnvironmentObejcts();
        }

    }

    public void SpawnAllEnvironmentObjects()
    {
        // get ground points
        groundPoints = groundGeneration.allGroundPoints;
        groundRotations = groundGeneration.allGroundRotations;

        DeleteAllEnvironmentObejcts();

        // spawn tree objects
        SpawnEnvironmentObjects(treePrefabs, 20);

        environmentSpawned = true;
    }

    public void DeleteAllEnvironmentObejcts()
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

    public void SpawnEnvironmentObjects(List<GameObject> prefabs, int count)
    {
        int pointIndex = 10; // index of the point to spawn the object at ,, start at ten to not spawn at direct beginning of generation
        int sortingOrder = 0; // sorting order of the object to be spawned

        // check prefabs
        if (prefabs.Count < 1) { Debug.LogWarning("No environment prefabs."); return; }

        // check ground points
        if (groundPoints.Count < 1) { Debug.LogWarning("No ground points."); return; }

        // check ground rotations
        if (groundPoints.Count < 1) { Debug.LogWarning("No rotation points."); return; }




        // << SPAWN OBJECTS >>
        for (int i = 0; i < count; i++)
        {

            // Debug.Log("point index " + pointIndex + "point count " + groundPoints.Count);

            // create a random environment object at indexed groundPoint and with rotation
            GameObject newEnvObject = Instantiate(prefabs[Random.Range(0, prefabs.Count)], groundPoints[pointIndex], Quaternion.Euler(new Vector3(0, 0, groundRotations[pointIndex])));

            //set parent
            newEnvObject.transform.parent = treeGenParent;
            allSpawnedObjects.Add(newEnvObject);

            // randomly face left or right
            int randomFacing = Random.Range(0, 2) * 2 - 1;
            newEnvObject.transform.localScale = new Vector3(randomFacing, 1);

            // set sorting layer
            newEnvObject.GetComponentInChildren<SortingGroup>().sortingLayerName = "Environment";

            //SORTING ORDER 4 / 5, very front
            newEnvObject.GetComponentInChildren<SortingGroup>().sortingOrder = sortingOrder;


            /* ===============================
             *  << SET UP FOR NEXT ENVIRONMENT OBJECT >>
             * ============================== */

            // toggle sorting order so that trees on this layer dont overlap
            if (sortingOrder == 0) 
            { 
                sortingOrder = 1; 
            } 
            else if (sortingOrder == 1) 
            { 
                sortingOrder = 0; 
            } 

            // if index + max space between objects < end of groundPoints, give random index amount
            if (pointIndex + maxSpaceBetweenObjects < groundPoints.Count)
            {
                pointIndex += Random.Range(minSpaceBetweenObjects, maxSpaceBetweenObjects + 1);
            }
            //else break
            else { break; }
        }
    }



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
