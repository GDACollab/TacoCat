using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GroundGeneration : MonoBehaviour
{
    /* ===========================
     * Sky Casey , updated 2023
     * 
     * This script takes the BezierCurve Objects to the next level
     * It takes in a beginning position and an end position and places multiple "chunks" of bezier ground generations between them.
     * It also needs a max length and height of each chunk
     * ===================================================================================================
     */

    public enum GENERATION_STYLES { consistent, sine, custom_sine, random };
    public enum CHUNK_STYLES { random, rounded, straight, flat };

    [Header("Generation References")]
    public GameObject bezierCurvePrefab;
    Transform chunkGenParent;
    List<GameObject> chunks = new List<GameObject>();
    public List<Vector3> allGroundPoints = new List<Vector3>(); // all ground points of the chunks
    public List<float> allGroundRotations = new List<float>(); // all ground rotations of the chunks

    [Header("Generation Type")]
    public bool startIsland;
    public bool endIsland;

    [Header("Generation State")]
    bool startIslandGenerated;
    bool mainGroundGenerated;
    bool endIslandGenerated;
    public bool generationFinished;

    [Header("Generation Values")]
    public Vector3 begGenPos, endGenPos; // end positions
    [HideInInspector]
    public float fullGenerationLength, fullGenerationHeight;  // store full generation length and height

    // [[ GENERATION STYLE ]]
    public GENERATION_STYLES generationStyle = GENERATION_STYLES.sine;

    // >> CUSTOM SINE RANGES
    public Vector2 chunkLengthRange = new Vector2(200, 500);
    public Vector2 chunkHeightRange = new Vector2(200, 500);

    // >> DEFAULT MAX CHUNK VALUES
    public int maxChunkLength = 700;
    public int maxChunkHeight = 300;

    [Tooltip("Choose the style of each chunk")]
    public List<CHUNK_STYLES> chunkStyles = new List<CHUNK_STYLES>();
    // [[ END ISLAND OFFSETS ]]
    public int startIslandXOffset = -700;
    public int startIslandYOffset = -700;
    public int endIslandXOffset = 700;
    public int endIslandYOffset = 700;


    public void CreateGeneration()
    {
        // create chunk parent
        chunkGenParent = new GameObject("Chunk Parent").transform;
        chunkGenParent.parent = this.transform;

        // start generation
        StartCoroutine( NewGeneration(generationStyle) );
    }

    public IEnumerator NewGeneration(GENERATION_STYLES style)
    {
        generationFinished = false;

        // [[ BREAK OUT OF COROUTINE IF END POINTS ARE NOT SET ]]
        if (begGenPos == Vector3.zero && endGenPos == Vector3.zero)
        {
            Debug.LogError("GENERATION ERROR:: End Points are not set");
            yield return null;
        }

        // get full generation size
        fullGenerationLength = endGenPos.x - begGenPos.x;
        fullGenerationHeight = endGenPos.y - begGenPos.y;

        // destroy all current chunks, if any
        if (chunks.Count > 0) { DestroyAllChunks(); }

        // [[ GENERATION ]]
        Debug.Log(">> NEW GENERATION ( " + this.gameObject.name + " )", this.gameObject);

        // generate start
        if (startIsland)
        {
            StartIslandGenerator();
            yield return new WaitUntil(() => startIslandGenerated);
            Debug.Log("----> Start Island Generated", this.gameObject);
        }

        // generate main ground
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
        yield return new WaitUntil(() => mainGroundGenerated);
        Debug.Log("----> Main Ground Generated", this.gameObject);

        // generate end
        if (endIsland)
        {
            EndIslandGenerator();
            yield return new WaitUntil(() => endIslandGenerated);
            Debug.Log("----> End Island Generated", this.gameObject);
        }

        // save all ground points
        SaveAllGroundPoints();
        yield return new WaitUntil(() => generationFinished);

        Debug.Log(">> " + this.gameObject.name + " Generation Finished [ " + chunks.Count + " chunks ]", this.gameObject);
    }

    public void DestroyGenerationObjects()
    {
        DestroyAllChunks();
        Destroy(this.gameObject);
    }

    #region GENERATION ====================================================

    public void StartIslandGenerator()
    {
        Vector2 offsetPos = begGenPos + new Vector3(startIslandXOffset, maxChunkHeight); // init last chunk as the current beginning position

        // << FLAT START ZONE >>
        SpawnBezierGroundChunk(offsetPos + new Vector2(startIslandXOffset, 0), offsetPos, CHUNK_STYLES.flat); // spawn flat beginning

        // << HILL TO GAIN SPEED >>
        SpawnBezierGroundChunk(offsetPos, begGenPos, CHUNK_STYLES.rounded); // spawn

        startIslandGenerated = true;

    }

    public void EndIslandGenerator()
    {
        Vector2 offsetPos = endGenPos + new Vector3(endIslandXOffset, 0); ; // init last chunk as the current beginning position

        SpawnBezierGroundChunk(endGenPos, offsetPos, CHUNK_STYLES.flat); // spawn flat beginning

        endIslandGenerated = true;
    }

    public void ConsistentChunkGenerator()
    {
        Debug.Log("Consistent Generation Style");

        Vector2 lastChunkEndPosition = begGenPos; // init last chunk as the current beginning position

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

        mainGroundGenerated = true;
    }

    public void RandomChunkGenerator()
    {
        Debug.Log("Random Generation Style");

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
        Vector2 lastChunkEndPosition = begGenPos;

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

        mainGroundGenerated = true;

    }

    public void SineChunkGenerator()
    {
        Debug.Log("Sine Generation Style");

        Vector2 lastChunkEndPosition = begGenPos; // init last chunk as the current beginning position

        int vertChunksNeeded = GetVerticalChunksNeeded();
        int horzChunksNeeded = GetHorizontalChunksNeeded();

        float currChunkHeight = maxChunkHeight; // init as max height

        // << SET CHUNK HEIGHT AND LENGTH >>
        float chunkLength = fullGenerationLength / horzChunksNeeded;

        // iterate through the num of chunks needed
        for (int i = horzChunksNeeded; i > 0; i--)
        {
            // Debug.Log(" ++ " + i + " / " + horzChunksNeeded + " chunks");

            // last chunk needs to meet end of generation
            if (i == 1)
            {
                // create new chunk starting at the last chunks end point and ending at the full length of this chunk
                Vector3 newEndGenPos = new Vector3(lastChunkEndPosition.x + chunkLength, endGenPos.y);
                SpawnBezierGroundChunk(lastChunkEndPosition, newEndGenPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list
                break;
            }

            // create new chunk starting at the last chunks end point and ending at the full length of this chunk
            Vector2 newGenEndPos = new Vector2(lastChunkEndPosition.x + chunkLength, lastChunkEndPosition.y + currChunkHeight);
            SpawnBezierGroundChunk(lastChunkEndPosition, newGenEndPos, chunkStyles[Random.Range(0, chunkStyles.Count)]); // use random chunk style from list

            // update last chunk end position
            lastChunkEndPosition = newGenEndPos;

            // invert height
            currChunkHeight *= -1;

        }

        mainGroundGenerated = true;
    }

    public void CustomSineChunkGenerator(Vector2 lengthRange, Vector2 heightRange)
    {
        Debug.Log("Custom Sine Generation Style");
        List<float> chunkLengths = new List<float>();

        Vector2 lastChunkEndPosition = begGenPos; // init last chunk as the current beginning position

        // << GENERATE RANDOM LENGTH VALUES >>
        float currentDistance = 0f;
        while (currentDistance < fullGenerationLength)
        {
            // get random range length of chunk
            float newChunkLength = Random.Range(lengthRange.x, lengthRange.y);

            // if next chunk will be longer than generation length, make last chunk meet full generation
            if (currentDistance + newChunkLength > fullGenerationLength)
            {
                if (chunkLengths.Count == 0) 
                { 
                    Debug.LogError("ERROR:: Custom Sine Gen Length is bigger than full generation length", this.gameObject);
                    return;
                }
                    
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

        mainGroundGenerated = true;

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
        GameObject newCurveObject = Instantiate(bezierCurvePrefab, newGenPosParentPos, Quaternion.identity);
        BezierCurveGeneration bezierGroundGen = newCurveObject.GetComponent<BezierCurveGeneration>();

        newCurveObject.SetActive(true); // set curve gen as active
        newCurveObject.transform.parent = chunkGenParent.transform; // set parent
        chunks.Add(newCurveObject); // add to chunks list

        // set beginning and end points of the bezier curve
        bezierGroundGen.p0_pos = begPos;
        bezierGroundGen.p3_pos = endPos;

        // set uphill, downhill, or flat
        bezierGroundGen.SetAngleType();

        // << SET GENERATION STYLE >>
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
    public void SaveAllGroundPoints()
    {
        allGroundPoints.Clear();
        allGroundRotations.Clear();

        //get positions from point lists in each chunk
        foreach (GameObject chunk in chunks)
        {
            allGroundPoints.AddRange(chunk.GetComponent<BezierCurveGeneration>().lineCurvePositions);
            allGroundRotations.AddRange(chunk.GetComponent<BezierCurveGeneration>().generatedRotations);
        }

        generationFinished = true;
    }

    public void DestroyAllChunks()
    {
        foreach (GameObject obj in chunks)
        {
            Destroy(obj);
        }
        chunks.Clear();
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

    }
}

