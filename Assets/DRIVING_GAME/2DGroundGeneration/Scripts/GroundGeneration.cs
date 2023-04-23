using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundGeneration : MonoBehaviour
{
    /* ===========================
     * Sky Casey , updated 2023
     * 
     * This script takes the BezierGroundGeneration to the next level
     * It takes in a beginning position and an end position and places multiple "chunks" of bezier ground generations between them.
     * It also needs a max length and height of each chunk
     * 
     * The Consitent Chunk Generator spawns every chunk with the same height and length
     * 
     * The Random Chunk Generator spawns every chunk with the same length, but with a random height
     * ===================================================================================================
     */

    public enum GENERATION_STYLES { consistent, sine, custom_sine, random };
    public enum CHUNK_STYLES { random, rounded, straight, flat };

    public GameObject curveGenerationPrefab;
    public GameObject chunkParent;
    public EnvironmentGenerator envGenerator;

    [Space(10)]
    public GameObject undergroundMeshObj;
    [HideInInspector]
    public MeshCreator undergroundMeshCreator;

    [Space(20)]
    [Tooltip("Reloads the ground generation every second so you can see how the script will react to the settings you have used.")]
    public bool editMode;
    [Tooltip("Override and show objects and mesh even if not in camera range")]
    public bool inCameraRangeOverride;
    public Color gizmosColor = Color.green;

    [Space(20)]
    public Vector3 fullGenerationPosOffset = Vector3.zero; // moves the entire generation to specified offset after everything is created
    private bool setGenOffset;

    [Range(-5000, 0)]
    public int begGenOffset = -700;
    [Range(0, 5000)]
    public int endGenOffset = 700;

    [Header("Full Generation Values ==============================")]
    [Tooltip("Choose the style of the full generation")]
    public GENERATION_STYLES generationStyle = GENERATION_STYLES.random;

    public Vector2 chunkLengthRange = new Vector2(200, 500);
    public Vector2 chunkHeightRange = new Vector2(200, 500);


    [Tooltip("Set the transform of beginning point of generation")]
    public Transform begGenerationPoint;
    [Tooltip("Set the transform of end point of generation")]
    public Transform endGenerationPoint;
    [Tooltip("Set the size of the end point sprites in edit mode")]
    [Range(0.1f, 10)]
    public float endPointDebugSize = 0.5f;
    [HideInInspector]
    public Vector2 beginningGenPos, endGenPos; // store end positions
    [HideInInspector]
    public float fullGenerationLength, fullGenerationHeight;  // store full generation length and height
    [Tooltip("Shows if generation is finished")]
    public bool generationFinished;
    public bool newGenerationStarted;

    // get all points and rotations
    [Tooltip("List of all generated ground points")]
    public List<Vector3> allGroundPoints = new List<Vector3>();
    [Tooltip("List of all generated rotations")]
    public List<float> allGroundRotations = new List<float>();


    [Header("Chunk Values ============================")]
    [Tooltip("Set the max length of a chunk")]
    public int maxChunkLength = 10;
    [Tooltip("Set the max height of a chunk")]
    public int maxChunkHeight = 5;
    [Tooltip("Set the height of the underground mesh")]
    public float undergroundMeshHeight = 100;
    [Tooltip("Choose the style of each chunk")]
    public List<CHUNK_STYLES> chunkStyles = new List<CHUNK_STYLES>(); 
    [Tooltip("List of chunks")]
    public List<GameObject> chunks = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        undergroundMeshCreator = undergroundMeshObj.GetComponent<MeshCreator>();

        // disable end point sprites
        begGenerationPoint.GetComponent<SpriteRenderer>().enabled = false;
        endGenerationPoint.GetComponent<SpriteRenderer>().enabled = false;

        // create new generation
        NewGeneration(generationStyle);

        InvokeRepeating("StaggeredUpdate", 1, 1);
    }


    public void StaggeredUpdate()
    {
        // set all ground points if generation not already finished
        if (!generationFinished && allGroundPoints.Count == 0 && chunks.Count >= GetHorizontalChunksNeeded())
        {
            SetAllGroundPoints();
        }

        // create mesh if generation finished && mesh not created
        if (generationFinished && !undergroundMeshCreator.meshCreated && allGroundPoints.Count > 0)
        {
            // create undergound mesh
            undergroundMeshCreator.CreateUnderground(allGroundPoints, undergroundMeshHeight);
        }
        else if (undergroundMeshCreator.meshCreated && !setGenOffset)
        {
            // adjust object to offset
            gameObject.transform.position += fullGenerationPosOffset;

            setGenOffset = true;
        }

        if (editMode)
        {
            // update size of sprites
            begGenerationPoint.localScale = new Vector2(endPointDebugSize, endPointDebugSize);
            endGenerationPoint.localScale = new Vector2(endPointDebugSize, endPointDebugSize);

            // enable end point sprites
            begGenerationPoint.GetComponent<SpriteRenderer>().enabled = true;
            endGenerationPoint.GetComponent<SpriteRenderer>().enabled = true;

            // if points found and mesh created
            if (generationFinished && undergroundMeshCreator.meshCreated && !newGenerationStarted)
            {
                newGenerationStarted = true; // manage generation resets

                // destroy all generation and start again
                generationFinished = false;
                allGroundPoints.Clear();
                allGroundRotations.Clear();
                undergroundMeshCreator.DestroyUndergroundMesh();

                NewGeneration(generationStyle);
            }
            else if (generationFinished && undergroundMeshCreator.meshCreated)
            {
                newGenerationStarted = false;
            }
        }
        else
        {
            // disable end point sprites
            begGenerationPoint.GetComponent<SpriteRenderer>().enabled = false;
            endGenerationPoint.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    #region GENERATION ====================================================
    public void NewGeneration(GENERATION_STYLES style)
    {
        generationFinished = false;

        // make sure z position = 0
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // get the positions of the corresponding transforms
        beginningGenPos = begGenerationPoint.position;
        endGenPos = endGenerationPoint.position;

        // get full generation size
        fullGenerationLength = endGenPos.x - beginningGenPos.x;
        fullGenerationHeight = endGenPos.y - beginningGenPos.y;

        // destroy all current chunks
        foreach (GameObject chunk in chunks) { DestroyImmediate(chunk); }

        // clear references to chunks in list
        chunks.Clear();


        StartIslandGenerator();

        if (style == GENERATION_STYLES.consistent)
        {
            ConsistentChunkGenerator();
        }
        else if (style == GENERATION_STYLES.random)
        {
            RandomChunkGenerator();
        }
        else if (style == GENERATION_STYLES.sine)
        {
            SineChunkGenerator();
        }
        else if (style == GENERATION_STYLES.custom_sine)
        {
            CustomSineChunkGenerator(chunkLengthRange, chunkHeightRange);
        }

        EndIslandGenerator();
    }

    public void StartIslandGenerator()
    {
        Vector2 offsetPos = begGenerationPoint.position + new Vector3(begGenOffset, maxChunkHeight); // init last chunk as the current beginning position

        // << FLAT START ZONE >>
        SpawnBezierGroundChunk(offsetPos + new Vector2(begGenOffset, 0), offsetPos, CHUNK_STYLES.flat); // spawn flat beginning

        // << HILL TO GAIN SPEED >>
        SpawnBezierGroundChunk(offsetPos, begGenerationPoint.position, CHUNK_STYLES.rounded); // spawn

    }

    public void EndIslandGenerator()
    {
        Vector2 offsetPos = endGenerationPoint.position + new Vector3(endGenOffset, 0); ; // init last chunk as the current beginning position

        SpawnBezierGroundChunk(endGenerationPoint.position, offsetPos, CHUNK_STYLES.flat); // spawn flat beginning
    }

    public void ConsistentChunkGenerator()
    {
        Vector2 lastChunkEndPosition = beginningGenPos; // init last chunk as the current beginning position

        int vertChunksNeeded = GetVerticalChunksNeeded();
        int horzChunksNeeded = GetHorizontalChunksNeeded();

        // << SET CHUNK HEIGHT AND LENGTH >>
        // only using one parameter for this cause its just easier
        float chunkLength = fullGenerationLength / horzChunksNeeded;
        float chunkHeight = fullGenerationHeight / horzChunksNeeded;

        // iterate through the num of chunks needed
        for (int i = horzChunksNeeded; i > 0; i--)
        {
            // create new chunk starting at the last chunks end point and ending at the full length of this chunk
            Vector2 newGenEndPos = new Vector2(lastChunkEndPosition.x + chunkLength, lastChunkEndPosition.y + chunkHeight);
            SpawnBezierGroundChunk(lastChunkEndPosition, newGenEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list

            // update last chunk end position
            lastChunkEndPosition = newGenEndPos;
        }
    }

    public void RandomChunkGenerator()
    {
        // estimated chunks needed
        float possibleHorizontalChunks = fullGenerationLength / maxChunkLength;
        float possibleVerticalChunks = fullGenerationHeight / maxChunkHeight;

        // true amount of chunks needed
        int horzChunksNeeded = GetHorizontalChunksNeeded(); // get horizontal chunks needed to reach end
        int vertChunksNeeded = GetVerticalChunksNeeded(); // get vertical chunks needed to reach the end

        // random height values
        float heightLeft = fullGenerationHeight; // amount of height left to fill in with chunks
        List<float> randomChunkHeights = new List<float>(); // create list of random heights that all add up to fullGenerationHeight

        // ========================================
        // << SET RANDOM HEIGHT VALUES >>
        // ======================================
        for (int i = 0; i < horzChunksNeeded; i++) //for each chunk needed
        {
            // get average height value of chunks between current chunk and end of generation
            float averageHeightNeeded = heightLeft / (horzChunksNeeded - i);
            float height; // height of current chunk

            // >> first chunk heights are 'random' 
            if (i < horzChunksNeeded * 0.5f)
            {
                //set random height values
                height = Random.Range(averageHeightNeeded / 2, maxChunkHeight);
            }
            // the second partition of chunk heights are set to average height to make sure generation makes it to end
            else
            {
                height = averageHeightNeeded;
                //print("2nd Half AVG Height: " + averageHeightNeeded);
            }

            heightLeft -= height;

            randomChunkHeights.Add(height);
        }

        // store end and beginning of current chunk
        Vector2 newGenEndPos;
        Vector2 lastChunkEndPosition = beginningGenPos;

        //get the necessecary length of each chunk to reach end
        float length = (possibleHorizontalChunks / horzChunksNeeded) * maxChunkLength;

        /* ======================================
         *  SPAWN CHUNKS WITH RANDOMIZED HEIGHTS
         * ====================================== */
        for (int i = 0; i < horzChunksNeeded; i++)
        {
            // get random index from list
            int randomHeightIndex = Random.Range(0, randomChunkHeights.Count);

            // set end position based off random height
            newGenEndPos = new Vector2(lastChunkEndPosition.x + length, lastChunkEndPosition.y + randomChunkHeights[randomHeightIndex]);

            // remove used height from list
            randomChunkHeights.Remove(randomChunkHeights[randomHeightIndex]);

            // spawn ground with values
            SpawnBezierGroundChunk(lastChunkEndPosition, newGenEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list

            // set last chunk pos to current end
            lastChunkEndPosition = newGenEndPos;
        }
    }

    public void SineChunkGenerator()
    {

        Debug.Log("Sine Generation Style");

        Vector2 lastChunkEndPosition = beginningGenPos; // init last chunk as the current beginning position

        int vertChunksNeeded = GetVerticalChunksNeeded();
        int horzChunksNeeded = GetHorizontalChunksNeeded();

        float currChunkHeight = maxChunkHeight; // init as max height

        // << SET CHUNK HEIGHT AND LENGTH >>
        float chunkLength = fullGenerationLength / horzChunksNeeded;

        // iterate through the num of chunks needed
        for (int i = horzChunksNeeded; i > 0; i--)
        {
            // last chunk needs to meet end of generation
            if (i == 1)
            {
                // create new chunk starting at the last chunks end point and ending at the full length of this chunk
                Vector2 endGenPos = new Vector2(lastChunkEndPosition.x + chunkLength, endGenerationPoint.position.y);
                SpawnBezierGroundChunk(lastChunkEndPosition, endGenPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list
                return;
            }


            // create new chunk starting at the last chunks end point and ending at the full length of this chunk
            Vector2 newGenEndPos = new Vector2(lastChunkEndPosition.x + chunkLength, lastChunkEndPosition.y + currChunkHeight);
            SpawnBezierGroundChunk(lastChunkEndPosition, newGenEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list

            // update last chunk end position
            lastChunkEndPosition = newGenEndPos;

            // invert height
            currChunkHeight *= -1;


        }
    }

    public void CustomSineChunkGenerator(Vector2 lengthRange, Vector2 heightRange)
    {

        Debug.Log("Custom Sine Generation Style");
        List<float> chunkLengths = new List<float>();


        Vector2 lastChunkEndPosition = beginningGenPos; // init last chunk as the current beginning position

        // << GENERATE RANDOM LENGTH VALUES >>
        float currentDistance = 0f;
        while (currentDistance < fullGenerationLength)
        {
            // get random range length of chunk
            float newChunkLength = Random.Range(lengthRange.x, lengthRange.y);

            // if next chunk will be longer than generation length, make last chunk meet full generation
            if (currentDistance + newChunkLength > fullGenerationLength)
            {
                chunkLengths[chunkLengths.Count - 1] += fullGenerationLength - currentDistance;
                currentDistance += fullGenerationLength - currentDistance;
            }
            // add random length to list
            else
            {
                chunkLengths.Add(newChunkLength);
                currentDistance += newChunkLength;
            }

        }

        // << SPAWN SINE HILL CHUNKS BASED ON LENGTHS >>
        for (int i = 0; i < chunkLengths.Count; i++)
        {
            float curChunkLength = chunkLengths[i];

            float curChunkHeight = Random.Range(heightRange.x, heightRange.y);


            // UPHILL CHUNK 
            Vector2 uphillChunkEndPos = new Vector2(lastChunkEndPosition.x + curChunkLength/2, endGenPos.y + curChunkHeight);
            SpawnBezierGroundChunk(lastChunkEndPosition, uphillChunkEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]);

            // DOWNHILL CHUNK 
            Vector2 downhillChunkEndPos = new Vector2(uphillChunkEndPos.x + curChunkLength/2, endGenPos.y);
            SpawnBezierGroundChunk(uphillChunkEndPos, downhillChunkEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]);


            // update last chunk end position
            lastChunkEndPosition = downhillChunkEndPos;


        }
    }

    #endregion

    #region SPAWNING ==========================
    // [[ CREATE NEW CHUNK ]] ==== >> style parameter defaults to flat hills generation
    public BezierCurveGeneration SpawnBezierGroundChunk(Vector2 begPos, Vector2 endPos, CHUNK_STYLES style = CHUNK_STYLES.random)
    {
        // get distance between beggining and end then spawn in middle
        float distance = Vector3.Distance(begPos, endPos);
        Vector3 newGenPosParentPos = new Vector3(begPos.x + distance / 2, begPos.y);

        // create new bezierCurveGeneration and store reference to script
        GameObject newCurveObject = Instantiate(curveGenerationPrefab, newGenPosParentPos, Quaternion.identity);
        BezierCurveGeneration bezierGroundGen = newCurveObject.GetComponent<BezierCurveGeneration>();


        newCurveObject.SetActive(true); // set curve gen as active
        newCurveObject.transform.parent = chunkParent.transform; // set parent
        chunks.Add(newCurveObject); // add to chunks list

        // set beginning and end points of the bezier curve
        bezierGroundGen.p0_pos = begPos;
        bezierGroundGen.p3_pos = endPos;

        bezierGroundGen.SetAngleType();


        // set camera override
        if (inCameraRangeOverride)
        {
            bezierGroundGen.cameraRangeOverride = true;
        }

        // << SET GENERATION STYLE >>
        Debug.Log("Chunk Style: " + style, gameObject);

        // if random 
        if (style == CHUNK_STYLES.random)
        {
            int enumLength = System.Enum.GetValues(typeof(CHUNK_STYLES)).Length;
            style = (CHUNK_STYLES)Random.Range(1, enumLength);
        }

        // set style
        if (style == CHUNK_STYLES.rounded) { SetRoundedHillsGeneration(bezierGroundGen); }
        else if (style == CHUNK_STYLES.straight) { SetStraightHillsGeneration(bezierGroundGen); }
        else if (style == CHUNK_STYLES.flat) { SetFlatHillsGeneration(bezierGroundGen); }

        // reset generation
        bezierGroundGen.first_generation = true;

        return bezierGroundGen;
    }
    #endregion

    #region CHUNK GENERATION STYLES =======================================================================



    // These are deciding the position of the middle edit points to create certain types of bezier curves
    // the x position is base on distance, the y on height distance

    public void SetRoundedHillsGeneration(BezierCurveGeneration ground)
    {
        // get beg and end pos
        Vector2 begPos = ground.p0_pos;
        Vector2 endPos = ground.p3_pos;

        // get distances
        float horzDistance = Vector3.Distance(begPos, endPos);
        float vertDistance = Mathf.Abs(endPos.y - begPos.y);

        Debug.Log("Rounded Hills gen angle " + ground.generationAngleType);

        if (ground.generationAngleType == "uphill")
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 3, begPos.y);
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 3, endPos.y);
        }
        else
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 3, begPos.y);
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 3, endPos.y);
        }
    }

    public void SetStraightHillsGeneration(BezierCurveGeneration ground)
    {
        // get beg and end pos
        Vector2 begPos = ground.p0_pos;
        Vector2 endPos = ground.p3_pos;

        // get distances
        float horzDistance = Vector3.Distance(begPos, endPos);
        float vertDistance = Mathf.Abs(endPos.y - begPos.y);

        if (ground.generationAngleType == "uphill")
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 2, begPos.y );
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 5, endPos.y - vertDistance / 2);
        }
        else
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 5, begPos.y - vertDistance / 2);
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 2, endPos.y);
        }
    }

    public void SetFlatHillsGeneration(BezierCurveGeneration ground)
    {
        // get beg and end pos
        Vector2 begPos = ground.p0_pos;
        Vector2 endPos = ground.p3_pos;

        // get distances
        float horzDistance = Vector3.Distance(begPos, endPos);
        float vertDistance = Mathf.Abs(endPos.y - begPos.y);


        if (ground.generationAngleType == "uphill")
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 30, begPos.y + vertDistance / 20);
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 30, endPos.y - vertDistance / 20);
        }
        else
        {
            ground.p1_pos = new Vector3(begPos.x + horzDistance / 30, begPos.y - vertDistance / 20);
            ground.p2_pos = new Vector3(endPos.x - horzDistance / 30, endPos.y + vertDistance / 20);
        }
    }

    #endregion

    #region HELPER FUNCTIONS =========================================================================

    // returns the number of horizontal chunks needed based off of the max height size
    public int GetHorizontalChunksNeeded()
    {

        // << GET HORZ CHUNK COUNT >>
        float possibleHorizontalChunks = fullGenerationLength / maxChunkLength; //how many chunks can we fit in this space?
        int maxNumofFullLengthChunks = Mathf.FloorToInt(possibleHorizontalChunks); //max full chunks of size 10

        // << RETURN NUMBER OF CHUNKS >>
        // if possible chunks is a decimal number, return rounded up number of chunks
        if (possibleHorizontalChunks > maxNumofFullLengthChunks)
        {
            return maxNumofFullLengthChunks + 1;
        }
        // if possible chunks is a whole number, return
        else
        {
            return maxNumofFullLengthChunks;
        }
    }

    // returns the number of vertical chunks needed based off of the max height size
    public int GetVerticalChunksNeeded()
    {
        // << GET VERT CHUNK COUNT >>
        float possibleVerticalChunks = fullGenerationHeight / maxChunkHeight;
        int maxNumofFullHeightChunks = Mathf.FloorToInt(possibleVerticalChunks);

        // << RETURN NUMBER OF CHUNKS >>
        // if possible chunks is a decimal number, return rounded up number of chunks
        if (possibleVerticalChunks > maxNumofFullHeightChunks)
        {
            return maxNumofFullHeightChunks + 1;
        }
        // if possible chunks is a whole number
        else
        {
            return maxNumofFullHeightChunks;
        }
    }

    // sets all of the ground points and rotations of each chunk
    public void SetAllGroundPoints()
    {
        allGroundPoints.Clear();
        allGroundRotations.Clear();

        //get positions from point lists in each chunk
        foreach (GameObject chunk in chunks)
        {
            //Debug.Log("chunk point count: " + chunk.GetComponent<BezierCurveGeneration>().generatedCurvePoints.Count);

            //add points and rotations to main list
            foreach (Vector3 point in chunk.GetComponent<BezierCurveGeneration>().generatedCurvePoints)
            {
                allGroundPoints.Add(point);
            }

            foreach (float rot in chunk.GetComponent<BezierCurveGeneration>().generatedCurvePointRotations)
            {
                allGroundRotations.Add(rot);
            }
        }

        generationFinished = true;

        chunkParent.SetActive(false); // disable chunk parent
    }

    public int GetClosestGroundPointIndexToPos(Vector3 pos)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < allGroundPoints.Count; i++)
        {
            float distance = Vector3.Distance(pos, allGroundPoints[i]);

            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }


    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(begGenerationPoint.position, endGenerationPoint.position);

        // << FLAT START ZONE >>
        Gizmos.color = gizmosColor;
        Vector2 offsetPos = begGenerationPoint.position + new Vector3(begGenOffset, maxChunkHeight); // init last chunk as the current beginning position
        Gizmos.DrawLine(offsetPos + new Vector2(begGenOffset, 0), offsetPos); // spawn flat beginning

        // << HILL TO GAIN SPEED >>
        Gizmos.DrawLine(offsetPos, begGenerationPoint.position); // spawn

        // << END OFFSET >>
        Vector2 endOffsetPosition = endGenerationPoint.position + new Vector3(endGenOffset, 0); ; // init last chunk as the current beginning position
        Gizmos.DrawLine(endGenerationPoint.position, endOffsetPosition); // spawn
    }
}

