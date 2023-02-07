using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientBenchManager : MonoBehaviour
{
    public GameObject ingredientSpotPrefab;

    public List<GameObject> ingredientSpots;


    // determines the scale of ingredient spot && positions of the spots
    public Transform ingrBound_botLeft;
    public Transform ingrBound_topRight;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrawGizmos()
    {
        /*
        Vector2 botLeft = ingrBound_botLeft.position;
        Vector2 topRight = ingrBound_topRight.position;


        // show the ingredient spot bounding box
        Vector2 size = topRight - botLeft;
        Vector2 center = botLeft + size / 2;

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(center, size);
        */


        /*
        // evenly space cubes inside bounding box
        int numCubes = 4;
        float margin = 1;
        float cubeSize = size.x / numCubes;

        for (int i = 0; i < numCubes; i++)
        {
            Vector2 offset = new Vector2((i * cubeSize) + margin, 0);
            Vector2 cubeCenter = new Vector2(botLeft.x + cubeSize/2, center.y) + offset;
            Gizmos.DrawWireCube(cubeCenter, new Vector2(cubeSize, cubeSize));
        }
        */

    }
}
