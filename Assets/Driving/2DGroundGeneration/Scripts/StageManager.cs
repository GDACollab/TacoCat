using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public bool allStagesGenerated;
    public GroundMeshCreator meshCreator;

    [Header("[[ GENERATION LENGTHS ]]")]
    public int mainGenerationLength = 30000;
    public int undergroundHeight = -1000;
    private Vector3 main_begPos, main_endPos;

    [Header("End Island Offsets")]
    public int startIslandXOffset = -1000;
    public int startIslandYOffset = -1000;
    [Space(10)]
    public int endIslandXOffset = 1000;

    [Header("[[ GROUND GENERATION ]]")]
    public int numStages = 3;
    int stageLength; // set based on mainGenerationLength / stages

    // ground generation values
    public List<GroundGeneration> stages;
    [HideInInspector]
    public List<Vector3> allLevelGroundPoints = new List<Vector3>(); // all ground points of the chunks
    [HideInInspector]
    public List<float> allLevelGroundRotations = new List<float>(); // all ground rotations of the chunks

    void Awake()
    {
        StartCoroutine(StageGeneration());
    }

    public IEnumerator StageGeneration()
    {
        // get end points
        main_begPos = this.transform.position;
        main_endPos = main_begPos + new Vector3(mainGenerationLength, 0);

        // get stage length
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
                allLevelGroundPoints.AddRange(groundGen.allGroundPoints);
                allLevelGroundRotations.AddRange(groundGen.allGroundRotations);

                // update new stage beginning
                newStageBeginningPos = groundGen.endGenPos;
            }

            // [[ CREATE MESH ]]
            if (meshCreator != null)
            {
                meshCreator.GenerateUndergroundMesh(allLevelGroundPoints, undergroundHeight);
            }
            else { Debug.LogError("ERROR:: Mesh Creator is null", this.gameObject); }

            allStagesGenerated = true;
        }


    }

    public int GetClosestGroundPointIndexToPos(Vector3 pos)
    {
        int closestIndex = -1;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < allLevelGroundPoints.Count; i++)
        {
            float distance = Vector3.Distance(pos, allLevelGroundPoints[i]);

            if (distance < closestDistance)
            {
                closestIndex = i;
                closestDistance = distance;
            }
        }

        return closestIndex;
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

        // [[ START ISLAND ]]
        // << FLAT START ZONE >>
        Gizmos.color = Color.green;
        Vector3 offsetPos = main_begPos + new Vector3(startIslandXOffset, startIslandYOffset); // init last chunk as the current beginning position
        Gizmos.DrawLine(offsetPos + new Vector3(startIslandXOffset, 0), offsetPos); // spawn flat beginning

        // << HILL TO GAIN SPEED >>
        Gizmos.DrawLine(offsetPos, main_begPos); // spawn

        // [[ END ISLAND ]]
        // << END OFFSET >>
        Gizmos.color = Color.red;
        Vector3 endOffsetPosition = main_endPos + new Vector3(endIslandXOffset, 0); ; // init last chunk as the current beginning position
        Gizmos.DrawLine(main_endPos, endOffsetPosition); // spawn
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        main_begPos = this.transform.position;
        main_endPos = main_begPos + new Vector3(mainGenerationLength, 0);
        stageLength = mainGenerationLength / numStages;
    }
#endif

}
