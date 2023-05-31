using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacoMakingLighting : MonoBehaviour
{
    [Range(0, 1)]
    public float timeOfDay;
    public TIME_OF_DAY dayCycleState = TIME_OF_DAY.MORNING;

    [Header("Time Of Day Lighting Palettes")]
    // Morning Sunrise
    [Tooltip("0: InsideTruck, 1: Customer, 2: Background")]
    public List<Color> morningSunrise = new List<Color>
    {
        new Color32(255, 167, 38, 255),
        new Color32(255, 213, 79, 255),
        new Color32(245, 124, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Mid-day
    [Tooltip("0: InsideTruck, 1: Customer, 2: Background")]
    public List<Color> midDay = new List<Color>
    {
        new Color32(41, 182, 246, 255),
        new Color32(79, 195, 247, 255),
        new Color32(3, 155, 229, 255),
        new Color32(129, 212, 250, 255)
    };

    // Evening Sunset
    [Tooltip("0: InsideTruck, 1: Customer, 2: Background")]
    public List<Color> eveningSunset = new List<Color>
    {
        new Color32(239, 108, 0, 255),
        new Color32(255, 167, 38, 255),
        new Color32(230, 81, 0, 255),
        new Color32(255, 204, 128, 255)
    };

    // Night
    [Tooltip("0: InsideTruck, 1: Customer, 2: Background")]
    public List<Color> night = new List<Color>
    {
        new Color32(40, 53, 147, 255),
        new Color32(92, 107, 192, 255),
        new Color32(26, 35, 126, 255),
        new Color32(159, 168, 218, 255)
    };


    [Tooltip("Percentage of TimeOfDay Between Palette Tranisitions. Sum of values should equal 1.")]
    public float morningTimeLength = 0.1f;
    public float midDayTimeLength = 0.5f;
    public float eveningTimeLength = 0.3f;
    public float nightTimeLength = 0.1f;

    [Space(10)]
    public List<float> sunIntensities = new List<float>();

    [Space(10)]
    public float lightColorAdjustSpeed = 2;

    [Space(20)]
    [Header("SETUP VARIABLES [[DONT TOUCH]]")]
    // Hold lights for each sorting layer
    public UnityEngine.Rendering.Universal.Light2D insideTruckLight;
    public UnityEngine.Rendering.Universal.Light2D customerLight;
    public UnityEngine.Rendering.Universal.Light2D backgroundLight;
    public UnityEngine.Rendering.Universal.Light2D skyboxLight;

    [Space(10)]
    // Hold each art asset in a list
    public List<GameObject> foregroundObjects;
    public List<GameObject> playAreaObjects;
    public List<GameObject> backgroundLayerObjects;
    public List<GameObject> skyboxLayerObjects;

    [Space(10)]
    public SunCycle sunCycle;
    public UnityEngine.Rendering.Universal.Light2D sunLight;

    [Header("SPACING")]
    // Space the layers out based on a modular float for perspective things
    [Range(0, 1000)]
    public float layerSpacing = 1000f;
    [Range(0, 5000)]
    public float skyboxSpacing = 3000f;

    // Update is called once per frame
    void Update()
    {
        UpdateLightColors(timeOfDay);
    }

    public void UpdateLightColors(float curTime)
    {

        List<Color> toPalette = GetPaletteAtTime(curTime);

        insideTruckLight.color = Color.Lerp(insideTruckLight.color, toPalette[0], lightColorAdjustSpeed * Time.deltaTime);
        customerLight.color = Color.Lerp(customerLight.color, toPalette[1], lightColorAdjustSpeed * Time.deltaTime);
        backgroundLight.color = Color.Lerp(backgroundLight.color, toPalette[2], lightColorAdjustSpeed * Time.deltaTime);

    }

    private List<Color> GetPaletteAtTime(float curTime)
    {
        List<Color>[] palettes = new List<Color>[]
        {
            morningSunrise,
            midDay,
            eveningSunset,
            night
        };

        // MORNING
        if (curTime < morningTimeLength)
        {
            dayCycleState = TIME_OF_DAY.MORNING;
            return palettes[0];
        }
        // MIDDAY
        else if (curTime < (morningTimeLength + midDayTimeLength))
        {
            dayCycleState = TIME_OF_DAY.MIDDAY;
            return palettes[1];
        }
        // EVENING
        if (curTime < (morningTimeLength + midDayTimeLength + eveningTimeLength))
        {
            dayCycleState = TIME_OF_DAY.EVENING;
            return palettes[2];
        }
        // NIGHT
        if (curTime > (morningTimeLength + midDayTimeLength + eveningTimeLength))
        {
            dayCycleState = TIME_OF_DAY.NIGHT;
            return palettes[3];
        }

        dayCycleState = TIME_OF_DAY.ERROR;
        return palettes[3];


    }


    public void OverwriteLightingPalette(float curTime)
    {
        List<Color> toPalette = GetPaletteAtTime(curTime);

        insideTruckLight.color = Color.Lerp(insideTruckLight.color, toPalette[0], lightColorAdjustSpeed * Time.deltaTime);
        customerLight.color = Color.Lerp(customerLight.color, toPalette[1], lightColorAdjustSpeed * Time.deltaTime);
        backgroundLight.color = Color.Lerp(backgroundLight.color, toPalette[2], lightColorAdjustSpeed * Time.deltaTime);
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