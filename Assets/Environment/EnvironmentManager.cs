using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class EnvironmentManager : MonoBehaviour
{
    // hold lights for each sorting layer
    public Light2D foregroundLight;
    public Light2D playAreaLight;
    public Light2D backgroundLight;
    public Light2D skyboxLight;

    // hold each art asset in a list
    public List<GameObject> foregroundObjects;
    public List<GameObject> playAreaObjects;
    public List<GameObject> backgroundLayerObjects;
    public List<GameObject> skyboxLayerObjects;

    // effect the colors of each different layer independently and globally
    public Color foregroundGlobalColor;
    public Color playAreaGlobalColor;
    public Color backgroundGlobalColor;
    public Color skyboxGlobalColor;

    // space the layers out based on a modular float for perspective things
    [Range(0, 1000)]
    public float layerSpacing = 1000f;
    [Range(0, 5000)]
    public float skyboxSpacing = 3000f;

    private void Start()
    {
        SetZPositions();

    }

    // Update is called once per frame
    void Update()
    {
        // UPDATE GLOBAL COLORS
        foregroundLight.color = foregroundGlobalColor;
        playAreaLight.color = playAreaGlobalColor;
        backgroundLight.color = backgroundGlobalColor;
        skyboxLight.color = skyboxGlobalColor;


        SetZPositions();
    }


    public void SetZPositions()
    {
        // << SET Z POSITIONS >>
        // set foreground position
        foreach (GameObject obj in foregroundObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, -layerSpacing);
        }

        // set play area position
        foreach (GameObject obj in playAreaObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
        }

        // set background position
        foreach (GameObject obj in backgroundLayerObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, layerSpacing);
        }

        // set skybox position
        foreach (GameObject obj in skyboxLayerObjects)
        {
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, skyboxSpacing);
        }
    }
}
