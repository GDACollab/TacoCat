using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public bool allStagesGenerated;
    public GroundMeshCreator meshCreator;

    [Header("[[ GENERATION LENGTHS ]]")]
    public int mainGenerationLength = 30000;
    public Vector3 generationOffset;

    [HideInInspector]
    public Vector3 main_begPos, main_endPos;

    [Header("End Island Offsets")]
    public int startIslandXOffset = -1000;
    public int startIslandYOffset = -1000;
    [Space(10)]
    public int endIslandXOffset = 1000;

    [Header("[[ GROUND GENERATION ]]")]
    int numStages;
    int stageLength; // set based on mainGenerationLength / stages

    // ground generation values
    public List<GroundGeneration> stages;
    public List<BezierCurveGeneration> allStageChunks = new List<BezierCurveGeneration>();
    [HideInInspector]
    public List<Vector3> allStageGroundPoints = new List<Vector3>(); // all ground points of the chunks
    [HideInInspector]
    public List<float> allStageGroundRotations = new List<float>(); // all ground rotations of the chunks

    public void BeginStageGeneration()
    {
        StartCoroutine(StageGeneration());
    }

    public IEnumerator StageGeneration()
    {
        // get end points
        main_begPos = this.transform.position;
        main_endPos = main_begPos + new Vector3(mainGenerationLength, 0);

        // get stage length
        numStages = stages.Count;
        stageLength = mainGenerationLength / numStages;

        // [[ GENERATE EACH STAGE ]]
        if (stages.Count > 0)
        {
            Vector3 newStageBeginningPos = this.transform.position;

            for (int i = 0; i < stages.Count; i++)
            {
                GroundGeneration groundGen = stages[i];
                
                // set end islands
                if (i == 0) 
                { 
                    groundGen.startIsland = true;
                    groundGen.startIslandXOffset = startIslandXOffset;
                    groundGen.startIslandYOffset = startIslandYOffset;
                }
                if (i == stages.Count - 1) 
                { 
                    groundGen.endIsland = true;
                    groundGen.endIslandXOffset = endIslandXOffset;
                }

                // start generation
                groundGen.begGenPos = new Vector3(newStageBeginningPos.x, newStageBeginningPos.y); // set beginning pos based on stage length
                groundGen.endGenPos = new Vector3(newStageBeginningPos.x + stageLength, newStageBeginningPos.y); // set end pos based on stage length
                groundGen.CreateGeneration();

                // wait until generation is finished
                yield return new WaitUntil(() => groundGen.generationFinished);

                // add all generation points
                allStageChunks.AddRange(groundGen.chunks);
                allStageGroundPoints.AddRange(groundGen.allGroundPoints);
                allStageGroundRotations.AddRange(groundGen.allGroundRotations);

                // update new stage beginning
                newStageBeginningPos = groundGen.endGenPos;
            }

            // [[ CREATE MESH ]]
            if (meshCreator != null)
            {
                meshCreator.GenerateUndergroundMesh(allStageGroundPoints);
            }
            else { Debug.LogError("ERROR:: Mesh Creator is null", this.gameObject); }

            allStagesGenerated = true;
        }

        // Get Cam Bezier Points
        CameraHandler cam = Camera.main.GetComponent<CameraHandler>();
        cam.Init();

        yield return new WaitUntil(() => cam.foundGenerationPoints);


        // Destroy Chunks
        DestroyAllChunks();

    }

    public int PosToGroundPointIndex(Vector3 pos)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < allStageGroundPoints.Count; i++)
        {
            float distance = Vector3.Distance(pos, allStageGroundPoints[i]);

            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
    }

    public void DestroyAllChunks()
    {
        Debug.Log("Stage Destroy Chunks");
        foreach (BezierCurveGeneration curve in allStageChunks)
        {
            if (curve != null)
            {
                Destroy(curve.gameObject);
            }
        }

        allStageChunks.Clear();
    }

    private void OnDrawGizmos()
    {
        // show stage areas
        for (int i = 0; i < numStages; i++)
        {
            // switch between colors
            if (i % 2 == 0) { Gizmos.color = Color.blue; }
            else { Gizmos.color = Color.magenta; }

            // draw middle line
            Gizmos.DrawLine(
                new Vector3((i * stageLength), this.transform.position.y),
                new Vector3((i * stageLength) + stageLength, this.transform.position.y));

            // draw bounding box
            Gizmos.DrawWireCube(
                new Vector3((i * stageLength) + (stageLength / 2), this.transform.position.y), 
                new Vector3(stageLength, stageLength));

        }

        Gizmos.color = Color.green;
        // manually placed starting hill
        Vector3 begGenPos = stages[0].begGenPos;
        Vector3 xOffset = new Vector3(startIslandXOffset, 0);
        Vector3 yOffset = new Vector3(0, startIslandYOffset);
        // << FLAT START ZONE >>
        Gizmos.DrawLine(begGenPos + (5 * xOffset), begGenPos + (3 * xOffset));

        // << DOWNHILL TO GAIN SPEED >>
        Gizmos.DrawLine(begGenPos + (3 * xOffset), begGenPos + xOffset + yOffset);

        // << UPHILL TO LAUNCH >>
        Gizmos.DrawLine(begGenPos + xOffset + yOffset, begGenPos);



        // << END ISLAND >>
        Gizmos.color = Color.red;
        Gizmos.DrawLine(stages[stages.Count - 1].endGenPos, begGenPos);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (stages.Count > 0)
        {
            main_begPos = this.transform.position;
            main_endPos = main_begPos + new Vector3(mainGenerationLength, 0);
            stageLength = mainGenerationLength / stages.Count;
        }

    }
#endif

}
